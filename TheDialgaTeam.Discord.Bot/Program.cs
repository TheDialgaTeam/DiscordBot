using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.DependencyInjection;
using TheDialgaTeam.Discord.Bot.Models.Discord.Command;
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

        public DependencyInjectionManager DependencyInjectionManager { get; private set; }

        private Commands.CommandService ConsoleCommandService { get; set; }

        private LoggerService LoggerService { get; set; }

        private bool IsRunning { get; set; } = true;

        public static void Main()
        {
            var program = new Program();
            program.MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            Console.Title = "The Dialga Team Discord Bot (.Net Core)";

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(this);
            serviceCollection.BindInterfacesAndSelfAsSingleton<FilePathService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<LoggerService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<SqliteDatabaseService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<DiscordAppService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<RestWebService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });
            CommandService.AddTypeReader<IEmote>(new EmoteTypeReader());
            await CommandService.AddModulesAsync(Assembly.GetExecutingAssembly(), ServiceProvider).ConfigureAwait(false);

            ConsoleCommandService = new Commands.CommandService(new Commands.CommandServiceConfig { CaseSensitiveCommands = false });
            await ConsoleCommandService.AddModulesAsync(Assembly.GetExecutingAssembly(), ServiceProvider).ConfigureAwait(false);

            DependencyInjectionManager = new DependencyInjectionManager(ServiceProvider);
            await DependencyInjectionManager.StartProgramLoopAsync().ConfigureAwait(false);

            LoggerService = ServiceProvider.GetService<LoggerService>();

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            do
            {
                var commandInput = await Console.In.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput?.Trim();

                if (commandInput == null)
                    break;

                var result = await ConsoleCommandService.ExecuteAsync(commandInput, 0, ServiceProvider).ConfigureAwait(false);

                if (!result.IsSuccess)
                    await LoggerService.LogMessageAsync("Unknown command. Please try again.", ConsoleColor.Red).ConfigureAwait(false);
            } while (IsRunning);

            await DependencyInjectionManager.StopProgramLoopAsync().ConfigureAwait(false);
        }

        private async void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            IsRunning = false;

            await DependencyInjectionManager.StopProgramLoopAsync().ConfigureAwait(false);
            Environment.Exit(0);
        }

        //private void ShowHelpMenu()
        //{
        //    var writeToConsole = new List<string>
        //    {
        //        "==================================================",
        //        "Command available:",
        //        "==================================================",
        //        "Exit - Close the application.",
        //        "Help - Show the help menu.",
        //        "AddDiscordApp - Add a new discord app into the database.",
        //        "RemoveDiscordApp - Remove a discord app from the database.",
        //        "StartDiscordApps - Start all discord app.",
        //        "StopDiscordApps - Start all discord app.",
        //        "AddGlobalAdmin - Add a user as a global admin.",
        //        "RemoveGlobalAdmin - Remove a user as a global admin.",
        //        "=================================================="
        //    };

        //    foreach (var message in writeToConsole)
        //        LoggerService.LogMessageAsync(message);
        //}

        //private async Task AddDiscordAppAsync()
        //{
        //    ulong clientId;
        //    string clientSecret = null, botToken = null;

        //    do
        //    {
        //        LoggerService.LogMessageAsync("Enter the Client ID of the Discord App you wish to add:");

        //        var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //        if (commandInput == null)
        //            return;

        //        if (!ulong.TryParse(commandInput, out clientId))
        //            LoggerService.LogMessageAsync("Invalid ID, try again!", ConsoleColor.Red);
        //    } while (clientId == 0);

        //    do
        //    {
        //        LoggerService.LogMessageAsync("Enter the Client Secret of the Discord App you wish to add:");

        //        var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //        if (commandInput == null)
        //            return;

        //        if (string.IsNullOrEmpty(commandInput))
        //            LoggerService.LogMessageAsync("Invalid Client Secret key, try again!", ConsoleColor.Red);
        //        else
        //            clientSecret = commandInput;
        //    } while (string.IsNullOrEmpty(clientSecret));

        //    do
        //    {
        //        LoggerService.LogMessageAsync("Enter the Bot Token of the Discord App you wish to add:");

        //        var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //        if (commandInput == null)
        //            return;

        //        if (string.IsNullOrEmpty(commandInput))
        //            LoggerService.LogMessageAsync("Invalid Bot Token key, try again!", ConsoleColor.Red);
        //        else
        //            botToken = commandInput;
        //    } while (string.IsNullOrEmpty(botToken));

        //    while (true)
        //    {
        //        LoggerService.LogMessageAsync("Do you wish to add this Discord App with the following information:");
        //        LoggerService.LogMessageAsync($"Client ID: {clientId}");
        //        LoggerService.LogMessageAsync($"Client Secret: {clientSecret}");
        //        LoggerService.LogMessageAsync($"Bot Token: {botToken}");
        //        LoggerService.LogMessageAsync("Y/n?");

        //        var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //        if (commandInput == null)
        //            return;

        //        if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
        //        {
        //            using (var context = SqliteDatabaseService.GetContext())
        //            {
        //                var discordApp = await context.GetDiscordAppTableAsync(clientId).ConfigureAwait(false);

        //                if (discordApp == null)
        //                {
        //                    discordApp = new DiscordApp { ClientId = clientId, ClientSecret = clientSecret, BotToken = botToken };

        //                    context.DiscordAppTable.Add(discordApp);
        //                }
        //                else
        //                {
        //                    discordApp.ClientSecret = clientSecret;
        //                    discordApp.BotToken = botToken;

        //                    context.DiscordAppTable.Update(discordApp);
        //                }

        //                await context.SaveChangesAsync().ConfigureAwait(false);
        //            }

        //            LoggerService.LogMessageAsync("Discord App have been added.", ConsoleColor.Green);
        //            break;
        //        }

        //        if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
        //        {
        //            LoggerService.LogMessageAsync("Discord App will not be added.", ConsoleColor.Red);
        //            break;
        //        }

        //        LoggerService.LogMessageAsync("Invalid input, try again!", ConsoleColor.Red);
        //    }
        //}

        //private async Task RemoveDiscordAppAsync()
        //{
        //    using (var context = SqliteDatabaseService.GetContext())
        //    {
        //        ulong clientId;
        //        DiscordApp discordApp = null;

        //        do
        //        {
        //            LoggerService.LogMessageAsync("Enter the Client ID of the Discord App you wish to remove:");

        //            var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //            if (commandInput == null)
        //                return;

        //            if (!ulong.TryParse(commandInput, out clientId))
        //                LoggerService.LogMessageAsync("Invalid ID, try again!", ConsoleColor.Red);
        //            else
        //            {
        //                discordApp = await context.GetDiscordAppTableAsync(clientId).ConfigureAwait(false);

        //                if (discordApp != null)
        //                    continue;

        //                LoggerService.LogMessageAsync("Could not find any Discord App with this ID!", ConsoleColor.Red);
        //                return;
        //            }
        //        } while (clientId == 0);

        //        while (true)
        //        {
        //            LoggerService.LogMessageAsync("Do you wish to remove this Discord App with the following information:");
        //            LoggerService.LogMessageAsync($"App Name: {discordApp?.AppName}");
        //            LoggerService.LogMessageAsync($"Client ID: {clientId}");
        //            LoggerService.LogMessageAsync("Y/n?");

        //            var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //            if (commandInput == null)
        //                return;

        //            if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
        //            {
        //                if (discordApp != null)
        //                {
        //                    await DiscordAppService.StopDiscordAppAsync(clientId).ConfigureAwait(false);

        //                    context.DiscordAppTable.Remove(discordApp);
        //                    await context.SaveChangesAsync().ConfigureAwait(false);
        //                }

        //                LoggerService.LogMessageAsync("Discord App have been deleted.", ConsoleColor.Green);
        //                break;
        //            }

        //            if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
        //            {
        //                LoggerService.LogMessageAsync("Discord App will not be deleted.", ConsoleColor.Red);
        //                break;
        //            }

        //            LoggerService.LogMessageAsync("Invalid input, try again!", ConsoleColor.Red);
        //        }
        //    }
        //}

        //private async Task StartDiscordAppsAsync()
        //{
        //    using (var context = SqliteDatabaseService.GetContext(true))
        //    {
        //        await context.DiscordAppTable.ForEachAsync(async a =>
        //        {
        //            var result = await DiscordAppService.StartDiscordAppAsync(a.ClientId).ConfigureAwait(false);
        //            LoggerService.LogMessageAsync(result.BuildDiscordTextResponse(), ConsoleColor.Green);
        //        }).ConfigureAwait(false);
        //    }

        //    LoggerService.LogMessageAsync("All discord apps have been started.", ConsoleColor.Green);
        //}

        //private async Task StopDiscordAppsAsync()
        //{
        //    using (var context = SqliteDatabaseService.GetContext(true))
        //    {
        //        await context.DiscordAppTable.ForEachAsync(async a =>
        //        {
        //            var result = await DiscordAppService.StopDiscordAppAsync(a.ClientId).ConfigureAwait(false);
        //            LoggerService.LogMessageAsync(result.BuildDiscordTextResponse(), ConsoleColor.Green);
        //        }).ConfigureAwait(false);
        //    }

        //    LoggerService.LogMessageAsync("All discord apps have been stopped.", ConsoleColor.Green);
        //}

        //private async Task AddGlobalAdminAsync()
        //{
        //    ulong userId;

        //    do
        //    {
        //        LoggerService.LogMessageAsync("Enter the User ID that you wish to add as a global admin:");

        //        var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //        if (commandInput == null)
        //            return;

        //        if (!ulong.TryParse(commandInput, out userId))
        //            LoggerService.LogMessageAsync("Invalid ID, try again!", ConsoleColor.Red);
        //    } while (userId == 0);

        //    while (true)
        //    {
        //        LoggerService.LogMessageAsync("Do you wish to add this user as a global admin with the following information:");
        //        LoggerService.LogMessageAsync($"User ID: {userId}");
        //        LoggerService.LogMessageAsync("Y/n?");

        //        var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //        if (commandInput == null)
        //            return;

        //        if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
        //        {
        //            using (var context = SqliteDatabaseService.GetContext())
        //            {
        //                var discordAppOwner = await context.DiscordAppOwnerTable.Where(a => a.DiscordAppId == null && a.UserId == userId).FirstOrDefaultAsync().ConfigureAwait(false);

        //                if (discordAppOwner == null)
        //                {
        //                    discordAppOwner = new DiscordAppOwner { UserId = userId };

        //                    context.DiscordAppOwnerTable.Add(discordAppOwner);
        //                    await context.SaveChangesAsync().ConfigureAwait(false);
        //                }
        //            }

        //            LoggerService.LogMessageAsync("User is now a global admin.", ConsoleColor.Green);
        //            break;
        //        }

        //        if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
        //        {
        //            LoggerService.LogMessageAsync("User will not be made global admin.", ConsoleColor.Red);
        //            break;
        //        }

        //        LoggerService.LogMessageAsync("Invalid input, try again!", ConsoleColor.Red);
        //    }
        //}

        //private async Task RemoveGlobalAdminAsync()
        //{
        //    using (var context = SqliteDatabaseService.GetContext())
        //    {
        //        ulong userId;
        //        DiscordAppOwner discordAppOwner = null;

        //        do
        //        {
        //            LoggerService.LogMessageAsync("Enter the User ID that you wish to remove as a global admin:");

        //            var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //            if (commandInput == null)
        //                return;

        //            if (!ulong.TryParse(commandInput, out userId))
        //                LoggerService.LogMessageAsync("Invalid ID, try again!", ConsoleColor.Red);
        //            else
        //            {
        //                var id = userId;
        //                discordAppOwner = await context.DiscordAppOwnerTable.Where(a => a.UserId == id).FirstOrDefaultAsync().ConfigureAwait(false);

        //                if (discordAppOwner != null)
        //                    continue;

        //                LoggerService.LogMessageAsync("Could not find any global admin with this ID!", ConsoleColor.Red);
        //                return;
        //            }
        //        } while (userId == 0);

        //        while (true)
        //        {
        //            LoggerService.LogMessageAsync("Do you wish to remove this user as a global admin with the following information:");
        //            LoggerService.LogMessageAsync($"User ID: {userId}");
        //            LoggerService.LogMessageAsync("Y/n?");

        //            var commandInput = await ReadLineAsync().ConfigureAwait(false);

        //            if (commandInput == null)
        //                return;

        //            if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
        //            {
        //                if (discordAppOwner != null)
        //                {
        //                    context.DiscordAppOwnerTable.Remove(discordAppOwner);
        //                    await context.SaveChangesAsync().ConfigureAwait(false);
        //                }

        //                LoggerService.LogMessageAsync("User is now not a global admin.", ConsoleColor.Green);
        //                break;
        //            }

        //            if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
        //            {
        //                LoggerService.LogMessageAsync("User will remain as a global admin.", ConsoleColor.Red);
        //                break;
        //            }

        //            LoggerService.LogMessageAsync("Invalid input, try again!", ConsoleColor.Red);
        //        }
        //    }
        //}
    }
}