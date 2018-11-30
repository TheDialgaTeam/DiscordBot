using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheDialgaTeam.Commands.Info;

namespace TheDialgaTeam.Commands.Extensions
{
    public static class CommandServiceExtensions
    {
        public static async Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this ICollection<CommandInfo> commands, string commandString, IServiceProvider provider)
        {
            var executableCommands = new List<CommandInfo>();

            var tasks = commands.Select(async c =>
            {
                var result = await c.CheckPreconditionsAsync(commandString, provider).ConfigureAwait(false);
                return new { Command = c, PreconditionResult = result };
            });

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            foreach (var result in results)
            {
                if (result.PreconditionResult.IsSuccess)
                    executableCommands.Add(result.Command);
            }

            return executableCommands;
        }
        public static Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this CommandService commandService, string commandString, IServiceProvider provider)
            => GetExecutableCommandsAsync(commandService.Commands.ToArray(), commandString, provider);
        public static async Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this ModuleInfo module, string commandString, IServiceProvider provider)
        {
            var executableCommands = new List<CommandInfo>();

            executableCommands.AddRange(await module.Commands.ToArray().GetExecutableCommandsAsync(commandString, provider).ConfigureAwait(false));

            var tasks = module.Submodules.Select(async s => await s.GetExecutableCommandsAsync(commandString, provider).ConfigureAwait(false));
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            executableCommands.AddRange(results.SelectMany(c => c));

            return executableCommands;
        }
    }
}
