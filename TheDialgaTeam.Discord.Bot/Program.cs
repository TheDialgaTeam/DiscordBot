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
    internal sealed class Program
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
            serviceCollection.AddSingleton(this);
            serviceCollection.AddSingleton<FilePathService>();
            serviceCollection.AddSingleton<LoggerService>();
            serviceCollection.AddSingleton<SQLiteService>();
            serviceCollection.AddSingleton<DiscordAppService>();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });
            CommandService.AddTypeReader<IEmote>(new EmoteTypeReader());
            await CommandService.AddModulesAsync(Assembly.GetExecutingAssembly(), ServiceProvider).ConfigureAwait(false);

            var loggerService = ServiceProvider.GetRequiredService<LoggerService>();
            await loggerService.LogMessageAsync("==================================================").ConfigureAwait(false);
            await loggerService.LogMessageAsync("The Dialga Team Discord Bot (.NET Core)").ConfigureAwait(false);
            await loggerService.LogMessageAsync("==================================================").ConfigureAwait(false);
            await loggerService.LogMessageAsync("Please wait while the bot is initializing...\n").ConfigureAwait(false);

            var sqliteService = ServiceProvider.GetRequiredService<SQLiteService>();
            await sqliteService.InitializeDatabaseAsync().ConfigureAwait(false);

            await loggerService.LogMessageAsync("\nDone initializing!").ConfigureAwait(false);

            var discordAppService = ServiceProvider.GetRequiredService<DiscordAppService>();
            await discordAppService.StartDiscordAppAsync(434237369088999427);

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}