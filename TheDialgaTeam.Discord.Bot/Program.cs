using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.DependencyInjection;
using TheDialgaTeam.DependencyInjection.ProgramLoop;
using TheDialgaTeam.Discord.Bot.Models.Discord.Command;
using TheDialgaTeam.Discord.Bot.Models.EntityFramework;
using TheDialgaTeam.Discord.Bot.Services.Console;
using TheDialgaTeam.Discord.Bot.Services.Discord;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;
using TheDialgaTeam.Discord.Bot.Services.IO;
using TheDialgaTeam.Discord.Bot.Services.Nancy;

namespace TheDialgaTeam.Discord.Bot
{
    public sealed class Program
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public CommandService CommandService { get; private set; }

        private ProgramLoopManager ProgramLoopManager { get; set; }

        private LoggerService LoggerService { get; set; }

        private SqliteDatabaseService SqliteDatabaseService { get; set; }

        private DiscordAppService DiscordAppService { get; set; }

        private bool IsRunning { get; set; } = true;

        public static void Main()
        {
            var program = new Program();
            program.MainAsync().GetAwaiter().GetResult();
        }

        private static async Task<string> ReadLineAsync()
        {
            var commandInput = await Console.In.ReadLineAsync().ConfigureAwait(false);
            return commandInput?.Trim();
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
            serviceCollection.BindInterfacesAndSelfAsSingleton<RestWebService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });
            CommandService.AddTypeReader<IEmote>(new EmoteTypeReader());

            await CommandService.AddModulesAsync(Assembly.GetExecutingAssembly(), ServiceProvider).ConfigureAwait(false);

            ProgramLoopManager = new ProgramLoopManager(ServiceProvider);
            await ProgramLoopManager.StartProgramLoopAsync().ConfigureAwait(false);

            LoggerService = ServiceProvider.GetService<LoggerService>();
            SqliteDatabaseService = ServiceProvider.GetService<SqliteDatabaseService>();
            DiscordAppService = ServiceProvider.GetService<DiscordAppService>();

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            do
            {
                var commandInput = await ReadLineAsync().ConfigureAwait(false);

                if (commandInput == null)
                    break;

                if (commandInput.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (commandInput.Equals("Help", StringComparison.OrdinalIgnoreCase))
                    ShowHelpMenu();
                else if (commandInput.Equals("AddDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await AddDiscordAppAsync().ConfigureAwait(false);
                else if (commandInput.Equals("RemoveDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await RemoveDiscordAppAsync().ConfigureAwait(false);
                else if (commandInput.Equals("StartDiscordApps", StringComparison.OrdinalIgnoreCase))
                    await StartDiscordAppsAsync().ConfigureAwait(false);
                else if (commandInput.Equals("StopDiscordApps", StringComparison.OrdinalIgnoreCase))
                    await StopDiscordAppsAsync().ConfigureAwait(false);
                else if (commandInput.Equals("AddGlobalAdmin", StringComparison.OrdinalIgnoreCase))
                    await AddGlobalAdminAsync().ConfigureAwait(false);
                else if (commandInput.Equals("RemoveGlobalAdmin", StringComparison.OrdinalIgnoreCase))
                    await RemoveGlobalAdminAsync().ConfigureAwait(false);
                else if (!string.IsNullOrEmpty(commandInput))
                    LoggerService.LogMessage("Unknown command. Please try again.", ConsoleColor.Red);
            } while (IsRunning);

            await ProgramLoopManager.StopProgramLoopAsync().ConfigureAwait(false);
        }

        private async void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            IsRunning = false;

            await ProgramLoopManager.StopProgramLoopAsync().ConfigureAwait(false);
            Environment.Exit(0);
        }

        private void ShowHelpMenu()
        {
            var writeToConsole = new List<string>
            {
                "==================================================",
                "Command available:",
                "==================================================",
                "Exit - Close the application.",
                "Help - Show the help menu.",
                "AddDiscordApp - Add a new discord app into the database.",
                "RemoveDiscordApp - Remove a discord app from the database.",
                "StartDiscordApps - Start all discord app.",
                "StopDiscordApps - Start all discord app.",
                "AddGlobalAdmin - Add a user as a global admin.",
                "RemoveGlobalAdmin - Remove a user as a global admin.",
                "=================================================="
            };

            foreach (var message in writeToConsole)
                LoggerService.LogMessage(message);
        }

        private async Task AddDiscordAppAsync()
        {
            ulong clientId;
            string clientSecret = null, botToken = null;

            do
            {
                LoggerService.LogMessage("Enter the Client ID of the Discord App you wish to add:");

                var commandInput = await ReadLineAsync().ConfigureAwait(false);

                if (commandInput == null)
                    return;

                if (!ulong.TryParse(commandInput, out clientId))
                    LoggerService.LogMessage("Invalid ID, try again!", ConsoleColor.Red);
            } while (clientId == 0);

            do
            {
                LoggerService.LogMessage("Enter the Client Secret of the Discord App you wish to add:");

                var commandInput = await ReadLineAsync().ConfigureAwait(false);

                if (commandInput == null)
                    return;

                if (string.IsNullOrEmpty(commandInput))
                    LoggerService.LogMessage("Invalid Client Secret key, try again!", ConsoleColor.Red);
                else
                    clientSecret = commandInput;
            } while (string.IsNullOrEmpty(clientSecret));

            do
            {
                LoggerService.LogMessage("Enter the Bot Token of the Discord App you wish to add:");

                var commandInput = await ReadLineAsync().ConfigureAwait(false);

                if (commandInput == null)
                    return;

                if (string.IsNullOrEmpty(commandInput))
                    LoggerService.LogMessage("Invalid Bot Token key, try again!", ConsoleColor.Red);
                else
                    botToken = commandInput;
            } while (string.IsNullOrEmpty(botToken));

            while (true)
            {
                LoggerService.LogMessage("Do you wish to add this Discord App with the following information:");
                LoggerService.LogMessage($"Client ID: {clientId}");
                LoggerService.LogMessage($"Client Secret: {clientSecret}");
                LoggerService.LogMessage($"Bot Token: {botToken}");
                LoggerService.LogMessage("Y/n?");

                var commandInput = await ReadLineAsync().ConfigureAwait(false);

                if (commandInput == null)
                    return;

                if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    using (var context = SqliteDatabaseService.GetContext())
                    {
                        var discordApp = await context.DiscordAppTable.Where(a => a.ClientId == clientId).FirstOrDefaultAsync().ConfigureAwait(false);

                        if (discordApp == null)
                        {
                            discordApp = new DiscordAppTable { ClientId = clientId, ClientSecret = clientSecret, BotToken = botToken };

                            context.DiscordAppTable.Add(discordApp);
                        }
                        else
                        {
                            discordApp.ClientSecret = clientSecret;
                            discordApp.BotToken = botToken;

                            context.DiscordAppTable.Update(discordApp);
                        }

                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }

                    LoggerService.LogMessage("Discord App have been added.", ConsoleColor.Green);
                    break;
                }

                if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    LoggerService.LogMessage("Discord App will not be added.", ConsoleColor.Red);
                    break;
                }

                LoggerService.LogMessage("Invalid input, try again!", ConsoleColor.Red);
            }
        }

        private async Task RemoveDiscordAppAsync()
        {
            using (var context = SqliteDatabaseService.GetContext())
            {
                ulong clientId;
                DiscordAppTable discordApp = null;

                do
                {
                    LoggerService.LogMessage("Enter the Client ID of the Discord App you wish to remove:");

                    var commandInput = await ReadLineAsync().ConfigureAwait(false);

                    if (commandInput == null)
                        return;

                    if (!ulong.TryParse(commandInput, out clientId))
                        LoggerService.LogMessage("Invalid ID, try again!", ConsoleColor.Red);
                    else
                    {
                        var id = clientId;
                        discordApp = await context.DiscordAppTable.Where(a => a.ClientId == id).FirstOrDefaultAsync().ConfigureAwait(false);

                        if (discordApp != null)
                            continue;

                        LoggerService.LogMessage("Could not find any Discord App with this ID!", ConsoleColor.Red);
                        return;
                    }
                } while (clientId == 0);

                while (true)
                {
                    LoggerService.LogMessage("Do you wish to remove this Discord App with the following information:");
                    LoggerService.LogMessage($"App Name: {discordApp?.AppName}");
                    LoggerService.LogMessage($"Client ID: {clientId}");
                    LoggerService.LogMessage("Y/n?");

                    var commandInput = await ReadLineAsync().ConfigureAwait(false);

                    if (commandInput == null)
                        return;

                    if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        if (discordApp != null)
                        {
                            await DiscordAppService.StopDiscordAppAsync(clientId).ConfigureAwait(false);

                            context.DiscordAppTable.Remove(discordApp);
                            await context.SaveChangesAsync().ConfigureAwait(false);
                        }

                        LoggerService.LogMessage("Discord App have been deleted.", ConsoleColor.Green);
                        break;
                    }

                    if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
                    {
                        LoggerService.LogMessage("Discord App will not be deleted.", ConsoleColor.Red);
                        break;
                    }

                    LoggerService.LogMessage("Invalid input, try again!", ConsoleColor.Red);
                }
            }
        }

        private async Task StartDiscordAppsAsync()
        {
            using (var context = SqliteDatabaseService.GetContext(true))
            {
                await context.DiscordAppTable.ForEachAsync(async a =>
                {
                    var result = await DiscordAppService.StartDiscordAppAsync(Convert.ToUInt64(a.ClientId)).ConfigureAwait(false);
                    LoggerService.LogMessage(result.BuildDiscordTextResponse(), ConsoleColor.Green);
                }).ConfigureAwait(false);
            }

            LoggerService.LogMessage("All discord apps have been started.", ConsoleColor.Green);
        }

        private async Task StopDiscordAppsAsync()
        {
            using (var context = SqliteDatabaseService.GetContext(true))
            {
                await context.DiscordAppTable.ForEachAsync(async a =>
                {
                    var result = await DiscordAppService.StopDiscordAppAsync(Convert.ToUInt64(a.ClientId)).ConfigureAwait(false);
                    LoggerService.LogMessage(result.BuildDiscordTextResponse(), ConsoleColor.Green);
                }).ConfigureAwait(false);
            }

            LoggerService.LogMessage("All discord apps have been stopped.", ConsoleColor.Green);
        }

        private async Task AddGlobalAdminAsync()
        {
            ulong userId;

            do
            {
                LoggerService.LogMessage("Enter the User ID that you wish to add as a global admin:");

                var commandInput = await ReadLineAsync().ConfigureAwait(false);

                if (commandInput == null)
                    return;

                if (!ulong.TryParse(commandInput, out userId))
                    LoggerService.LogMessage("Invalid ID, try again!", ConsoleColor.Red);
            } while (userId == 0);

            while (true)
            {
                LoggerService.LogMessage("Do you wish to add this user as a global admin with the following information:");
                LoggerService.LogMessage($"User ID: {userId}");
                LoggerService.LogMessage("Y/n?");

                var commandInput = await ReadLineAsync().ConfigureAwait(false);

                if (commandInput == null)
                    return;

                if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    using (var context = SqliteDatabaseService.GetContext())
                    {
                        var discordAppOwner = await context.DiscordAppOwnerTable.Where(a => a.DiscordAppId == null && a.UserId == userId).FirstOrDefaultAsync().ConfigureAwait(false);

                        if (discordAppOwner == null)
                        {
                            discordAppOwner = new DiscordAppOwnerTable { UserId = userId };

                            context.DiscordAppOwnerTable.Add(discordAppOwner);
                            await context.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }

                    LoggerService.LogMessage("User is now a global admin.", ConsoleColor.Green);
                    break;
                }

                if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    LoggerService.LogMessage("User will not be made global admin.", ConsoleColor.Red);
                    break;
                }

                LoggerService.LogMessage("Invalid input, try again!", ConsoleColor.Red);
            }
        }

        private async Task RemoveGlobalAdminAsync()
        {
            using (var context = SqliteDatabaseService.GetContext())
            {
                ulong userId;
                DiscordAppOwnerTable discordAppOwner = null;

                do
                {
                    LoggerService.LogMessage("Enter the User ID that you wish to remove as a global admin:");

                    var commandInput = await ReadLineAsync().ConfigureAwait(false);

                    if (commandInput == null)
                        return;

                    if (!ulong.TryParse(commandInput, out userId))
                        LoggerService.LogMessage("Invalid ID, try again!", ConsoleColor.Red);
                    else
                    {
                        var id = userId;
                        discordAppOwner = await context.DiscordAppOwnerTable.Where(a => a.UserId == id).FirstOrDefaultAsync().ConfigureAwait(false);

                        if (discordAppOwner != null)
                            continue;

                        LoggerService.LogMessage("Could not find any global admin with this ID!", ConsoleColor.Red);
                        return;
                    }
                } while (userId == 0);

                while (true)
                {
                    LoggerService.LogMessage("Do you wish to remove this user as a global admin with the following information:");
                    LoggerService.LogMessage($"User ID: {userId}");
                    LoggerService.LogMessage("Y/n?");

                    var commandInput = await ReadLineAsync().ConfigureAwait(false);

                    if (commandInput == null)
                        return;

                    if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        if (discordAppOwner != null)
                        {
                            context.DiscordAppOwnerTable.Remove(discordAppOwner);
                            await context.SaveChangesAsync().ConfigureAwait(false);
                        }

                        LoggerService.LogMessage("User is now not a global admin.", ConsoleColor.Green);
                        break;
                    }

                    if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
                    {
                        LoggerService.LogMessage("User will remain as a global admin.", ConsoleColor.Red);
                        break;
                    }

                    LoggerService.LogMessage("Invalid input, try again!", ConsoleColor.Red);
                }
            }
        }
    }
}