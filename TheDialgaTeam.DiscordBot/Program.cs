using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Command;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Services;

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

        private IDiscordAppService DiscordAppService { get; set; }

        private IPollHandlerService PollHandlerService { get; set; }

        private IWebService WebService { get; set; }

        public static void Main(string[] args)
        {
            var program = new Program();
            program.MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            try
            {
                ServiceCollection = new ServiceCollection();
                ServiceCollection.AddSingleton<IProgram>(this);
                ServiceCollection.AddSingleton<ILoggerService, LoggerService>();
                ServiceCollection.AddSingleton<ISQLiteService, SQLiteService>();
                ServiceCollection.AddSingleton<IDiscordAppService, DiscordAppService>();
                ServiceCollection.AddSingleton<IPollHandlerService, PollHandlerService>();
                ServiceCollection.AddSingleton<IWebService, WebService>();

                ServiceProvider = ServiceCollection.BuildServiceProvider();

                CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, DefaultRunMode = RunMode.Async });
                CommandService.AddTypeReader<IEmote>(new EmoteTypeReader());
                await CommandService.AddModulesAsync(Assembly.GetEntryAssembly());

                LoggerService = ServiceProvider.GetRequiredService<ILoggerService>();
                await LoggerService.LogMessageAsync("==================================================");
                await LoggerService.LogMessageAsync("The Dialga Team Discord Bot (.NET Core)");
                await LoggerService.LogMessageAsync("==================================================");
                await LoggerService.LogMessageAsync("Please wait while the bot is initializing...\n");

                SQLiteService = ServiceProvider.GetRequiredService<ISQLiteService>();
                await SQLiteService.InitializeDatabaseAsync();

                DiscordAppService = ServiceProvider.GetRequiredService<IDiscordAppService>();

                PollHandlerService = ServiceProvider.GetRequiredService<IPollHandlerService>();

                var pollUpdateHandler = new Thread(async () => await PollHandlerService.UpdatePollTask()) { IsBackground = true };
                pollUpdateHandler.Start();

                WebService = ServiceProvider.GetRequiredService<IWebService>();
                await WebService.StartAsync();

                await LoggerService.LogMessageAsync("\nDone initializing!");
                await LoggerService.LogMessageAsync("Use StartDiscordApp or StartDiscordApps to start the bot instances.");

                await DiscordAppService.StartDiscordAppsAsync();
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
                catch (Exception ex)
                {
                    await LoggerService.LogErrorMessageAsync(ex.ToString());
                }
            }
        }

        private async Task ListDiscordAppOwnerAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null)
                await DiscordAppService.ListDiscordAppOwnerAsync();
            else
                await DiscordAppService.ListDiscordAppOwnerAsync((ulong)paramObject[0]);
        }

        private async Task AddDiscordAppOwnerAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });
            var paramObject2 = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.ULong });

            if (paramObject2?[0] != null && paramObject2[1] != null)
                await DiscordAppService.AddDiscordAppOwnerAsync((ulong)paramObject2[0], (ulong)paramObject2[1]);
            else if (paramObject?[0] != null)
                await DiscordAppService.AddDiscordAppOwnerAsync((ulong)paramObject[0]);
        }

        private async Task RemoveDiscordAppOwnerAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });
            var paramObject2 = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.ULong });

            if (paramObject2?[0] != null && paramObject2[1] != null)
                await DiscordAppService.RemoveDiscordAppOwnerAsync((ulong)paramObject2[0], (ulong)paramObject2[1]);
            else if (paramObject?[0] != null)
                await DiscordAppService.RemoveDiscordAppOwnerAsync((ulong)paramObject[0]);
        }

        private async Task ListDiscordAppAsync()
        {
            await DiscordAppService.ListDiscordAppAsync();
        }

        private async Task AddDiscordAppAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.String, CommandProcessorModel.ParamenterType.String });

            if (paramObject?[0] == null || paramObject[1] == null || paramObject[2] == null)
                return;

            await DiscordAppService.AddDiscordAppAsync((ulong)paramObject[0], paramObject[1].ToString(), paramObject[2].ToString());
        }

        private async Task RemoveDiscordAppAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null)
                return;

            await DiscordAppService.RemoveDiscordAppAsync((ulong)paramObject[0]);
        }

        private async Task ListRunningDiscordAppAsync()
        {
            await DiscordAppService.ListRunningDiscordAppAsync();
        }

        private async Task StartDiscordAppAsync(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

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
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

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