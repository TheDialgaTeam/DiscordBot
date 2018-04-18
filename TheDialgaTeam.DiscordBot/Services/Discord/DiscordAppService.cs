using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;
using TheDialgaTeam.DiscordBot.Services.Logger;
using TheDialgaTeam.DiscordBot.Services.SQLite;

namespace TheDialgaTeam.DiscordBot.Services.Discord
{
    public interface IDiscordAppService
    {
        List<IDiscordShardedClientModel> DiscordShardedClientModels { get; }

        Task ListRunningDiscordAppAsync();

        Task<bool> StartDiscordAppAsync(ulong clientId);

        Task StartDiscordAppsAsync();

        Task<bool> StopDiscordAppAsync(ulong clientId);

        Task StopDiscordAppsAsync();
    }

    internal sealed class DiscordAppService : IDiscordAppService
    {
        public List<IDiscordShardedClientModel> DiscordShardedClientModels { get; } = new List<IDiscordShardedClientModel>();

        private IProgram Program { get; }

        private ILoggerService LoggerService { get; }

        private ISQLiteService SQLiteService { get; }

        public DiscordAppService(IProgram program, ILoggerService loggerService, ISQLiteService sqliteService)
        {
            Program = program;
            LoggerService = loggerService;
            SQLiteService = sqliteService;
        }

        public async Task ListRunningDiscordAppAsync()
        {
            await LoggerService.LogMessageAsync("==================================================");
            await LoggerService.LogMessageAsync("List of running Discord App:");
            await LoggerService.LogMessageAsync("==================================================");

            foreach (var discordSocketClientModel in DiscordShardedClientModels)
                await LoggerService.LogMessageAsync($"{discordSocketClientModel.DiscordAppModel.ClientId}");
        }

        public async Task<bool> StartDiscordAppAsync(ulong clientId)
        {
            var stringClientId = clientId.ToString();
            var discordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == stringClientId).FirstOrDefaultAsync();

            if (discordAppModel == null)
            {
                await LoggerService.LogMessageAsync("This discord app is does not exist!");
                return false;
            }

            foreach (var discordSocketClientModel in DiscordShardedClientModels)
            {
                if (discordSocketClientModel.DiscordAppModel.ClientId != clientId.ToString())
                    continue;

                await StopDiscordAppAsync(clientId);
                break;
            }

            var newDiscordShardedClientModel = new DiscordShardedClientModel(discordAppModel);
            AddListener(newDiscordShardedClientModel);

            await newDiscordShardedClientModel.StartListeningAsync();

            DiscordShardedClientModels.Add(newDiscordShardedClientModel);
            return true;
        }

        public async Task StartDiscordAppsAsync()
        {
            await StopDiscordAppsAsync();

            var discordAppModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().ToArrayAsync();

            foreach (var discordAppModel in discordAppModels)
            {
                var newDiscordShardedClientModel = new DiscordShardedClientModel(discordAppModel);
                AddListener(newDiscordShardedClientModel);

                await newDiscordShardedClientModel.StartListeningAsync();

                DiscordShardedClientModels.Add(newDiscordShardedClientModel);
            }
        }

        public async Task<bool> StopDiscordAppAsync(ulong clientId)
        {
            IDiscordShardedClientModel tempDiscordShardedClientModel = null;

            foreach (var discordSocketClientModel in DiscordShardedClientModels)
            {
                if (discordSocketClientModel.DiscordAppModel.ClientId != clientId.ToString())
                    continue;

                await discordSocketClientModel.StopListeningAsync();
                RemoveListener(discordSocketClientModel);
                await LoggerService.LogMessageAsync($"Successfully stopped {clientId} app.");

                tempDiscordShardedClientModel = discordSocketClientModel;
            }

            if (tempDiscordShardedClientModel == null)
            {
                await LoggerService.LogMessageAsync("App is not running!");
                return false;
            }

            DiscordShardedClientModels.Remove(tempDiscordShardedClientModel);
            return true;
        }

        public async Task StopDiscordAppsAsync()
        {
            foreach (var discordSocketClientModel in DiscordShardedClientModels)
            {
                await discordSocketClientModel.StopListeningAsync();
                RemoveListener(discordSocketClientModel);
                await LoggerService.LogMessageAsync($"Successfully stopped {discordSocketClientModel.DiscordAppModel.ClientId} app.");
            }

            DiscordShardedClientModels.Clear();
        }

        private void AddListener(IDiscordShardedClientModel discordShardedClientModel)
        {
            discordShardedClientModel.Log += DiscordShardedClientModelOnLog;
            discordShardedClientModel.MessageReceived += DiscordShardedClientModelOnMessageReceived;
            discordShardedClientModel.LeftGuild += DiscordShardedClientModelOnLeftGuild;
            discordShardedClientModel.UserJoined += DiscordShardedClientModelOnUserJoined;
            discordShardedClientModel.ShardReady += DiscordShardedClientModelOnShardReady;
        }

        private void RemoveListener(IDiscordShardedClientModel discordShardedClientModel)
        {
            discordShardedClientModel.ShardReady -= DiscordShardedClientModelOnShardReady;
            discordShardedClientModel.UserJoined -= DiscordShardedClientModelOnUserJoined;
            discordShardedClientModel.LeftGuild -= DiscordShardedClientModelOnLeftGuild;
            discordShardedClientModel.MessageReceived -= DiscordShardedClientModelOnMessageReceived;
            discordShardedClientModel.Log -= DiscordShardedClientModelOnLog;
        }

        private async Task DiscordShardedClientModelOnLog(IDiscordShardedClientModel discordShardedClientModel, LogMessage logMessage)
        {
            await LoggerService.LogMessageAsync(discordShardedClientModel, logMessage);
        }

        private async Task DiscordShardedClientModelOnMessageReceived(IDiscordShardedClientModel discordShardedClientModel, SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage socketUserMessage))
                return;

            ICommandContext context = null;

            switch (socketUserMessage.Channel)
            {
                case SocketDMChannel _:
                case SocketGroupChannel _:
                    context = new SocketCommandContext(discordShardedClientModel.DiscordShardedClient.GetShard(0), socketUserMessage);
                    break;

                case SocketGuildChannel socketGuildChannel:
                    context = new SocketCommandContext(discordShardedClientModel.DiscordShardedClient.GetShardFor(socketGuildChannel.Guild), socketUserMessage);
                    break;
            }

            if (context == null)
                return;

            var argPos = 0;

            if (socketUserMessage.Channel is SocketDMChannel)
                socketUserMessage.HasMentionPrefix(discordShardedClientModel.DiscordShardedClient.CurrentUser, ref argPos);
            else
            {
                var clientId = discordShardedClientModel.DiscordShardedClient.CurrentUser.Id.ToString();
                var guildId = context.Guild.Id.ToString();
                var discordGuildModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

                if (discordGuildModel == null && !socketUserMessage.HasMentionPrefix(discordShardedClientModel.DiscordShardedClient.CurrentUser, ref argPos))
                    return;

                if (!socketUserMessage.HasMentionPrefix(discordShardedClientModel.DiscordShardedClient.CurrentUser, ref argPos) &&
                    !socketUserMessage.HasStringPrefix(discordGuildModel?.StringPrefix ?? "", ref argPos, StringComparison.OrdinalIgnoreCase))
                    return;
            }

            await Program.CommandService.ExecuteAsync(context, argPos, Program.ServiceProvider);
        }

        private async Task DiscordShardedClientModelOnLeftGuild(IDiscordShardedClientModel discordShardedClientModel, SocketGuild socketGuild)
        {
            var clientId = discordShardedClientModel.DiscordAppModel.ClientId;
            var guildId = socketGuild.Id.ToString();

            await SQLiteService.SQLiteAsyncConnection.Table<DiscordChannelModeratorModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
            await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
            await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModeratorModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
            await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
            await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
            await SQLiteService.SQLiteAsyncConnection.Table<ServerHoundModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
        }

        private async Task DiscordShardedClientModelOnUserJoined(IDiscordShardedClientModel discordShardedClientModel, SocketGuildUser socketGuildUser)
        {
            var clientId = discordShardedClientModel.DiscordAppModel.ClientId;
            var guildId = socketGuildUser.Guild.Id.ToString();

            var discordGuildModuleModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId && a.Module == "ServerHound").FirstOrDefaultAsync();

            if (!discordGuildModuleModel?.Active ?? true)
                return;

            var serverHoundModel = await SQLiteService.SQLiteAsyncConnection.Table<ServerHoundModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (!serverHoundModel?.DBans ?? true)
                return;

            // Check DBans :)
            var httpClient = new HttpClient();

            var values = new Dictionary<string, string>
                {
                    { "token", "Ln3SIwdcIu" },
                    { "userid", socketGuildUser.Id.ToString() }
                };

            var content = new FormUrlEncodedContent(values);
            var response = await httpClient.PostAsync("https://bans.discordlist.net/api", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (responseString.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                var permission = socketGuildUser.Guild.GetUser(discordShardedClientModel.DiscordShardedClient.CurrentUser.Id).GuildPermissions;
                var dmChannel = await socketGuildUser.GetOrCreateDMChannelAsync();

                if (permission.BanMembers)
                {
                    await socketGuildUser.Guild.AddBanAsync(socketGuildUser, reason: "You have been banned due to ServerHound DBans listing.");
                    await dmChannel.SendMessageAsync("You have been banned from the guild due to ServerHound DBans listing. If you think that this is a false positive, please goto: https://discord.gg/MPDPnEw and make an appeal to de-list your account.");
                }
                else if (permission.KickMembers)
                {
                    await socketGuildUser.KickAsync("You have been kicked due to ServerHound DBans listing.");
                    await dmChannel.SendMessageAsync("You have been kicked from the guild due to ServerHound DBans listing. If you think that this is a false positive, please goto: https://discord.gg/MPDPnEw and make an appeal to de-list your account.");
                }
            }
        }

        private async Task DiscordShardedClientModelOnShardReady(IDiscordShardedClientModel discordShardedClientModel, DiscordSocketClient discordSocketClient)
        {
            await LoggerService.LogMessageAsync($"[Bot {discordShardedClientModel.DiscordAppModel.ClientId}] {discordShardedClientModel.DiscordShardedClient.CurrentUser.Username} have started!");

            if (!discordShardedClientModel.DiscordAppModel.Verified)
            {
                if (discordShardedClientModel.DiscordAppModel.ClientId != discordShardedClientModel.DiscordShardedClient.CurrentUser.Id.ToString())
                {
                    await LoggerService.LogMessageAsync($"[Bot {discordShardedClientModel.DiscordAppModel.ClientId}] Client Id mismatch. Verification failed!");
                    await StopDiscordAppAsync(Convert.ToUInt64(discordShardedClientModel.DiscordAppModel.ClientId));
                    //await RemoveDiscordAppAsync(Convert.ToUInt64(discordShardedClientModel.DiscordAppModel.ClientId));
                    return;
                }

                discordShardedClientModel.DiscordAppModel.Verified = true;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordShardedClientModel.DiscordAppModel);
            }

            await discordShardedClientModel.DiscordShardedClient.SetGameAsync($"@{discordShardedClientModel.DiscordShardedClient.CurrentUser.Username} help");

            discordShardedClientModel.DiscordAppModel.AppName = discordShardedClientModel.DiscordShardedClient.CurrentUser.Username;
            await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordShardedClientModel.DiscordAppModel);

            //discordShardedClientModel.IsReady = true;
        }
    }
}