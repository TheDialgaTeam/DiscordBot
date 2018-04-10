using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Command;
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

                ServiceProvider = ServiceCollection.BuildServiceProvider();

                CommandService = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });
                await CommandService.AddModulesAsync(Assembly.GetEntryAssembly());

                LoggerService = ServiceProvider.GetRequiredService<ILoggerService>();
                await LoggerService.LogMessageAsync("==================================================");
                await LoggerService.LogMessageAsync("The Dialga Team Discord Bot (.NET Core)");
                await LoggerService.LogMessageAsync("==================================================");
                await LoggerService.LogMessageAsync("Please wait while the bot is initializing...");

                SQLiteService = ServiceProvider.GetRequiredService<ISQLiteService>();
                await SQLiteService.InitializeDatabaseAsync();

                await LoggerService.LogMessageAsync("Done initializing!");

                DiscordAppService = ServiceProvider.GetRequiredService<IDiscordAppService>();
                await DiscordAppService.StartDiscordApps();
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
                        await ListDiscordAppOwner(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("AddDiscordAppOwner", StringComparison.OrdinalIgnoreCase))
                        await AddDiscordAppOwner(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("RemoveDiscordAppOwner", StringComparison.OrdinalIgnoreCase))
                        await RemoveDiscordAppOwner(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("ListDiscordApp", StringComparison.OrdinalIgnoreCase))
                        await ListDiscordApp();
                    else if (commandProcessorModel.Command.Equals("AddDiscordApp", StringComparison.OrdinalIgnoreCase))
                        await AddDiscordApp(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("RemoveDiscordApp", StringComparison.OrdinalIgnoreCase))
                        await RemoveDiscordApp(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("ListRunningDiscordApp", StringComparison.OrdinalIgnoreCase))
                        await ListRunningDiscordApp();
                    else if (commandProcessorModel.Command.Equals("StartDiscordApp", StringComparison.OrdinalIgnoreCase))
                        await StartDiscordApp(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("StartDiscordApps", StringComparison.OrdinalIgnoreCase))
                        await StartDiscordApps();
                    else if (commandProcessorModel.Command.Equals("StopDiscordApp", StringComparison.OrdinalIgnoreCase))
                        await StopDiscordApp(commandProcessorModel);
                    else if (commandProcessorModel.Command.Equals("StopDiscordApps", StringComparison.OrdinalIgnoreCase))
                        await StopDiscordApps();
                }
                catch (Exception ex)
                {
                    await LoggerService.LogErrorMessageAsync(ex.ToString());
                }
            }
        }

        private async Task ListDiscordAppOwner(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null)
                await DiscordAppService.ListDiscordAppOwner();
            else
                await DiscordAppService.ListDiscordAppOwner((ulong)paramObject[0]);
        }

        private async Task AddDiscordAppOwner(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null || paramObject[1] == null)
                await DiscordAppService.AddDiscordAppOwner((ulong)paramObject[0]);
            else
                await DiscordAppService.AddDiscordAppOwner((ulong)paramObject[0], (ulong)paramObject[1]);
        }

        private async Task RemoveDiscordAppOwner(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null || paramObject[1] == null)
                await DiscordAppService.RemoveDiscordAppOwner((ulong)paramObject[0]);
            else
                await DiscordAppService.RemoveDiscordAppOwner((ulong)paramObject[0], (ulong)paramObject[1]);
        }

        private async Task ListDiscordApp()
        {
            await DiscordAppService.ListDiscordApp();
        }

        private async Task AddDiscordApp(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong, CommandProcessorModel.ParamenterType.String });

            if (paramObject?[0] == null || paramObject[1] == null)
                return;

            await DiscordAppService.AddDiscordApp((ulong)paramObject[0], paramObject[1].ToString());
        }

        private async Task RemoveDiscordApp(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null)
                return;

            await DiscordAppService.RemoveDiscordApp((ulong)paramObject[0]);
        }

        private async Task ListRunningDiscordApp()
        {
            await DiscordAppService.ListRunningDiscordApp();
        }

        private async Task StartDiscordApp(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null)
                return;

            await DiscordAppService.StartDiscordApp((ulong)paramObject[0]);
        }

        private async Task StartDiscordApps()
        {
            await DiscordAppService.StartDiscordApps();
        }

        private async Task StopDiscordApp(ICommandProcessorModel commandProcessorModel)
        {
            var paramObject = commandProcessorModel.GetCommandParamenterTypeObjects(new[] { CommandProcessorModel.ParamenterType.ULong });

            if (paramObject?[0] == null)
                return;

            await DiscordAppService.StopDiscordApp((ulong)paramObject[0]);
        }

        private async Task StopDiscordApps()
        {
            await DiscordAppService.StopDiscordApps();
        }
    }
}