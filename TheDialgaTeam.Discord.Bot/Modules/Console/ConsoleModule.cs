using System;
using System.Threading.Tasks;
using TheDialgaTeam.Commands;
using TheDialgaTeam.Commands.Attributes;
using TheDialgaTeam.Discord.Bot.Services.Console;

namespace TheDialgaTeam.Discord.Bot.Modules.Console
{
    [Name("Console")]
    public sealed class ConsoleModule : ModuleBase
    {
        private Program Program { get; }

        private LoggerService LoggerService { get; }

        public ConsoleModule(Program program, LoggerService loggerService)
        {
            Program = program;
            LoggerService = loggerService;
        }

        [Command("Exit")]
        public async Task ExitAsync()
        {
            await Program.DependencyInjectionManager.StopProgramLoopAsync().ConfigureAwait(false);
            Environment.Exit(0);
        }

        protected override async Task ReplyAsync(string message = null)
        {
            await LoggerService.LogMessageAsync(message).ConfigureAwait(false);
        }
    }
}