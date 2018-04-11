using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;

namespace TheDialgaTeam.DiscordBot.Services
{
    public interface IDiscordAppService
    {
        List<IDiscordSocketClientModel> DiscordSocketClientModels { get; }

        Task ListDiscordAppOwner(ulong clientId = 0);

        Task AddDiscordAppOwner(ulong userId, ulong clientId = 0);

        Task RemoveDiscordAppOwner(ulong userId, ulong clientId = 0);

        Task ListDiscordApp();

        Task<bool> AddDiscordApp(ulong clientId, string botToken);

        Task<bool> RemoveDiscordApp(ulong clientId);

        Task ListRunningDiscordApp();

        Task<bool> StartDiscordApp(ulong clientId);

        Task StartDiscordApps();

        Task<bool> StopDiscordApp(ulong clientId);

        Task StopDiscordApps();
    }

    internal sealed class DiscordAppService : IDiscordAppService
    {
        public List<IDiscordSocketClientModel> DiscordSocketClientModels { get; } = new List<IDiscordSocketClientModel>();

        private IProgram Program { get; }

        private ILoggerService LoggerService { get; }

        private ISQLiteService SQLiteService { get; }

        public DiscordAppService(IProgram program, ILoggerService loggerService, ISQLiteService sqliteService)
        {
            Program = program;
            LoggerService = loggerService;
            SQLiteService = sqliteService;
        }

        public async Task ListDiscordAppOwner(ulong clientId = 0)
        {
            var stringClientId = clientId == 0 ? string.Empty : clientId.ToString();
            var discordAppOwnerModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppOwnerModel>().Where(a => a.ClientId == stringClientId).ToArrayAsync();

            if (discordAppOwnerModels.Length == 0)
            {
                await LoggerService.LogMessageAsync("No discord app owner have been found!");
                return;
            }

            await LoggerService.LogMessageAsync("==================================================");
            await LoggerService.LogMessageAsync("List of discord app owners:");
            await LoggerService.LogMessageAsync("==================================================");

            foreach (var discordAppOwnerModel in discordAppOwnerModels)
                await LoggerService.LogMessageAsync($"Id: {discordAppOwnerModel.Id} | UserId: {discordAppOwnerModel.UserId}");
        }

        public async Task AddDiscordAppOwner(ulong userId, ulong clientId = 0)
        {
            var stringUserId = userId.ToString();
            var stringClientId = clientId == 0 ? string.Empty : clientId.ToString();

            if (await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppOwnerModel>().Where(a => a.UserId == stringUserId && a.ClientId == stringClientId).CountAsync() > 0)
            {
                await LoggerService.LogMessageAsync("This user is already a discord app owner!");
                return;
            }

            await SQLiteService.SQLiteAsyncConnection.InsertAsync(new DiscordAppOwnerModel { UserId = stringUserId, ClientId = stringClientId });
            await LoggerService.LogMessageAsync("Successfully added a new discord app owner.");
        }

        public async Task RemoveDiscordAppOwner(ulong userId, ulong clientId = 0)
        {
            var stringUserId = userId.ToString();
            var stringClientId = clientId == 0 ? string.Empty : clientId.ToString();
            var discordAppOwnerModel = SQLiteService.SQLiteAsyncConnection.Table<DiscordAppOwnerModel>().Where(a => a.UserId == stringUserId && a.ClientId == stringClientId).FirstOrDefaultAsync();

            if (discordAppOwnerModel == null)
            {
                await LoggerService.LogMessageAsync("This user is not a discord app owner!");
                return;
            }

            await SQLiteService.SQLiteAsyncConnection.DeleteAsync(discordAppOwnerModel);
            await LoggerService.LogMessageAsync("Successfully removed the discord app owner.");
        }

        public async Task ListDiscordApp()
        {
            var discordAppModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().ToArrayAsync();

            if (discordAppModels.Length == 0)
            {
                await LoggerService.LogMessageAsync("No discord app have been found!");
                return;
            }

            await LoggerService.LogMessageAsync("==================================================");
            await LoggerService.LogMessageAsync("List of discord app:");
            await LoggerService.LogMessageAsync("==================================================");

            foreach (var discordAppModel in discordAppModels)
                await LoggerService.LogMessageAsync($"Id: {discordAppModel.Id} | ClientId: {discordAppModel.ClientId}");
        }

        public async Task<bool> AddDiscordApp(ulong clientId, string botToken)
        {
            var stringClientId = clientId.ToString();

            var discordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == stringClientId).FirstOrDefaultAsync() ?? new DiscordAppModel { ClientId = stringClientId };
            discordAppModel.SetBotToken(botToken);

            if (discordAppModel.Id == default(int))
            {
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(discordAppModel);
                await LoggerService.LogMessageAsync("Successfully added new discord app.");
            }
            else
            {
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordAppModel);
                await LoggerService.LogMessageAsync("Successfully updated the discord app.");
            }

            return true;
        }

        public async Task<bool> RemoveDiscordApp(ulong clientId)
        {
            var stringClientId = clientId.ToString();
            var discordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == stringClientId).FirstOrDefaultAsync();

            if (discordAppModel == null)
            {
                await LoggerService.LogMessageAsync("This discord app is does not exist!");
                return false;
            }

            await SQLiteService.SQLiteAsyncConnection.DeleteAsync(discordAppModel);
            await LoggerService.LogMessageAsync("Successfully remove the discord app.");
            return true;
        }

        public async Task ListRunningDiscordApp()
        {
            if (DiscordSocketClientModels.Count == 0)
            {
                await LoggerService.LogMessageAsync("No running discord app have been found!");
                return;
            }

            await LoggerService.LogMessageAsync("==================================================");
            await LoggerService.LogMessageAsync("List of running discord app:");
            await LoggerService.LogMessageAsync("==================================================");

            foreach (var discordSocketClientModel in DiscordSocketClientModels)
                await LoggerService.LogMessageAsync($"ClientId: {discordSocketClientModel.DiscordAppModel.ClientId}");
        }

        public async Task<bool> StartDiscordApp(ulong clientId)
        {
            var stringClientId = clientId.ToString();
            var discordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == stringClientId).FirstOrDefaultAsync();

            if (discordAppModel == null)
            {
                await LoggerService.LogMessageAsync("This discord app is does not exist!");
                return false;
            }

            foreach (var discordSocketClientModel in DiscordSocketClientModels)
            {
                if (discordSocketClientModel.DiscordAppModel.ClientId != clientId.ToString())
                    continue;

                await StopDiscordApp(clientId);
                break;
            }

            var newDiscordSocketClientModel = new DiscordSocketClientModel(discordAppModel);
            AddListener(newDiscordSocketClientModel);

            await newDiscordSocketClientModel.StartListening();

            DiscordSocketClientModels.Add(newDiscordSocketClientModel);
            return true;
        }

        public async Task StartDiscordApps()
        {
            await StopDiscordApps();

            var discordAppModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().ToArrayAsync();

            foreach (var discordAppModel in discordAppModels)
            {
                var newDiscordSocketClientModel = new DiscordSocketClientModel(discordAppModel);
                AddListener(newDiscordSocketClientModel);

                await newDiscordSocketClientModel.StartListening();

                DiscordSocketClientModels.Add(newDiscordSocketClientModel);
            }
        }

        public async Task<bool> StopDiscordApp(ulong clientId)
        {
            IDiscordSocketClientModel tempDiscordSocketClientModel = null;

            foreach (var discordSocketClientModel in DiscordSocketClientModels)
            {
                if (discordSocketClientModel.DiscordAppModel.ClientId != clientId.ToString())
                    continue;

                await discordSocketClientModel.StopListening();
                RemoveListener(discordSocketClientModel);
                await LoggerService.LogMessageAsync($"Successfully stopped {clientId} app.");

                tempDiscordSocketClientModel = discordSocketClientModel;
            }

            if (tempDiscordSocketClientModel == null)
            {
                await LoggerService.LogMessageAsync("App is not running!");
                return false;
            }

            DiscordSocketClientModels.Remove(tempDiscordSocketClientModel);
            return true;
        }

        public async Task StopDiscordApps()
        {
            foreach (var discordSocketClientModel in DiscordSocketClientModels)
            {
                await discordSocketClientModel.StopListening();
                RemoveListener(discordSocketClientModel);
                await LoggerService.LogMessageAsync($"Successfully stopped {discordSocketClientModel.DiscordAppModel.ClientId} app.");
            }

            DiscordSocketClientModels.Clear();
        }

        private void AddListener(IDiscordSocketClientModel discordSocketClientModel)
        {
            discordSocketClientModel.Log += DiscordSocketClientModelOnLog;
            discordSocketClientModel.Ready += DiscordSocketClientModelOnReady;
            discordSocketClientModel.MessageReceived += DiscordSocketClientModelOnMessageReceived;
        }

        private void RemoveListener(IDiscordSocketClientModel discordSocketClientModel)
        {
            discordSocketClientModel.MessageReceived -= DiscordSocketClientModelOnMessageReceived;
            discordSocketClientModel.Ready -= DiscordSocketClientModelOnReady;
            discordSocketClientModel.Log -= DiscordSocketClientModelOnLog;
        }

        private async Task DiscordSocketClientModelOnLog(IDiscordSocketClientModel socketClientModel, LogMessage logMessage)
        {
            await LoggerService.LogMessageAsync($"[Bot {socketClientModel.DiscordAppModel.ClientId}] {(string.IsNullOrEmpty(socketClientModel.DiscordSocketClient?.CurrentUser?.Username) ? logMessage.Message : socketClientModel.DiscordSocketClient?.CurrentUser?.Username + ": " + logMessage.Message)}");
        }

        private async Task DiscordSocketClientModelOnReady(IDiscordSocketClientModel socketClientModel)
        {
            await LoggerService.LogMessageAsync($"[Bot {socketClientModel.DiscordAppModel.ClientId}] {socketClientModel.DiscordSocketClient.CurrentUser.Username} have started!");

            if (!socketClientModel.DiscordAppModel.Verified)
            {
                if (socketClientModel.DiscordAppModel.ClientId != socketClientModel.DiscordSocketClient.CurrentUser.Id.ToString())
                {
                    await LoggerService.LogMessageAsync($"[Bot {socketClientModel.DiscordAppModel.ClientId}] Client Id mismatch. Verification failed!");
                    await StopDiscordApp(Convert.ToUInt64(socketClientModel.DiscordAppModel.ClientId));
                    await RemoveDiscordApp(Convert.ToUInt64(socketClientModel.DiscordAppModel.ClientId));
                    return;
                }

                socketClientModel.DiscordAppModel.Verified = true;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(socketClientModel.DiscordAppModel);
            }

            await socketClientModel.DiscordSocketClient.SetGameAsync($"@{socketClientModel.DiscordSocketClient.CurrentUser.Username} help");
        }

        private async Task DiscordSocketClientModelOnMessageReceived(IDiscordSocketClientModel socketClientModel, SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message))
                return;

            var argPos = 0;
            var context = new SocketCommandContext(socketClientModel.DiscordSocketClient, message);

            if (message.Channel is SocketDMChannel)
            {
                message.HasMentionPrefix(socketClientModel.DiscordSocketClient.CurrentUser, ref argPos);
            }
            else
            {
                var clientId = socketClientModel.DiscordSocketClient.CurrentUser.Id.ToString();
                var guildId = context.Guild.Id.ToString();
                var discordGuildModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

                if (discordGuildModel == null && !message.HasMentionPrefix(socketClientModel.DiscordSocketClient.CurrentUser, ref argPos))
                    return;

                if (!message.HasMentionPrefix(socketClientModel.DiscordSocketClient.CurrentUser, ref argPos) &&
                    !message.HasStringPrefix(discordGuildModel?.CharPrefix ?? "", ref argPos, StringComparison.OrdinalIgnoreCase))
                    return;
            }

            await Program.CommandService.ExecuteAsync(context, argPos, Program.ServiceProvider);
        }
    }
}