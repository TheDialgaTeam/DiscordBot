using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.DependencyInjection;
using TheDialgaTeam.DependencyInjection.ProgramLoop;
using TheDialgaTeam.Discord.Bot.Services.Console;
using TheDialgaTeam.Discord.Bot.Services.Discord;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot
{
    public sealed class Program
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private ProgramLoopManager ProgramLoopManager { get; set; }

        public static void Main()
        {
            var program = new Program();
            program.MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(this);

            serviceCollection.BindInterfacesAndSelfAsSingleton<FramesPerSecondService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<FilePathService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<LoggerService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<SqliteDatabaseService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<DiscordAppService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            ProgramLoopManager = new ProgramLoopManager(ServiceProvider);
            await ProgramLoopManager.StartProgramLoopAsync().ConfigureAwait(false);

            while (true)
            {
                var commandInput = await Console.In.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput.Trim();

                if (commandInput.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    break;
            }

            await ProgramLoopManager.StopProgramLoopAsync();
        }
    }
}