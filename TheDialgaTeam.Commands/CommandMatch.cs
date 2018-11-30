using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheDialgaTeam.Commands.Info;
using TheDialgaTeam.Commands.Results;

namespace TheDialgaTeam.Commands
{
    public struct CommandMatch
    {
        /// <summary> The command that matches the search result. </summary>
        public CommandInfo Command { get; }
        /// <summary> The alias of the command. </summary>
        public string Alias { get; }

        public CommandMatch(CommandInfo command, string alias)
        {
            Command = command;
            Alias = alias;
        }

        public Task<PreconditionResult> CheckPreconditionsAsync(string commandString, IServiceProvider services = null)
            => Command.CheckPreconditionsAsync(commandString, services);
        public Task<ParseResult> ParseAsync(string commandString, SearchResult searchResult, PreconditionResult preconditionResult = null, IServiceProvider services = null)
            => Command.ParseAsync(commandString, Alias.Length, searchResult, preconditionResult, services);
        public Task<IResult> ExecuteAsync(string commandString, IEnumerable<object> argList, IEnumerable<object> paramList, IServiceProvider services)
            => Command.ExecuteAsync(commandString, argList, paramList, services);
        public Task<IResult> ExecuteAsync(string commandString, ParseResult parseResult, IServiceProvider services)
            => Command.ExecuteAsync(commandString, parseResult, services);
    }
}
