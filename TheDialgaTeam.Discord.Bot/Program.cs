using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Discord.Bot.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Service.Discord;
using TheDialgaTeam.Discord.Bot.Service.IO;
using TheDialgaTeam.Discord.Bot.Service.Logger;
using TheDialgaTeam.Discord.Bot.Service.Nancy;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot
{
    public sealed class Program
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public CommandService CommandService { get; private set; }

        private LoggerService LoggerService { get; set; }

        private SQLiteService SQLiteService { get; set; }

        private DiscordAppService DiscordAppService { get; set; }

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
            serviceCollection.AddSingleton<RestWebService>();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });
            CommandService.AddTypeReader<IEmote>(new EmoteTypeReader());
            await CommandService.AddModulesAsync(Assembly.GetExecutingAssembly(), ServiceProvider).ConfigureAwait(false);

            Console.Title = "The Dialga Team Discord Bot (.Net Core)";

            LoggerService = ServiceProvider.GetRequiredService<LoggerService>();
            await LoggerService.LogMessageAsync("==================================================").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("The Dialga Team Discord Bot (.NET Core)").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("==================================================").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("Please wait while the bot is initializing...\n").ConfigureAwait(false);

            SQLiteService = ServiceProvider.GetRequiredService<SQLiteService>();
            await SQLiteService.InitializeDatabaseAsync().ConfigureAwait(false);

            await LoggerService.LogMessageAsync("\nDone initializing!").ConfigureAwait(false);

            DiscordAppService = ServiceProvider.GetRequiredService<DiscordAppService>();

            var restWebService = ServiceProvider.GetRequiredService<RestWebService>();
            await restWebService.StartAsync();

            while (true)
            {
                var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput.Trim();

                if (commandInput.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (commandInput.Equals("Help", StringComparison.OrdinalIgnoreCase))
                    await ShowHelpMenuAsync().ConfigureAwait(false);
                else if (commandInput.Equals("AddDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await AddDiscordAppAsync().ConfigureAwait(false);
                else if (commandInput.Equals("RemoveDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await RemoveDiscordAppAsync().ConfigureAwait(false);
                else if (commandInput.Equals("StartDiscordApps", StringComparison.OrdinalIgnoreCase))
                    await StartDiscordAppsAsync().ConfigureAwait(false);
                else if (commandInput.Equals("StopDiscordApps", StringComparison.OrdinalIgnoreCase))
                    await StopDiscordAppsAsync().ConfigureAwait(false);
                else if (!string.IsNullOrEmpty(commandInput))
                    await LoggerService.LogMessageAsync("Unknown command. Please try again.", ConsoleColor.Red).ConfigureAwait(false);
            }

            var discordApps = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);

            foreach (var discordAppTable in discordApps)
                await DiscordAppService.StopDiscordAppAsync(Convert.ToUInt64(discordAppTable.ClientId)).ConfigureAwait(false);

            await restWebService.StopAsync();
        }

        private async Task ShowHelpMenuAsync()
        {
            await LoggerService.LogMessageAsync("==================================================").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("Command available:").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("==================================================").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("Exit - Close the application.").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("Help - Show the help menu.").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("AddDiscordApp - Add a new discord app into the database.").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("RemoveDiscordApp - Remove a discord app from the database.").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("StartDiscordApps - Start all discord app.").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("StopDiscordApps - Start all discord app.").ConfigureAwait(false);
            await LoggerService.LogMessageAsync("==================================================").ConfigureAwait(false);
        }

        private async Task AddDiscordAppAsync()
        {
            ulong clientId;
            string clientSecret = null, botToken = null;

            do
            {
                await LoggerService.LogMessageAsync("Enter the Client ID of the Discord App you wish to add:").ConfigureAwait(false);
                var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput.Trim();

                if (!ulong.TryParse(commandInput, out clientId))
                    await LoggerService.LogMessageAsync("Invalid ID, try again!", ConsoleColor.Red).ConfigureAwait(false);
            } while (clientId == 0);

            do
            {
                await LoggerService.LogMessageAsync("Enter the Client Secret of the Discord App you wish to add:").ConfigureAwait(false);
                var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput.Trim();

                if (string.IsNullOrEmpty(commandInput))
                    await LoggerService.LogMessageAsync("Invalid Client Secret key, try again!", ConsoleColor.Red).ConfigureAwait(false);
                else
                    clientSecret = commandInput;
            } while (string.IsNullOrEmpty(clientSecret));

            do
            {
                await LoggerService.LogMessageAsync("Enter the Bot Token of the Discord App you wish to add:").ConfigureAwait(false);
                var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput.Trim();

                if (string.IsNullOrEmpty(commandInput))
                    await LoggerService.LogMessageAsync("Invalid Bot Token key, try again!", ConsoleColor.Red).ConfigureAwait(false);
                else
                    botToken = commandInput;
            } while (string.IsNullOrEmpty(botToken));

            while (true)
            {
                await LoggerService.LogMessageAsync("Do you wish to add this Discord App with the following information:").ConfigureAwait(false);
                await LoggerService.LogMessageAsync($"Client ID: {clientId}").ConfigureAwait(false);
                await LoggerService.LogMessageAsync($"Client Secret: {clientSecret}").ConfigureAwait(false);
                await LoggerService.LogMessageAsync($"Bot Token: {botToken}").ConfigureAwait(false);
                await LoggerService.LogMessageAsync("Y/n?").ConfigureAwait(false);

                var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput.Trim();

                if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    var discordApp = new DiscordAppTable { ClientId = clientId.ToString(), ClientSecret = clientSecret, BotToken = botToken };
                    await SQLiteService.SQLiteAsyncConnection.InsertOrReplaceAsync(discordApp).ConfigureAwait(false);
                    await LoggerService.LogMessageAsync("Discord App have been added.", ConsoleColor.Green);
                    break;
                }

                if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    await LoggerService.LogMessageAsync("Discord App will not be added.", ConsoleColor.Red);
                    break;
                }

                await LoggerService.LogMessageAsync("Invalid input, try again!", ConsoleColor.Red);
            }
        }

        private async Task RemoveDiscordAppAsync()
        {
            ulong clientId;
            DiscordAppTable discordApp = null;

            do
            {
                await LoggerService.LogMessageAsync("Enter the Client ID of the Discord App you wish to remove:").ConfigureAwait(false);
                var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput.Trim();

                if (!ulong.TryParse(commandInput, out clientId))
                    await LoggerService.LogMessageAsync("Invalid ID, try again!", ConsoleColor.Red).ConfigureAwait(false);
                else
                {
                    var clientIdString = clientId.ToString();
                    discordApp = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (discordApp != null)
                        continue;

                    await LoggerService.LogMessageAsync("Could not find any Discord App with this ID!", ConsoleColor.Red).ConfigureAwait(false);
                    return;
                }
            } while (clientId == 0);

            while (true)
            {
                await LoggerService.LogMessageAsync("Do you wish to remove this Discord App with the following information:").ConfigureAwait(false);
                await LoggerService.LogMessageAsync($"App Name: {discordApp?.AppName}").ConfigureAwait(false);
                await LoggerService.LogMessageAsync($"Client ID: {clientId}").ConfigureAwait(false);
                await LoggerService.LogMessageAsync("Y/n?").ConfigureAwait(false);

                var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput.Trim();

                if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    await SQLiteService.SQLiteAsyncConnection.DeleteAsync(discordApp).ConfigureAwait(false);
                    await LoggerService.LogMessageAsync("Discord App have been deleted.", ConsoleColor.Green);
                    break;
                }

                if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    await LoggerService.LogMessageAsync("Discord App will not be deleted.", ConsoleColor.Red);
                    break;
                }

                await LoggerService.LogMessageAsync("Invalid input, try again!", ConsoleColor.Red);
            }
        }

        private async Task StartDiscordAppsAsync()
        {
            var discordApps = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);

            foreach (var discordAppTable in discordApps)
            {
                var result = await DiscordAppService.StartDiscordAppAsync(Convert.ToUInt64(discordAppTable.ClientId)).ConfigureAwait(false);
                await LoggerService.LogMessageAsync(result.BuildDiscordTextResponse(), ConsoleColor.Green).ConfigureAwait(false);
            }

            await LoggerService.LogMessageAsync("All discord apps have been started.", ConsoleColor.Green).ConfigureAwait(false);
        }

        private async Task StopDiscordAppsAsync()
        {
            var discordApps = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);

            foreach (var discordAppTable in discordApps)
            {
                var result = await DiscordAppService.StopDiscordAppAsync(Convert.ToUInt64(discordAppTable.ClientId)).ConfigureAwait(false);
                await LoggerService.LogMessageAsync(result.BuildDiscordTextResponse(), ConsoleColor.Green).ConfigureAwait(false);
            }

            await LoggerService.LogMessageAsync("All discord apps have been stopped.", ConsoleColor.Green).ConfigureAwait(false);
        }
    }
}