using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheDialgaTeam.Commands.Builders;
using TheDialgaTeam.Commands.Info;
using TheDialgaTeam.Commands.Map;
using TheDialgaTeam.Commands.Readers;
using TheDialgaTeam.Commands.Results;

namespace TheDialgaTeam.Commands
{
    /// <summary>
    ///     Provides a framework for building Discord commands.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The service provides a framework for building Discord commands both dynamically via runtime builders or
    ///         statically via compile-time modules. To create a command module at compile-time, see
    ///         <see cref="ModuleBase" /> (most common); otherwise, see <see cref="ModuleBuilder" />.
    ///     </para>
    /// </remarks>
    public class CommandService
    {
        private readonly SemaphoreSlim _moduleLock;
        private readonly ConcurrentDictionary<Type, ModuleInfo> _typedModuleDefs;
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>> _typeReaders;
        private readonly ConcurrentDictionary<Type, TypeReader> _defaultTypeReaders;
        private readonly HashSet<ModuleInfo> _moduleDefs;
        private readonly CommandMap _map;

        internal readonly bool _caseSensitive, _throwOnError, _ignoreExtraArgs;
        internal readonly char _separatorChar;
        internal readonly RunMode _defaultRunMode;
        internal readonly IReadOnlyDictionary<char, char> _quotationMarkAliasMap;

        /// <summary>
        ///     Represents all modules loaded within <see cref="CommandService" />.
        /// </summary>
        public IEnumerable<ModuleInfo> Modules => _moduleDefs.Select(x => x);

        /// <summary>
        ///     Represents all commands loaded within <see cref="CommandService" />.
        /// </summary>
        public IEnumerable<CommandInfo> Commands => _moduleDefs.SelectMany(x => x.Commands);

        /// <summary>
        ///     Represents all <see cref="TypeReader" /> loaded within <see cref="CommandService" />.
        /// </summary>
        public ILookup<Type, TypeReader> TypeReaders => _typeReaders.SelectMany(x => x.Value.Select(y => new { y.Key, y.Value })).ToLookup(x => x.Key, x => x.Value);

        /// <summary>
        ///     Initializes a new <see cref="CommandService" /> class.
        /// </summary>
        public CommandService() : this(new CommandServiceConfig())
        {
        }

        /// <summary>
        ///     Initializes a new <see cref="CommandService" /> class with the provided configuration.
        /// </summary>
        /// <param name="config">The configuration class.</param>
        /// <exception cref="InvalidOperationException">
        ///     The <see cref="RunMode" /> cannot be set to <see cref="RunMode.Default" />.
        /// </exception>
        public CommandService(CommandServiceConfig config)
        {
            _caseSensitive = config.CaseSensitiveCommands;
            _throwOnError = config.ThrowOnError;
            _ignoreExtraArgs = config.IgnoreExtraArgs;
            _separatorChar = config.SeparatorChar;
            _defaultRunMode = config.DefaultRunMode;
            _quotationMarkAliasMap = (config.QuotationMarkAliasMap ?? new Dictionary<char, char>()).ToImmutableDictionary();
            if (_defaultRunMode == RunMode.Default)
                throw new InvalidOperationException("The default run mode cannot be set to Default.");

            _moduleLock = new SemaphoreSlim(1, 1);
            _typedModuleDefs = new ConcurrentDictionary<Type, ModuleInfo>();
            _moduleDefs = new HashSet<ModuleInfo>();
            _map = new CommandMap(this);
            _typeReaders = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>>();

            _defaultTypeReaders = new ConcurrentDictionary<Type, TypeReader>();

            foreach (var type in PrimitiveParsers.SupportedTypes)
            {
                _defaultTypeReaders[type] = PrimitiveTypeReader.Create(type);
                _defaultTypeReaders[typeof(Nullable<>).MakeGenericType(type)] = NullableTypeReader.Create(type, _defaultTypeReaders[type]);
            }

            var tsreader = new TimeSpanTypeReader();
            _defaultTypeReaders[typeof(TimeSpan)] = tsreader;
            _defaultTypeReaders[typeof(TimeSpan?)] = NullableTypeReader.Create(typeof(TimeSpan), tsreader);

            _defaultTypeReaders[typeof(string)] =
                new PrimitiveTypeReader<string>((string x, out string y) =>
                {
                    y = x;
                    return true;
                }, 0);
        }

        //Modules
        public async Task<ModuleInfo> CreateModuleAsync(string primaryAlias, Action<ModuleBuilder> buildFunc)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                var builder = new ModuleBuilder(this, null, primaryAlias);
                buildFunc(builder);

                var module = builder.Build(this, null);

                return LoadModuleInternal(module);
            }
            finally
            {
                _moduleLock.Release();
            }
        }

        /// <summary>
        ///     Add a command module from a <see cref="Type" />.
        /// </summary>
        /// <example>
        ///     <para>The following example registers the module <c>MyModule</c> to <c>commandService</c>.</para>
        ///     <code language="cs">
        ///     await commandService.AddModuleAsync&lt;MyModule&gt;(serviceProvider);
        ///     </code>
        /// </example>
        /// <typeparam name="T">The type of module.</typeparam>
        /// <param name="services">
        ///     The <see cref="IServiceProvider" /> for your dependency injection solution if using one;
        ///     otherwise, pass <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentException">This module has already been added.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The <see cref="ModuleInfo" /> fails to be built; an invalid type may have been provided.
        /// </exception>
        /// <returns>
        ///     A task that represents the asynchronous operation for adding the module. The task result contains the
        ///     built module.
        /// </returns>
        public Task<ModuleInfo> AddModuleAsync<T>(IServiceProvider services)
        {
            return AddModuleAsync(typeof(T), services);
        }

        /// <summary>
        ///     Adds a command module from a <see cref="Type" />.
        /// </summary>
        /// <param name="type">The type of module.</param>
        /// <param name="services">
        ///     The <see cref="IServiceProvider" /> for your dependency injection solution if using one;
        ///     otherwise, pass <c>null</c> .
        /// </param>
        /// <exception cref="ArgumentException">This module has already been added.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The <see cref="ModuleInfo" /> fails to be built; an invalid type may have been provided.
        /// </exception>
        /// <returns>
        ///     A task that represents the asynchronous operation for adding the module. The task result contains the
        ///     built module.
        /// </returns>
        public async Task<ModuleInfo> AddModuleAsync(Type type, IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            await _moduleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                var typeInfo = type.GetTypeInfo();

                if (_typedModuleDefs.ContainsKey(type))
                    throw new ArgumentException("This module has already been added.");

                var module = (await ModuleClassBuilder.BuildAsync(this, services, typeInfo).ConfigureAwait(false)).FirstOrDefault();

                if (module.Value == default(ModuleInfo))
                    throw new InvalidOperationException($"Could not build the module {type.FullName}, did you pass an invalid type?");

                _typedModuleDefs[module.Key] = module.Value;

                return LoadModuleInternal(module.Value);
            }
            finally
            {
                _moduleLock.Release();
            }
        }

        /// <summary>
        ///     Add command modules from an <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> containing command modules.</param>
        /// <param name="services">
        ///     The <see cref="IServiceProvider" /> for your dependency injection solution if using one;
        ///     otherwise, pass <c>null</c>.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation for adding the command modules. The task result
        ///     contains an enumerable collection of modules added.
        /// </returns>
        public async Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            await _moduleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                var types = await ModuleClassBuilder.SearchAsync(assembly, this).ConfigureAwait(false);
                var moduleDefs = await ModuleClassBuilder.BuildAsync(types, this, services).ConfigureAwait(false);

                foreach (var info in moduleDefs)
                {
                    _typedModuleDefs[info.Key] = info.Value;
                    LoadModuleInternal(info.Value);
                }

                return moduleDefs.Select(x => x.Value).ToImmutableArray();
            }
            finally
            {
                _moduleLock.Release();
            }
        }

        private ModuleInfo LoadModuleInternal(ModuleInfo module)
        {
            _moduleDefs.Add(module);

            foreach (var command in module.Commands)
                _map.AddCommand(command);

            foreach (var submodule in module.Submodules)
                LoadModuleInternal(submodule);

            return module;
        }

        /// <summary>
        ///     Removes the command module.
        /// </summary>
        /// <param name="module">The <see cref="ModuleInfo" /> to be removed from the service.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation. The task result contains a value that
        ///     indicates whether the <paramref name="module" /> is successfully removed.
        /// </returns>
        public async Task<bool> RemoveModuleAsync(ModuleInfo module)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                return RemoveModuleInternal(module);
            }
            finally
            {
                _moduleLock.Release();
            }
        }

        /// <summary>
        ///     Removes the command module.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the module.</typeparam>
        /// <returns>
        ///     A task that represents the asynchronous removal operation. The task result contains a value that
        ///     indicates whether the module is successfully removed.
        /// </returns>
        public Task<bool> RemoveModuleAsync<T>()
        {
            return RemoveModuleAsync(typeof(T));
        }

        /// <summary>
        ///     Removes the command module.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the module.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation. The task result contains a value that
        ///     indicates whether the module is successfully removed.
        /// </returns>
        public async Task<bool> RemoveModuleAsync(Type type)
        {
            await _moduleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!_typedModuleDefs.TryRemove(type, out var module))
                    return false;

                return RemoveModuleInternal(module);
            }
            finally
            {
                _moduleLock.Release();
            }
        }

        private bool RemoveModuleInternal(ModuleInfo module)
        {
            if (!_moduleDefs.Remove(module))
                return false;

            foreach (var cmd in module.Commands)
                _map.RemoveCommand(cmd);

            foreach (var submodule in module.Submodules)
                RemoveModuleInternal(submodule);

            return true;
        }

        //Type Readers
        /// <summary>
        ///     Adds a custom <see cref="TypeReader" /> to this <see cref="CommandService" /> for the supplied object
        ///     type.
        ///     If <typeparamref name="T" /> is a <see cref="ValueType" />, a nullable <see cref="TypeReader" /> will
        ///     also be added.
        ///     If a default <see cref="TypeReader" /> exists for <typeparamref name="T" />, a warning will be logged
        ///     and the default <see cref="TypeReader" /> will be replaced.
        /// </summary>
        /// <typeparam name="T">The object type to be read by the <see cref="TypeReader" />.</typeparam>
        /// <param name="reader">An instance of the <see cref="TypeReader" /> to be added.</param>
        public void AddTypeReader<T>(TypeReader reader)
        {
            AddTypeReader(typeof(T), reader);
        }

        /// <summary>
        ///     Adds a custom <see cref="TypeReader" /> to this <see cref="CommandService" /> for the supplied object
        ///     type.
        ///     If <paramref name="type" /> is a <see cref="ValueType" />, a nullable <see cref="TypeReader" /> for the
        ///     value type will also be added.
        ///     If a default <see cref="TypeReader" /> exists for <paramref name="type" />, a warning will be logged and
        ///     the default <see cref="TypeReader" /> will be replaced.
        /// </summary>
        /// <param name="type">A <see cref="Type" /> instance for the type to be read.</param>
        /// <param name="reader">An instance of the <see cref="TypeReader" /> to be added.</param>
        public void AddTypeReader(Type type, TypeReader reader)
        {
            AddTypeReader(type, reader, true);
        }

        /// <summary>
        ///     Adds a custom <see cref="TypeReader" /> to this <see cref="CommandService" /> for the supplied object
        ///     type.
        ///     If <typeparamref name="T" /> is a <see cref="ValueType" />, a nullable <see cref="TypeReader" /> will
        ///     also be added.
        /// </summary>
        /// <typeparam name="T">The object type to be read by the <see cref="TypeReader" />.</typeparam>
        /// <param name="reader">An instance of the <see cref="TypeReader" /> to be added.</param>
        /// <param name="replaceDefault">
        ///     Defines whether the <see cref="TypeReader" /> should replace the default one for
        ///     <see cref="Type" /> if it exists.
        /// </param>
        public void AddTypeReader<T>(TypeReader reader, bool replaceDefault)
        {
            AddTypeReader(typeof(T), reader, replaceDefault);
        }

        /// <summary>
        ///     Adds a custom <see cref="TypeReader" /> to this <see cref="CommandService" /> for the supplied object
        ///     type.
        ///     If <paramref name="type" /> is a <see cref="ValueType" />, a nullable <see cref="TypeReader" /> for the
        ///     value type will also be added.
        /// </summary>
        /// <param name="type">A <see cref="Type" /> instance for the type to be read.</param>
        /// <param name="reader">An instance of the <see cref="TypeReader" /> to be added.</param>
        /// <param name="replaceDefault">
        ///     Defines whether the <see cref="TypeReader" /> should replace the default one for <see cref="Type" /> if
        ///     it exists.
        /// </param>
        public void AddTypeReader(Type type, TypeReader reader, bool replaceDefault)
        {
            if (replaceDefault && HasDefaultTypeReader(type))
            {
                _defaultTypeReaders.AddOrUpdate(type, reader, (k, v) => reader);

                if (type.GetTypeInfo().IsValueType)
                {
                    var nullableType = typeof(Nullable<>).MakeGenericType(type);
                    var nullableReader = NullableTypeReader.Create(type, reader);
                    _defaultTypeReaders.AddOrUpdate(nullableType, nullableReader, (k, v) => nullableReader);
                }
            }
            else
            {
                var readers = _typeReaders.GetOrAdd(type, x => new ConcurrentDictionary<Type, TypeReader>());
                readers[reader.GetType()] = reader;

                if (type.GetTypeInfo().IsValueType)
                    AddNullableTypeReader(type, reader);
            }
        }

        internal bool HasDefaultTypeReader(Type type)
        {
            if (_defaultTypeReaders.ContainsKey(type))
                return true;

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsEnum)
                return true;

            return false;
        }

        internal void AddNullableTypeReader(Type valueType, TypeReader valueTypeReader)
        {
            var readers = _typeReaders.GetOrAdd(typeof(Nullable<>).MakeGenericType(valueType), x => new ConcurrentDictionary<Type, TypeReader>());
            var nullableReader = NullableTypeReader.Create(valueType, valueTypeReader);
            readers[nullableReader.GetType()] = nullableReader;
        }

        internal IDictionary<Type, TypeReader> GetTypeReaders(Type type)
        {
            if (_typeReaders.TryGetValue(type, out var definedTypeReaders))
                return definedTypeReaders;
            return null;
        }

        internal TypeReader GetDefaultTypeReader(Type type)
        {
            if (_defaultTypeReaders.TryGetValue(type, out var reader))
                return reader;
            var typeInfo = type.GetTypeInfo();

            //Is this an enum?
            if (typeInfo.IsEnum)
            {
                reader = EnumTypeReader.GetReader(type);
                _defaultTypeReaders[type] = reader;
                return reader;
            }

            return null;
        }

        //Execution
        /// <summary>
        ///     Searches for the command.
        /// </summary>
        /// <param name="commandString">The context of the command.</param>
        /// <param name="argPos">The position of which the command starts at.</param>
        /// <returns>The result containing the matching commands.</returns>
        public SearchResult Search(string commandString, int argPos)
        {
            return Search(commandString.Substring(argPos));
        }

        /// <summary>
        ///     Searches for the command.
        /// </summary>
        /// <param name="context">The context of the command.</param>
        /// <param name="input">The command string.</param>
        /// <returns>The result containing the matching commands.</returns>
        public SearchResult Search(string context, string input)
        {
            return Search(input);
        }

        public SearchResult Search(string input)
        {
            var searchInput = _caseSensitive ? input : input.ToLowerInvariant();
            var matches = _map.GetCommands(searchInput).OrderByDescending(x => x.Command.Priority).ToImmutableArray();

            if (matches.Length > 0)
                return SearchResult.FromSuccess(input, matches);
            return SearchResult.FromError(CommandError.UnknownCommand, "Unknown command.");
        }

        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="commandString">The context of the command.</param>
        /// <param name="argPos">The position of which the command starts at.</param>
        /// <param name="services">The service to be used in the command's dependency injection.</param>
        /// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
        /// <returns>
        ///     A task that represents the asynchronous execution operation. The task result contains the result of the
        ///     command execution.
        /// </returns>
        public Task<IResult> ExecuteAsync(string commandString, int argPos, IServiceProvider services, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
        {
            return ExecuteAsync(commandString, commandString.Substring(argPos), services, multiMatchHandling);
        }

        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="commandString">The context of the command.</param>
        /// <param name="input">The command string.</param>
        /// <param name="services">The service to be used in the command's dependency injection.</param>
        /// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
        /// <returns>
        ///     A task that represents the asynchronous execution operation. The task result contains the result of the
        ///     command execution.
        /// </returns>
        public async Task<IResult> ExecuteAsync(string commandString, string input, IServiceProvider services, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
        {
            services = services ?? EmptyServiceProvider.Instance;

            var searchResult = Search(input);
            if (!searchResult.IsSuccess)
                return searchResult;

            var commands = searchResult.Commands;
            var preconditionResults = new Dictionary<CommandMatch, PreconditionResult>();

            foreach (var match in commands)
                preconditionResults[match] = await match.Command.CheckPreconditionsAsync(commandString, services).ConfigureAwait(false);

            var successfulPreconditions = preconditionResults
                .Where(x => x.Value.IsSuccess)
                .ToArray();

            if (successfulPreconditions.Length == 0)
            {
                //All preconditions failed, return the one from the highest priority command
                var bestCandidate = preconditionResults
                    .OrderByDescending(x => x.Key.Command.Priority)
                    .FirstOrDefault(x => !x.Value.IsSuccess);

                return bestCandidate.Value;
            }

            //If we get this far, at least one precondition was successful.

            var parseResultsDict = new Dictionary<CommandMatch, ParseResult>();

            foreach (var pair in successfulPreconditions)
            {
                var parseResult = await pair.Key.ParseAsync(commandString, searchResult, pair.Value, services).ConfigureAwait(false);

                if (parseResult.Error == CommandError.MultipleMatches)
                {
                    IReadOnlyList<TypeReaderValue> argList, paramList;

                    switch (multiMatchHandling)
                    {
                        case MultiMatchHandling.Best:
                            argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                            paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                            parseResult = ParseResult.FromSuccess(argList, paramList);
                            break;
                    }
                }

                parseResultsDict[pair.Key] = parseResult;
            }

            // Calculates the 'score' of a command given a parse result
            float CalculateScore(CommandMatch match, ParseResult parseResult)
            {
                float argValuesScore = 0, paramValuesScore = 0;

                if (match.Command.Parameters.Count > 0)
                {
                    var argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
                    var paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

                    argValuesScore = argValuesSum / match.Command.Parameters.Count;
                    paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
                }

                var totalArgsScore = (argValuesScore + paramValuesScore) / 2;
                return match.Command.Priority + totalArgsScore * 0.99f;
            }

            //Order the parse results by their score so that we choose the most likely result to execute
            var parseResults = parseResultsDict
                .OrderByDescending(x => CalculateScore(x.Key, x.Value));

            var successfulParses = parseResults
                .Where(x => x.Value.IsSuccess)
                .ToArray();

            if (successfulParses.Length == 0)
            {
                //All parses failed, return the one from the highest priority command, using score as a tie breaker
                var bestMatch = parseResults
                    .FirstOrDefault(x => !x.Value.IsSuccess);

                return bestMatch.Value;
            }

            //If we get this far, at least one parse was successful. Execute the most likely overload.
            var chosenOverload = successfulParses[0];
            var result = await chosenOverload.Key.ExecuteAsync(commandString, chosenOverload.Value, services).ConfigureAwait(false);

            return result;
        }
    }
}