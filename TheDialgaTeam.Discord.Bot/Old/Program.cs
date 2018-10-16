using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.DependencyInjection.ProgramLoop;
using TheDialgaTeam.Discord.Bot.Old.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Old.Service.Discord;
using TheDialgaTeam.Discord.Bot.Old.Service.Nancy;
using TheDialgaTeam.Discord.Bot.Old.Service.SQLite;
using TheDialgaTeam.Discord.Bot.Services.Console;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Old
{
    public sealed class Program
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public CommandService CommandService { get; private set; }

        private LoggerService LoggerService { get; set; }

        private SQLiteService SQLiteService { get; set; }

        private DiscordAppService DiscordAppService { get; set; }

        private RestWebService RestWebService { get; set; }

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

            serviceCollection.AddSingleton<FilePathService>();
            serviceCollection.AddSingleton<IInitializable>(x => x.GetService<FilePathService>());

            serviceCollection.AddSingleton<LoggerService>();
            serviceCollection.AddSingleton<IInitializable>(x => x.GetService<LoggerService>());
            serviceCollection.AddSingleton<ITickableAsync>(x => x.GetService<LoggerService>());
            serviceCollection.AddSingleton<IDisposableAsync>(x => x.GetService<LoggerService>());
            serviceCollection.AddSingleton<IErrorLogger>(x => x.GetService<LoggerService>());

            serviceCollection.AddSingleton<SQLiteService>();
            serviceCollection.AddSingleton<IInitializableAsync>(x => x.GetService<SQLiteService>());

            serviceCollection.AddSingleton<DiscordAppService>();
            serviceCollection.AddSingleton<IInitializableAsync>(x => x.GetService<DiscordAppService>());
            serviceCollection.AddSingleton<ITickableAsync>(x => x.GetService<DiscordAppService>());
            serviceCollection.AddSingleton<IDisposableAsync>(x => x.GetService<DiscordAppService>());

            serviceCollection.AddSingleton<RestWebService>();
            serviceCollection.AddSingleton<IDisposableAsync>(x => x.GetService<RestWebService>());

            ServiceProvider = serviceCollection.BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });

            CommandService.AddTypeReader<IEmote>(new EmoteTypeReader());
            await CommandService.AddModulesAsync(Assembly.GetExecutingAssembly(), ServiceProvider).ConfigureAwait(false);

            LoggerService = ServiceProvider.GetService<LoggerService>();
            SQLiteService = ServiceProvider.GetService<SQLiteService>();
            DiscordAppService = ServiceProvider.GetService<DiscordAppService>();
            RestWebService = ServiceProvider.GetService<RestWebService>();

            ProgramLoopManager = new ProgramLoopManager(ServiceProvider);
            await ProgramLoopManager.StartProgramLoopAsync();

            while (true)
            {
                var commandInput = await Console.In.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput.Trim();

                if (commandInput.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    break;

                //if (commandInput.Equals("Help", StringComparison.OrdinalIgnoreCase))
                //    await ShowHelpMenuAsync().ConfigureAwait(false);
                //else if (commandInput.Equals("AddDiscordApp", StringComparison.OrdinalIgnoreCase))
                //    await AddDiscordAppAsync().ConfigureAwait(false);
                //else if (commandInput.Equals("RemoveDiscordApp", StringComparison.OrdinalIgnoreCase))
                //    await RemoveDiscordAppAsync().ConfigureAwait(false);
                //else if (commandInput.Equals("StartDiscordApps", StringComparison.OrdinalIgnoreCase))
                //    await StartDiscordAppsAsync().ConfigureAwait(false);
                //else if (commandInput.Equals("StopDiscordApps", StringComparison.OrdinalIgnoreCase))
                //    await StopDiscordAppsAsync().ConfigureAwait(false);
                //else if (commandInput.Equals("AddGlobalAdmin", StringComparison.OrdinalIgnoreCase))
                //    await AddGlobalAdminAsync().ConfigureAwait(false);
                //else if (commandInput.Equals("RemoveGlobalAdmin", StringComparison.OrdinalIgnoreCase))
                //    await RemoveGlobalAdminAsync().ConfigureAwait(false);
                //else if (commandInput.Equals("StartNancyService", StringComparison.OrdinalIgnoreCase))
                //    await StartNancyService().ConfigureAwait(false);
                //else if (commandInput.Equals("StopNancyService", StringComparison.OrdinalIgnoreCase))
                //    await StopNancyService().ConfigureAwait(false);
                //else if (commandInput.Equals("SetNancyPort", StringComparison.OrdinalIgnoreCase))
                //    await SetNancyPort().ConfigureAwait(false);
                //else if (!string.IsNullOrEmpty(commandInput))
                //    LoggerService.LogMessage("Unknown command. Please try again.", ConsoleColor.Red);
            }

            await ProgramLoopManager.StopProgramLoopAsync();
        }

        private async Task ShowHelpMenuAsync()
        {
            LoggerService.LogMessage("==================================================");
            LoggerService.LogMessage("Command available:");
            LoggerService.LogMessage("==================================================");
            LoggerService.LogMessage("Exit - Close the application.");
            LoggerService.LogMessage("Help - Show the help menu.");
            LoggerService.LogMessage("AddDiscordApp - Add a new discord app into the database.");
            LoggerService.LogMessage("RemoveDiscordApp - Remove a discord app from the database.");
            LoggerService.LogMessage("StartDiscordApps - Start all discord app.");
            LoggerService.LogMessage("StopDiscordApps - Start all discord app.");
            LoggerService.LogMessage("AddGlobalAdmin - Add a user as a global admin.");
            LoggerService.LogMessage("RemoveGlobalAdmin - Remove a user as a global admin.");
            LoggerService.LogMessage("StartNancyService - Start nancy gateway service.");
            LoggerService.LogMessage("StopNancyService - Stop nancy gateway service.");
            LoggerService.LogMessage("SetNancyPort - Set nancy gateway service port.");
            LoggerService.LogMessage("==================================================");
        }

        //private async Task AddDiscordAppAsync()
        //{
        //    ulong clientId;
        //    string clientSecret = null, botToken = null;

        //    do
        //    {
        //        await LoggerService.LogMessage("Enter the Client ID of the Discord App you wish to add:");
        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (!ulong.TryParse(commandInput, out clientId))
        //            await LoggerService.LogMessage("Invalid ID, try again!", ConsoleColor.Red);
        //    } while (clientId == 0);

        //    do
        //    {
        //        await LoggerService.LogMessage("Enter the Client Secret of the Discord App you wish to add:");
        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (string.IsNullOrEmpty(commandInput))
        //            await LoggerService.LogMessage("Invalid Client Secret key, try again!", ConsoleColor.Red);
        //        else
        //            clientSecret = commandInput;
        //    } while (string.IsNullOrEmpty(clientSecret));

        //    do
        //    {
        //        await LoggerService.LogMessage("Enter the Bot Token of the Discord App you wish to add:");
        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (string.IsNullOrEmpty(commandInput))
        //            await LoggerService.LogMessage("Invalid Bot Token key, try again!", ConsoleColor.Red);
        //        else
        //            botToken = commandInput;
        //    } while (string.IsNullOrEmpty(botToken));

        //    while (true)
        //    {
        //        await LoggerService.LogMessage("Do you wish to add this Discord App with the following information:");
        //        await LoggerService.LogMessage($"Client ID: {clientId}");
        //        await LoggerService.LogMessage($"Client Secret: {clientSecret}");
        //        await LoggerService.LogMessage($"Bot Token: {botToken}");
        //        await LoggerService.LogMessage("Y/n?");

        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var discordApp = new DiscordAppTable { ClientId = clientId.ToString(), ClientSecret = clientSecret, BotToken = botToken };
        //            await SQLiteService.SQLiteAsyncConnection.InsertOrReplaceAsync(discordApp).ConfigureAwait(false);
        //            await LoggerService.LogMessage("Discord App have been added.", ConsoleColor.Green);
        //            break;
        //        }

        //        if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
        //        {
        //            await LoggerService.LogMessage("Discord App will not be added.", ConsoleColor.Red);
        //            break;
        //        }

        //        await LoggerService.LogMessage("Invalid input, try again!", ConsoleColor.Red);
        //    }
        //}

        //private async Task RemoveDiscordAppAsync()
        //{
        //    ulong clientId;
        //    DiscordAppTable discordApp = null;

        //    do
        //    {
        //        await LoggerService.LogMessage("Enter the Client ID of the Discord App you wish to remove:");
        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (!ulong.TryParse(commandInput, out clientId))
        //            await LoggerService.LogMessage("Invalid ID, try again!", ConsoleColor.Red);
        //        else
        //        {
        //            var clientIdString = clientId.ToString();
        //            discordApp = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);

        //            if (discordApp != null)
        //                continue;

        //            await LoggerService.LogMessage("Could not find any Discord App with this ID!", ConsoleColor.Red);
        //            return;
        //        }
        //    } while (clientId == 0);

        //    while (true)
        //    {
        //        await LoggerService.LogMessage("Do you wish to remove this Discord App with the following information:");
        //        await LoggerService.LogMessage($"App Name: {discordApp?.AppName}");
        //        await LoggerService.LogMessage($"Client ID: {clientId}");
        //        await LoggerService.LogMessage("Y/n?");

        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
        //        {
        //            await SQLiteService.SQLiteAsyncConnection.DeleteAsync(discordApp).ConfigureAwait(false);
        //            await LoggerService.LogMessage("Discord App have been deleted.", ConsoleColor.Green);
        //            break;
        //        }

        //        if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
        //        {
        //            await LoggerService.LogMessage("Discord App will not be deleted.", ConsoleColor.Red);
        //            break;
        //        }

        //        await LoggerService.LogMessage("Invalid input, try again!", ConsoleColor.Red);
        //    }
        //}

        //private async Task StartDiscordAppsAsync()
        //{
        //    var discordApps = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);

        //    foreach (var discordAppTable in discordApps)
        //    {
        //        var result = await DiscordAppService.StartDiscordAppAsync(Convert.ToUInt64(discordAppTable.ClientId)).ConfigureAwait(false);
        //        await LoggerService.LogMessage(result.BuildDiscordTextResponse(), ConsoleColor.Green);
        //    }

        //    await LoggerService.LogMessage("All discord apps have been started.", ConsoleColor.Green);
        //}

        //private async Task StopDiscordAppsAsync()
        //{
        //    var discordApps = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);

        //    foreach (var discordAppTable in discordApps)
        //    {
        //        var result = await DiscordAppService.StopDiscordAppAsync(Convert.ToUInt64(discordAppTable.ClientId))9;
        //        await LoggerService.LogMessage(result.BuildDiscordTextResponse(), ConsoleColor.Green).ConfigureAwait(false);
        //    }

        //    await LoggerService.LogMessage("All discord apps have been stopped.", ConsoleColor.Green).ConfigureAwait(false);
        //}

        //private async Task AddGlobalAdminAsync()
        //{
        //    ulong userId;

        //    do
        //    {
        //        await LoggerService.LogMessage("Enter the User ID that you wish to add as a global admin:").ConfigureAwait(false);
        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (!ulong.TryParse(commandInput, out userId))
        //            await LoggerService.LogMessage("Invalid ID, try again!", ConsoleColor.Red).ConfigureAwait(false);
        //    } while (userId == 0);

        //    while (true)
        //    {
        //        await LoggerService.LogMessage("Do you wish to add this user as a global admin with the following information:").ConfigureAwait(false);
        //        await LoggerService.LogMessage($"User ID: {userId}").ConfigureAwait(false);
        //        await LoggerService.LogMessage("Y/n?").ConfigureAwait(false);

        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var userIdString = userId.ToString();
        //            var discordAppOwner = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppOwnerTable>().Where(a => a.DiscordAppId == null && a.UserId == userIdString).FirstOrDefaultAsync().ConfigureAwait(false);

        //            if (discordAppOwner == null)
        //                await SQLiteService.SQLiteAsyncConnection.InsertAsync(new DiscordAppOwnerTable { UserId = userIdString }).ConfigureAwait(false);

        //            await LoggerService.LogMessage("User is now a global admin.", ConsoleColor.Green);
        //            break;
        //        }

        //        if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
        //        {
        //            await LoggerService.LogMessage("User will not be made global admin.", ConsoleColor.Red);
        //            break;
        //        }

        //        await LoggerService.LogMessage("Invalid input, try again!", ConsoleColor.Red);
        //    }
        //}

        //private async Task RemoveGlobalAdminAsync()
        //{
        //    ulong userId;
        //    DiscordAppOwnerTable discordAppOwner = null;

        //    do
        //    {
        //        await LoggerService.LogMessage("Enter the User ID that you wish to remove as a global admin:").ConfigureAwait(false);
        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (!ulong.TryParse(commandInput, out userId))
        //            await LoggerService.LogMessage("Invalid ID, try again!", ConsoleColor.Red).ConfigureAwait(false);
        //        else
        //        {
        //            var userIdString = userId.ToString();
        //            discordAppOwner = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppOwnerTable>().Where(a => a.DiscordAppId == null && a.UserId == userIdString).FirstOrDefaultAsync();

        //            if (discordAppOwner != null)
        //                continue;

        //            await LoggerService.LogMessage("Could not find any global admin with this ID!", ConsoleColor.Red).ConfigureAwait(false);
        //            return;
        //        }
        //    } while (userId == 0);

        //    while (true)
        //    {
        //        await LoggerService.LogMessage("Do you wish to remove this user as a global admin with the following information:").ConfigureAwait(false);
        //        await LoggerService.LogMessage($"User ID: {userId}").ConfigureAwait(false);
        //        await LoggerService.LogMessage("Y/n?").ConfigureAwait(false);

        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
        //        {
        //            await SQLiteService.SQLiteAsyncConnection.DeleteAsync(discordAppOwner).ConfigureAwait(false);
        //            await LoggerService.LogMessage("User is now not a global admin.", ConsoleColor.Green);
        //            break;
        //        }

        //        if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
        //        {
        //            await LoggerService.LogMessage("User will remain as a global admin.", ConsoleColor.Red);
        //            break;
        //        }

        //        await LoggerService.LogMessage("Invalid input, try again!", ConsoleColor.Red);
        //    }
        //}

        //private async Task StartNancyService()
        //{
        //    await RestWebService.StartAsync();
        //    await LoggerService.LogMessage("Nancy service started.", ConsoleColor.Green).ConfigureAwait(false);
        //}

        //private async Task StopNancyService()
        //{
        //    await RestWebService.StopAsync();
        //    await LoggerService.LogMessage("Nancy service stopped.", ConsoleColor.Green).ConfigureAwait(false);
        //}

        //private async Task SetNancyPort()
        //{
        //    ushort port;

        //    do
        //    {
        //        await LoggerService.LogMessage("Enter the port you wish to use for nancy (0-65535):").ConfigureAwait(false);
        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (!ushort.TryParse(commandInput, out port))
        //            await LoggerService.LogMessage("Invalid port number, try again!", ConsoleColor.Red).ConfigureAwait(false);
        //    } while (port == 0);

        //    while (true)
        //    {
        //        await LoggerService.LogMessage("Do you wish to change the nancy port with the following information:").ConfigureAwait(false);
        //        await LoggerService.LogMessage($"Nancy Port: {port}").ConfigureAwait(false);
        //        await LoggerService.LogMessage("Y/n?").ConfigureAwait(false);

        //        var commandInput = await LoggerService.ReadLineAsync().ConfigureAwait(false);
        //        commandInput = commandInput.Trim();

        //        if (string.IsNullOrEmpty(commandInput) || commandInput.Equals("y", StringComparison.OrdinalIgnoreCase))
        //        {
        //            await RestWebService.StartAsync(port);
        //            await LoggerService.LogMessage("Nancy port have been changed.", ConsoleColor.Green);
        //            break;
        //        }

        //        if (commandInput.Equals("n", StringComparison.OrdinalIgnoreCase))
        //        {
        //            await LoggerService.LogMessage("Nancy port will not be changed.", ConsoleColor.Red);
        //            break;
        //        }

        //        await LoggerService.LogMessage("Invalid input, try again!", ConsoleColor.Red);
        //    }
        //}
    }
}