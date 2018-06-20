using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Discord.Bot.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Service.Discord;
using TheDialgaTeam.Discord.Bot.Service.IO;
using TheDialgaTeam.Discord.Bot.Service.Logger;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot
{
    public interface IProgram
    {
        IServiceProvider ServiceProvider { get; }

        CommandService CommandService { get; }
    }

    internal sealed class Program : IProgram
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public CommandService CommandService { get; private set; }

        public static void Main()
        {
            var program = new Program();
            program.MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IProgram>(this);
            serviceCollection.AddSingleton<IFilePathService, FilePathService>();
            serviceCollection.AddSingleton<ILoggerService, LoggerService>();
            serviceCollection.AddSingleton<ISQLiteService, SQLiteService>();
            serviceCollection.AddSingleton<IDiscordAppService, DiscordAppService>();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });
            CommandService.AddTypeReader<IEmote>(new EmoteTypeReader());
            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider).ConfigureAwait(false);

            var loggerService = ServiceProvider.GetRequiredService<ILoggerService>();
            await loggerService.LogMessageAsync("==================================================").ConfigureAwait(false);
            await loggerService.LogMessageAsync("The Dialga Team Discord Bot (.NET Core)").ConfigureAwait(false);
            await loggerService.LogMessageAsync("==================================================").ConfigureAwait(false);
            await loggerService.LogMessageAsync("Please wait while the bot is initializing...\n").ConfigureAwait(false);

            var sqliteService = ServiceProvider.GetRequiredService<ISQLiteService>();
            await sqliteService.InitializeDatabaseAsync().ConfigureAwait(false);

            await loggerService.LogMessageAsync("\nDone initializing!").ConfigureAwait(false);

            var discordAppService = ServiceProvider.GetRequiredService<IDiscordAppService>();
            await discordAppService.StartDiscordAppsAsync();

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}