using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services.Discord;

namespace TheDialgaTeam.DiscordBot.Services
{
    public interface IDiscordSocketClientService
    {
        List<IDiscordSocketClientModel> DiscordSocketClientModels { get; }

        Task AddDiscordSocketClient(ulong botId, string botToken);

        Task RemoveDiscordSocketClient(ulong botId);

        Task StartDiscordSocketClient(ulong botId);

        Task StartDiscordSocketClients();

        Task StopDiscordSocketClient(ulong botId);

        Task StopDiscordSocketClients();
    }

    internal sealed class DiscordSocketClientService : IDiscordSocketClientService
    {
        public List<IDiscordSocketClientModel> DiscordSocketClientModels { get; } = new List<IDiscordSocketClientModel>();

        private Program Program { get; }

        private CommandService CommandService { get; }

        private ILoggerService LoggerService { get; }

        private ISQLiteService SQLiteService { get; }

        public DiscordSocketClientService(Program program, CommandService commandService, ILoggerService loggerService, ISQLiteService sqliteService)
        {
            Program = program;
            CommandService = commandService;
            LoggerService = loggerService;
            SQLiteService = sqliteService;
        }

        public async Task AddDiscordSocketClient(ulong botId, string botToken)
        {
            var botIdString = botId.ToString();
            IBotInstanceModel botInstanceModel = await SQLiteService.SQLiteAsyncConnection.Table<BotInstanceModel>().Where(a => a.BotId == botIdString).FirstOrDefaultAsync();

            if (botInstanceModel != null)
            {
                botInstanceModel.BotToken = botToken;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(botInstanceModel);
                await LoggerService.LogMessageAsync("Successfully updated the bot into the database!");
            }
            else
            {
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(new BotInstanceModel { BotId = botId.ToString(), BotToken = botToken });
                await LoggerService.LogMessageAsync("Successfully added a new bot into the database!");
            }
        }

        public async Task RemoveDiscordSocketClient(ulong botId)
        {
            var botIdString = botId.ToString();
            IBotInstanceModel botInstanceModel = await SQLiteService.SQLiteAsyncConnection.Table<BotInstanceModel>().Where(a => a.BotId == botIdString).FirstOrDefaultAsync();

            if (botInstanceModel == null)
                await LoggerService.LogMessageAsync("Unable to find the bot from the database!");
            else
            {
                await SQLiteService.SQLiteAsyncConnection.DeleteAsync(botInstanceModel);
                await LoggerService.LogMessageAsync("Successfully removed the bot from the database!");
            }
        }

        public async Task StartDiscordSocketClient(ulong botId)
        {
            await StopDiscordSocketClient(botId);

            var botIdString = botId.ToString();
            IBotInstanceModel botInstanceModel = await SQLiteService.SQLiteAsyncConnection.Table<BotInstanceModel>().Where(a => a.BotId == botIdString).FirstOrDefaultAsync();

            if (botInstanceModel != null)
            {
                var discordSocketClientModel = new DiscordSocketClientModel(LoggerService, SQLiteService, botInstanceModel);
                await discordSocketClientModel.StartListening();

                discordSocketClientModel.Log += DiscordSocketClientModelOnLog;
                discordSocketClientModel.Ready += DiscordSocketClientModelOnReady;
                discordSocketClientModel.MessageReceived += DiscordSocketClientModelOnMessageReceived;

                DiscordSocketClientModels.Add(discordSocketClientModel);
            }
        }

        public async Task StartDiscordSocketClients()
        {
            if (DiscordSocketClientModels.Count > 0)
                await StopDiscordSocketClients();

            IBotInstanceModel[] botInstanceModels = await SQLiteService.SQLiteAsyncConnection.Table<BotInstanceModel>().ToArrayAsync();

            foreach (var botInstanceModel in botInstanceModels)
            {
                var discordSocketClientModel = new DiscordSocketClientModel(LoggerService, SQLiteService, botInstanceModel);
                await discordSocketClientModel.StartListening();

                discordSocketClientModel.Log += DiscordSocketClientModelOnLog;
                discordSocketClientModel.Ready += DiscordSocketClientModelOnReady;
                discordSocketClientModel.MessageReceived += DiscordSocketClientModelOnMessageReceived;

                DiscordSocketClientModels.Add(discordSocketClientModel);
            }
        }

        public async Task StopDiscordSocketClient(ulong botId)
        {
            for (var i = 0; i < DiscordSocketClientModels.Count; i++)
            {
                if (DiscordSocketClientModels[i].DiscordSocketClient.CurrentUser.Id != botId)
                    continue;

                await DiscordSocketClientModels[i].StopListening();

                DiscordSocketClientModels[i].MessageReceived -= DiscordSocketClientModelOnMessageReceived;
                DiscordSocketClientModels[i].Ready -= DiscordSocketClientModelOnReady;
                DiscordSocketClientModels[i].Log -= DiscordSocketClientModelOnLog;

                DiscordSocketClientModels.Remove(DiscordSocketClientModels[i]);
                break;
            }
        }

        public async Task StopDiscordSocketClients()
        {
            foreach (var discordSocketClientModel in DiscordSocketClientModels)
            {
                await discordSocketClientModel.StopListening();

                discordSocketClientModel.MessageReceived -= DiscordSocketClientModelOnMessageReceived;
                discordSocketClientModel.Ready -= DiscordSocketClientModelOnReady;
                discordSocketClientModel.Log -= DiscordSocketClientModelOnLog;
            }

            DiscordSocketClientModels.Clear();
        }

        private async Task DiscordSocketClientModelOnLog(IDiscordSocketClientModel socketClientModel, LogMessage logMessage)
        {
            await LoggerService.LogMessageAsync($"[Bot {socketClientModel.BotInstanceModel.Id}] {socketClientModel.DiscordSocketClient?.CurrentUser?.Username}: {logMessage.Message}");
        }

        private async Task DiscordSocketClientModelOnReady(IDiscordSocketClientModel socketClientModel)
        {
            await socketClientModel.DiscordSocketClient.SetGameAsync($"@{socketClientModel.DiscordSocketClient.CurrentUser.Username} help");
        }

        private async Task DiscordSocketClientModelOnMessageReceived(IDiscordSocketClientModel socketClientModel, SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message))
                return;

            var argPos = 0;

            if (socketMessage.Channel.GetType() != typeof(SocketDMChannel) && !message.HasMentionPrefix(socketClientModel.DiscordSocketClient.CurrentUser, ref argPos))
                return;

            var context = new SocketCommandContext(socketClientModel.DiscordSocketClient, message);

            await CommandService.ExecuteAsync(context, argPos, Program.ServiceProvider);
        }
    }
}