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

namespace TheDialgaTeam.DiscordBot.Services
{
    public interface IDiscordAppService
    {
        List<IDiscordSocketClientModel> DiscordSocketClientModels { get; }

        Task ListDiscordAppOwnerAsync(ulong clientId = 0);

        Task AddDiscordAppOwnerAsync(ulong userId, ulong clientId = 0);

        Task RemoveDiscordAppOwnerAsync(ulong userId, ulong clientId = 0);

        Task ListDiscordAppAsync();

        Task AddDiscordAppAsync(ulong clientId, string clientSecret, string botToken);

        Task<bool> RemoveDiscordAppAsync(ulong clientId);

        Task ListRunningDiscordAppAsync();

        Task<bool> StartDiscordAppAsync(ulong clientId);

        Task StartDiscordAppsAsync();

        Task<bool> StopDiscordAppAsync(ulong clientId);

        Task StopDiscordAppsAsync();
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

        public async Task ListDiscordAppOwnerAsync(ulong clientId = 0)
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

        public async Task AddDiscordAppOwnerAsync(ulong userId, ulong clientId = 0)
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

        public async Task RemoveDiscordAppOwnerAsync(ulong userId, ulong clientId = 0)
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

        public async Task ListDiscordAppAsync()
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

        public async Task AddDiscordAppAsync(ulong clientId, string clientSecret, string botToken)
        {
            var stringClientId = clientId.ToString();
            var discordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == stringClientId).FirstOrDefaultAsync();

            if (discordAppModel == null)
            {
                var discordAppModelNew = new DiscordAppModel { ClientId = clientId.ToString(), ClientSecret = clientSecret, BotToken = botToken };

                await SQLiteService.SQLiteAsyncConnection.InsertAsync(discordAppModelNew);
                await LoggerService.LogMessageAsync("Successfully added new discord app.");
            }
            else
            {
                discordAppModel.ClientSecret = clientSecret;
                discordAppModel.BotToken = botToken;

                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordAppModel);
                await LoggerService.LogMessageAsync("Successfully updated the discord app.");
            }
        }

        public async Task<bool> RemoveDiscordAppAsync(ulong clientId)
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

        public async Task ListRunningDiscordAppAsync()
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

        public async Task<bool> StartDiscordAppAsync(ulong clientId)
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

                await StopDiscordAppAsync(clientId);
                break;
            }

            var newDiscordSocketClientModel = new DiscordSocketClientModel(discordAppModel);
            AddListener(newDiscordSocketClientModel);

            await newDiscordSocketClientModel.StartListeningAsync();

            DiscordSocketClientModels.Add(newDiscordSocketClientModel);
            return true;
        }

        public async Task StartDiscordAppsAsync()
        {
            await StopDiscordAppsAsync();

            var discordAppModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().ToArrayAsync();

            foreach (var discordAppModel in discordAppModels)
            {
                var newDiscordSocketClientModel = new DiscordSocketClientModel(discordAppModel);
                AddListener(newDiscordSocketClientModel);

                await newDiscordSocketClientModel.StartListeningAsync();

                DiscordSocketClientModels.Add(newDiscordSocketClientModel);
            }
        }

        public async Task<bool> StopDiscordAppAsync(ulong clientId)
        {
            IDiscordSocketClientModel tempDiscordSocketClientModel = null;

            foreach (var discordSocketClientModel in DiscordSocketClientModels)
            {
                if (discordSocketClientModel.DiscordAppModel.ClientId != clientId.ToString())
                    continue;

                await discordSocketClientModel.StopListeningAsync();
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

        public async Task StopDiscordAppsAsync()
        {
            foreach (var discordSocketClientModel in DiscordSocketClientModels)
            {
                await discordSocketClientModel.StopListeningAsync();
                RemoveListener(discordSocketClientModel);
                await LoggerService.LogMessageAsync($"Successfully stopped {discordSocketClientModel.DiscordAppModel.ClientId} app.");
            }

            DiscordSocketClientModels.Clear();
        }

        private void AddListener(IDiscordSocketClientModel discordSocketClientModel)
        {
            discordSocketClientModel.Log += DiscordSocketClientModelOnLog;
            discordSocketClientModel.MessageReceived += DiscordSocketClientModelOnMessageReceived;
            discordSocketClientModel.LeftGuild += DiscordSocketClientModelOnLeftGuild;
            discordSocketClientModel.UserJoined += DiscordSocketClientModelOnUserJoined;
            discordSocketClientModel.Ready += DiscordSocketClientModelOnReady;
        }

        private void RemoveListener(IDiscordSocketClientModel discordSocketClientModel)
        {
            discordSocketClientModel.Ready -= DiscordSocketClientModelOnReady;
            discordSocketClientModel.UserJoined -= DiscordSocketClientModelOnUserJoined;
            discordSocketClientModel.LeftGuild -= DiscordSocketClientModelOnLeftGuild;
            discordSocketClientModel.MessageReceived -= DiscordSocketClientModelOnMessageReceived;
            discordSocketClientModel.Log -= DiscordSocketClientModelOnLog;
        }

        private async Task DiscordSocketClientModelOnLog(IDiscordSocketClientModel socketClientModel, LogMessage logMessage)
        {
            await LoggerService.LogMessageAsync($"[Bot {socketClientModel.DiscordAppModel.ClientId}] {(string.IsNullOrEmpty(socketClientModel.DiscordSocketClient?.CurrentUser?.Username) ? logMessage.Message : socketClientModel.DiscordSocketClient?.CurrentUser?.Username + ": " + logMessage.Message)}");
        }

        private async Task DiscordSocketClientModelOnMessageReceived(IDiscordSocketClientModel socketClientModel, SocketMessage socketMessage)
        {
            await Task.Run(async () =>
            {
                if (!(socketMessage is SocketUserMessage message))
                    return;

                var argPos = 0;
                var context = new SocketCommandContext(socketClientModel.DiscordSocketClient, message);

                if (message.Channel is SocketDMChannel)
                    message.HasMentionPrefix(socketClientModel.DiscordSocketClient.CurrentUser, ref argPos);
                else
                {
                    var clientId = socketClientModel.DiscordSocketClient.CurrentUser.Id.ToString();
                    var guildId = context.Guild.Id.ToString();
                    var discordGuildModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

                    if (discordGuildModel == null && !message.HasMentionPrefix(socketClientModel.DiscordSocketClient.CurrentUser, ref argPos))
                        return;

                    if (!message.HasMentionPrefix(socketClientModel.DiscordSocketClient.CurrentUser, ref argPos) &&
                        !message.HasStringPrefix(discordGuildModel?.StringPrefix ?? "", ref argPos, StringComparison.OrdinalIgnoreCase))
                        return;
                }

                await Program.CommandService.ExecuteAsync(context, argPos, Program.ServiceProvider);
            });
        }

        private async Task DiscordSocketClientModelOnLeftGuild(IDiscordSocketClientModel socketClientModel, SocketGuild socketGuild)
        {
            await Task.Run(async () =>
            {
                var clientId = socketClientModel.DiscordAppModel.ClientId;
                var guildId = socketGuild.Id.ToString();

                foreach (var socketGuildChannel in socketGuild.Channels)
                {
                    var channelId = socketGuildChannel.Id.ToString();
                    await SQLiteService.SQLiteAsyncConnection.Table<DiscordChannelModeratorModel>().DeleteAsync(a => a.ClientId == clientId && a.ChannelId == channelId);
                }

                await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
                await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModeratorModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
                await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
                await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
                await SQLiteService.SQLiteAsyncConnection.Table<ServerHoundModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
            });
        }

        private async Task DiscordSocketClientModelOnUserJoined(IDiscordSocketClientModel socketClientModel, SocketGuildUser socketGuildUser)
        {
            await Task.Run(async () =>
            {
                var clientId = socketClientModel.DiscordAppModel.ClientId;
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
                    var permission = socketGuildUser.Guild.GetUser(socketClientModel.DiscordSocketClient.CurrentUser.Id).GuildPermissions;
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
            });
        }

        private async Task DiscordSocketClientModelOnReady(IDiscordSocketClientModel socketClientModel)
        {
            await Task.Run(async () =>
            {
                await LoggerService.LogMessageAsync($"[Bot {socketClientModel.DiscordAppModel.ClientId}] {socketClientModel.DiscordSocketClient.CurrentUser.Username} have started!");

                if (!socketClientModel.DiscordAppModel.Verified)
                {
                    if (socketClientModel.DiscordAppModel.ClientId != socketClientModel.DiscordSocketClient.CurrentUser.Id.ToString())
                    {
                        await LoggerService.LogMessageAsync($"[Bot {socketClientModel.DiscordAppModel.ClientId}] Client Id mismatch. Verification failed!");
                        await StopDiscordAppAsync(Convert.ToUInt64(socketClientModel.DiscordAppModel.ClientId));
                        await RemoveDiscordAppAsync(Convert.ToUInt64(socketClientModel.DiscordAppModel.ClientId));
                        return;
                    }

                    socketClientModel.DiscordAppModel.Verified = true;
                    await SQLiteService.SQLiteAsyncConnection.UpdateAsync(socketClientModel.DiscordAppModel);
                }

                await socketClientModel.DiscordSocketClient.SetGameAsync($"@{socketClientModel.DiscordSocketClient.CurrentUser.Username} help");

                socketClientModel.DiscordAppModel.AppName = socketClientModel.DiscordSocketClient.CurrentUser.Username;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(socketClientModel.DiscordAppModel);
            });
        }
    }
}