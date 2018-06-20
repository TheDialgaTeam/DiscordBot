using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Services;
using TheDialgaTeam.DiscordBot.Services.Discord;
using TheDialgaTeam.DiscordBot.Services.Logger;
using TheDialgaTeam.DiscordBot.Services.Nancy;
using TheDialgaTeam.DiscordBot.Services.SQLite;

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

        //private IPollHandlerService PollHandlerService { get; set; }

        //private IWebService WebService { get; set; }

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

            ServiceCollection.AddSingleton<IDiscordAppService, DiscordAppService>();
            //ServiceCollection.AddSingleton<IPollHandlerService, PollHandlerService>();
            //ServiceCollection.AddSingleton<IWebService, WebService>();

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

            DiscordAppService = ServiceProvider.GetRequiredService<IDiscordAppService>();

            //PollHandlerService = ServiceProvider.GetRequiredService<IPollHandlerService>();
            //PollHandlerService.UpdatePollTask();

            //WebService = ServiceProvider.GetRequiredService<IWebService>();
            //await WebService.StartAsync();

            await LoggerService.LogMessageAsync("\nDone initializing!");

            await DiscordAppService.StartDiscordAppsAsync();
        }
    }
}