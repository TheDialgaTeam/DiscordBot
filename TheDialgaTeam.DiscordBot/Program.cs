using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Command;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot
{
    internal sealed class Program
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private IServiceCollection ServiceCollection { get; set; }

        private CommandService CommandService { get; set; }

        private ILoggerService LoggerService { get; set; }

        private ISQLiteService SQLiteService { get; set; }

        private IDiscordSocketClientService DiscordSocketClientService { get; set; }

        public static void Main(string[] args)
        {
            var program = new Program();
            program.MainAsync(args).GetAwaiter().GetResult();
        }

        private async Task MainAsync(string[] args)
        {
            try
            {
                CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });
                await CommandService.AddModulesAsync(Assembly.GetEntryAssembly());

                ServiceCollection = new ServiceCollection();
                ServiceCollection.AddSingleton(this);
                ServiceCollection.AddSingleton(ServiceCollection);
                ServiceCollection.AddSingleton(CommandService);
                ServiceCollection.AddSingleton<ILoggerService, LoggerService>();
                ServiceCollection.AddSingleton<ISQLiteService, SQLiteService>();
                ServiceCollection.AddSingleton<IDiscordSocketClientService, DiscordSocketClientService>();

                ServiceProvider = ServiceCollection.BuildServiceProvider();

                LoggerService = ServiceProvider.GetRequiredService<ILoggerService>();
                await LoggerService.LogMessageAsync("==================================================");
                await LoggerService.LogMessageAsync("The Dialga Team Discord Bot (.NET Core)");
                await LoggerService.LogMessageAsync("==================================================");
                await LoggerService.LogMessageAsync("Please wait while the bot is initializing...");

                SQLiteService = ServiceProvider.GetRequiredService<ISQLiteService>();
                await SQLiteService.InitializeDatabaseAsync();

                DiscordSocketClientService = ServiceProvider.GetRequiredService<IDiscordSocketClientService>();

                await LoggerService.LogMessageAsync("Done initializing!");

                await DiscordSocketClientService.StartDiscordSocketClients();
            }
            catch (Exception ex)
            {
                await LoggerService.LogErrorMessageAsync(ex.ToString());
            }

            while (true)
            {
                try
                {
                    var input = await LoggerService.ReadLineAsync();
                    ICommandProcessorModel commandProcessorModel = new CommandProcessorModel(input);

                    if (commandProcessorModel.Command.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    if (commandProcessorModel.Command.Equals("AddBotInstance", StringComparison.OrdinalIgnoreCase))
                        await AddBotInstance(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("RemoveBotInstance", StringComparison.OrdinalIgnoreCase))
                        await RemoveBotInstance(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("ListBotInstances", StringComparison.OrdinalIgnoreCase))
                        await ListBotInstances(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("StartBotInstance", StringComparison.OrdinalIgnoreCase))
                        await StartBotInstance(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("StartBotInstances", StringComparison.OrdinalIgnoreCase))
                        await StartBotInstances(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("StopBotInstance", StringComparison.OrdinalIgnoreCase))
                        await StopBotInstance(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("StopBotInstances", StringComparison.OrdinalIgnoreCase))
                        await StopBotInstances(commandProcessorModel);
                }
                catch (Exception ex)
                {
                    await LoggerService.LogErrorMessageAsync(ex.ToString());
                }
            }
        }

        private async Task AddBotInstance(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.String });

            if (paramObject?[0] == null || paramObject[1] == null)
                return;

            await DiscordSocketClientService.AddDiscordSocketClient((ulong)paramObject[0], paramObject[1].ToString());
        }

        private async Task RemoveBotInstance(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null)
                return;

            await DiscordSocketClientService.RemoveDiscordSocketClient((ulong)paramObject[0]);
        }

        private async Task ListBotInstances(ICommandProcessorModel commandProcessorModel)
        {
            var botInstances = await SQLiteService.SQLiteAsyncConnection.Table<BotInstanceModel>().ToArrayAsync();

            await LoggerService.LogMessageAsync("==================================================");
            await LoggerService.LogMessageAsync("List of bots registered:");
            await LoggerService.LogMessageAsync("==================================================");

            foreach (var botInstance in botInstances)
                await LoggerService.LogMessageAsync($"Id: {botInstance.Id} | BotId: {botInstance.BotId}");
        }

        private async Task StartBotInstance(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null)
                return;

            await DiscordSocketClientService.StartDiscordSocketClient((ulong)paramObject[0]);
        }

        private async Task StartBotInstances(ICommandProcessorModel commandProcessorModel)
        {
            await DiscordSocketClientService.StartDiscordSocketClients();
        }

        private async Task StopBotInstance(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null)
                return;

            await DiscordSocketClientService.StopDiscordSocketClient((ulong)paramObject[0]);
        }

        private async Task StopBotInstances(ICommandProcessorModel commandProcessorModel)
        {
            await DiscordSocketClientService.StopDiscordSocketClients();
        }
    }
}