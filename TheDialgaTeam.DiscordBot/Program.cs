using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.DiscordBot.Model.Command;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services;
using TheDialgaTeam.DiscordBot.Services.Discord;
using TheDialgaTeam.DiscordBot.Services.Logger;
using TheDialgaTeam.DiscordBot.Services.Nancy;
using TheDialgaTeam.DiscordBot.Services.SQLite;
using TheDialgaTeam.DiscordBot.Services.SQLite.Table;

namespace TheDialgaTeam.DiscordBot
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

        private IServiceCollection ServiceCollection { get; set; }

        private ILoggerService LoggerService { get; set; }

        private ISQLiteService SQLiteService { get; set; }

        private IDiscordAppOwnerTableService DiscordAppOwnerTableService { get; set; }

        private IDiscordAppTableService DiscordAppTableService { get; set; }

        private IDiscordAppService DiscordAppService { get; set; }

        private IPollHandlerService PollHandlerService { get; set; }

        private IWebService WebService { get; set; }

        public static void Main()
        {
            var program = new Program();
            program.MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddSingleton<IProgram>(this);
            ServiceCollection.AddSingleton<ILoggerService, LoggerService>();

            ServiceCollection.AddSingleton<ISQLiteService, SQLiteService>();
            ServiceCollection.AddSingleton<IDiscordAppOwnerTableService, DiscordAppOwnerTableService>();
            ServiceCollection.AddSingleton<IDiscordAppTableService, DiscordAppTableService>();

            ServiceCollection.AddSingleton<IDiscordAppService, DiscordAppService>();
            ServiceCollection.AddSingleton<IPollHandlerService, PollHandlerService>();
            ServiceCollection.AddSingleton<IWebService, WebService>();

            ServiceProvider = ServiceCollection.BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });
            CommandService.AddTypeReader<IEmote>(new EmoteTypeReader());
            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);

            LoggerService = ServiceProvider.GetRequiredService<ILoggerService>();
            await LoggerService.LogMessageAsync("==================================================");
            await LoggerService.LogMessageAsync("The Dialga Team Discord Bot (.NET Core)");
            await LoggerService.LogMessageAsync("==================================================");
            await LoggerService.LogMessageAsync("Please wait while the bot is initializing...\n");

            SQLiteService = ServiceProvider.GetRequiredService<ISQLiteService>();
            await SQLiteService.InitializeDatabaseAsync();

            DiscordAppOwnerTableService = ServiceProvider.GetRequiredService<IDiscordAppOwnerTableService>();
            DiscordAppTableService = ServiceProvider.GetRequiredService<IDiscordAppTableService>();

            DiscordAppService = ServiceProvider.GetRequiredService<IDiscordAppService>();

            PollHandlerService = ServiceProvider.GetRequiredService<IPollHandlerService>();
            PollHandlerService.UpdatePollTask();

            WebService = ServiceProvider.GetRequiredService<IWebService>();
            await WebService.StartAsync();

            await LoggerService.LogMessageAsync("\nDone initializing!");

            await DiscordAppService.StartDiscordAppsAsync();

            while (true)
            {
                var input = await LoggerService.ReadLineAsync();
                ICommandProcessorModel commandProcessorModel = new CommandProcessorModel(input);

                if (commandProcessorModel.Command.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (commandProcessorModel.Command.Equals("ListDiscordAppOwner", StringComparison.OrdinalIgnoreCase))
                    await ListDiscordAppOwnerAsync(commandProcessorModel);
                else if (commandProcessorModel.Command.Equals("AddDiscordAppOwner", StringComparison.OrdinalIgnoreCase))
                    await AddDiscordAppOwnerAsync(commandProcessorModel);
                else if (commandProcessorModel.Command.Equals("RemoveDiscordAppOwner", StringComparison.OrdinalIgnoreCase))
                    await RemoveDiscordAppOwnerAsync(commandProcessorModel);
                else if (commandProcessorModel.Command.Equals("ListDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await ListDiscordAppAsync();
                else if (commandProcessorModel.Command.Equals("AddDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await AddDiscordAppAsync(commandProcessorModel);
                else if (commandProcessorModel.Command.Equals("RemoveDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await RemoveDiscordAppAsync(commandProcessorModel);
                else if (commandProcessorModel.Command.Equals("ListRunningDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await ListRunningDiscordAppAsync();
                else if (commandProcessorModel.Command.Equals("StartDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await StartDiscordAppAsync(commandProcessorModel);
                else if (commandProcessorModel.Command.Equals("StartDiscordApps", StringComparison.OrdinalIgnoreCase))
                    await StartDiscordAppsAsync();
                else if (commandProcessorModel.Command.Equals("StopDiscordApp", StringComparison.OrdinalIgnoreCase))
                    await StopDiscordAppAsync(commandProcessorModel);
                else if (commandProcessorModel.Command.Equals("StopDiscordApps", StringComparison.OrdinalIgnoreCase))
                    await StopDiscordAppsAsync();
            }
        }

        private async Task ListDiscordAppOwnerAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(CommandProcessorModel.ParamenterType.ULong);
            var discordAppOwnerModels = await DiscordAppOwnerTableService.GetAllAsync(paramObject?[0] as ulong?);

            await LoggerService.LogMessageAsync("==================================================");
            await LoggerService.LogMessageAsync("List of Discord App Owner:");
            await LoggerService.LogMessageAsync("==================================================");

            foreach (var discordAppOwnerModel in discordAppOwnerModels)
                await LoggerService.LogMessageAsync($"{discordAppOwnerModel.UserId}");
        }

        private async Task AddDiscordAppOwnerAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.ULong);
            var paramObject2 = commandProcessorModel.GetCommandParamenterTypeObjects(CommandProcessorModel.ParamenterType.ULong);

            IDiscordAppOwnerModel discordAppOwnerModel = null;

            if (paramObject != null)
                discordAppOwnerModel = await DiscordAppOwnerTableService.SetAsync((ulong)paramObject[1], paramObject[0] as ulong?);
            else if (paramObject2 != null)
                discordAppOwnerModel = await DiscordAppOwnerTableService.SetAsync((ulong)paramObject2[0]);

            if (discordAppOwnerModel != null)
                await LoggerService.LogMessageAsync("Successfully added a new Discord App Owner!");
        }

        private async Task RemoveDiscordAppOwnerAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.ULong);
            var paramObject2 = commandProcessorModel.GetCommandParamenterTypeObjects(CommandProcessorModel.ParamenterType.ULong);

            var removeCount = 0;

            if (paramObject != null)
                removeCount = await DiscordAppOwnerTableService.RemoveAsync((ulong)paramObject[1], paramObject[0] as ulong?);
            else if (paramObject2 != null)
                removeCount = await DiscordAppOwnerTableService.RemoveAsync((ulong)paramObject2[0]);

            if (removeCount > 0)
                await LoggerService.LogMessageAsync("Successfully removed a Discord App Owner!");
        }

        private async Task ListDiscordAppAsync()
        {
            var discordAppModels = await DiscordAppTableService.GetAllAsync();

            await LoggerService.LogMessageAsync("==================================================");
            await LoggerService.LogMessageAsync("List of Discord App:");
            await LoggerService.LogMessageAsync("==================================================");

            foreach (var discordAppModel in discordAppModels)
                await LoggerService.LogMessageAsync($"{discordAppModel.ClientId}");
        }

        private async Task AddDiscordAppAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.String, CommandProcessorModel.ParamenterType.String);

            if (paramObject?[0] == null || paramObject[1] == null || paramObject[2] == null)
                return;

            var discordAppModel = DiscordAppTableService.SetAsync((ulong)paramObject[0], paramObject[1].ToString(), paramObject[2].ToString());

            if (discordAppModel != null)
                await LoggerService.LogMessageAsync("Successfully added a Discord App!");
        }

        private async Task RemoveDiscordAppAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(CommandProcessorModel.ParamenterType.ULong);

            if (paramObject?[0] == null)
                return;

            if (await DiscordAppTableService.RemoveAsync((ulong)paramObject[0]) > 0)
                await LoggerService.LogMessageAsync("Successfully removed a Discord App!");
        }

        private async Task ListRunningDiscordAppAsync()
        {
            await DiscordAppService.ListRunningDiscordAppAsync();
        }

        private async Task StartDiscordAppAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(CommandProcessorModel.ParamenterType.ULong);

            if (paramObject?[0] == null)
                return;

            await DiscordAppService.StartDiscordAppAsync((ulong)paramObject[0]);
        }

        private async Task StartDiscordAppsAsync()
        {
            await DiscordAppService.StartDiscordAppsAsync();
        }

        private async Task StopDiscordAppAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(CommandProcessorModel.ParamenterType.ULong);

            if (paramObject?[0] == null)
                return;

            await DiscordAppService.StopDiscordAppAsync((ulong)paramObject[0]);
        }

        private async Task StopDiscordAppsAsync()
        {
            await DiscordAppService.StopDiscordAppsAsync();
        }
    }
}