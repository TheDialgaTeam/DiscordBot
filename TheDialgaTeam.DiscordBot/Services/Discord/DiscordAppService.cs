using System.Collections.Generic;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services.Logger;
using TheDialgaTeam.DiscordBot.Services.SQLite;

namespace TheDialgaTeam.DiscordBot.Services.Discord
{
    public interface IDiscordAppService
    {
        List<IDiscordShardedClientHelper> DiscordShardedClientHelpers { get; }
    }

    internal sealed class DiscordAppService : IDiscordAppService
    {
        public List<IDiscordShardedClientHelper> DiscordShardedClientHelpers { get; } = new List<IDiscordShardedClientHelper>();

        private IProgram Program { get; }

        private ILoggerService LoggerService { get; }

        private ISQLiteService SQLiteService { get; }

        public DiscordAppService(IProgram program, ILoggerService loggerService, ISQLiteService sqliteService)
        {
            Program = program;
            LoggerService = loggerService;
            SQLiteService = sqliteService;
        }

        public async Task StartDiscordAppServiceAsync()
        {
            var discordApps = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);

            foreach (var discordApp in discordApps)
            {
                if (string.IsNullOrEmpty(discordApp.BotToken))
                    continue;

                var discordShardedClientHelper = new DiscordShardedClientHelper(discordApp.BotToken);
                await discordShardedClientHelper.StartListeningAsync().ConfigureAwait(false);

                DiscordShardedClientHelpers.Add(discordShardedClientHelper);
            }
        }

        //private void AddListener(IDiscordShardedClientHelper discordShardedClientHelper)
        //{
        //    discordShardedClientHelper.Log += DiscordShardedClientModelOnLog;
        //    discordShardedClientHelper.MessageReceived += DiscordShardedClientModelOnMessageReceived;
        //    discordShardedClientHelper.LeftGuild += DiscordShardedClientModelOnLeftGuild;
        //    discordShardedClientHelper.UserJoined += DiscordShardedClientModelOnUserJoined;
        //    discordShardedClientHelper.ShardReady += DiscordShardedClientModelOnShardReady;
        //}

        //private void RemoveListener(IDiscordShardedClientHelper discordShardedClientHelper)
        //{
        //    discordShardedClientHelper.ShardReady -= DiscordShardedClientModelOnShardReady;
        //    discordShardedClientHelper.UserJoined -= DiscordShardedClientModelOnUserJoined;
        //    discordShardedClientHelper.LeftGuild -= DiscordShardedClientModelOnLeftGuild;
        //    discordShardedClientHelper.MessageReceived -= DiscordShardedClientModelOnMessageReceived;
        //    discordShardedClientHelper.Log -= DiscordShardedClientModelOnLog;
        //}

        //private Task DiscordShardedClientModelOnLog(IDiscordShardedClient discordShardedClientHelper, LogMessage logMessage)
        //{
        //    Task.Run(async () => await LoggerService.LogMessageAsync(discordShardedClientHelper, logMessage)).ConfigureAwait(false);

        //    return Task.CompletedTask;
        //}

        //private Task DiscordShardedClientModelOnMessageReceived(IDiscordShardedClient discordShardedClientHelper, SocketMessage socketMessage)
        //{
        //    Task.Run(async () =>
        //    {
        //        if (!(socketMessage is SocketUserMessage socketUserMessage))
        //            return;

        //        ICommandContext context = null;

        //        switch (socketUserMessage.Channel)
        //        {
        //            case SocketDMChannel _:
        //            case SocketGroupChannel _:
        //                context = new SocketCommandContext(discordShardedClientHelper.DiscordShardedClient.GetShard(0), socketUserMessage);

        //                break;

        //            case SocketGuildChannel socketGuildChannel:
        //                context = new SocketCommandContext(discordShardedClientHelper.DiscordShardedClient.GetShardFor(socketGuildChannel.Guild), socketUserMessage);

        //                break;
        //        }

        //        if (context == null)
        //            return;

        //        var argPos = 0;

        //        if (socketUserMessage.Channel is SocketDMChannel)
        //            socketUserMessage.HasMentionPrefix(discordShardedClientHelper.DiscordShardedClient.CurrentUser, ref argPos);
        //        else
        //        {
        //            var clientId = discordShardedClientHelper.DiscordShardedClient.CurrentUser.Id.ToString();
        //            var guildId = context.Guild.Id.ToString();
        //            var discordGuildModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

        //            if (discordGuildModel == null && !socketUserMessage.HasMentionPrefix(discordShardedClientHelper.DiscordShardedClient.CurrentUser, ref argPos))
        //                return;

        //            if (!socketUserMessage.HasMentionPrefix(discordShardedClientHelper.DiscordShardedClient.CurrentUser, ref argPos) &&
        //                !socketUserMessage.HasStringPrefix(discordGuildModel?.StringPrefix ?? "", ref argPos, StringComparison.OrdinalIgnoreCase))
        //                return;
        //        }

        //        await Program.CommandService.ExecuteAsync(context, argPos, Program.ServiceProvider);
        //    }).ConfigureAwait(false);

        //    return Task.CompletedTask;
        //}

        //private Task DiscordShardedClientModelOnLeftGuild(IDiscordShardedClient discordShardedClientHelper, SocketGuild socketGuild)
        //{
        //    Task.Run(async () =>
        //    {
        //        var clientId = discordShardedClientHelper.DiscordAppModel.ClientId;
        //        var guildId = socketGuild.Id.ToString();

        //        await SQLiteService.SQLiteAsyncConnection.Table<DiscordChannelModeratorModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
        //        await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
        //        await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModeratorModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
        //        await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
        //        await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
        //        await SQLiteService.SQLiteAsyncConnection.Table<PollModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
        //        await SQLiteService.SQLiteAsyncConnection.Table<ServerHoundModel>().DeleteAsync(a => a.ClientId == clientId && a.GuildId == guildId);
        //    }).ConfigureAwait(false);

        //    return Task.CompletedTask;
        //}

        //private Task DiscordShardedClientModelOnUserJoined(IDiscordShardedClient discordShardedClientHelper, SocketGuildUser socketGuildUser)
        //{
        //    Task.Run(async () =>
        //    {
        //        var clientId = discordShardedClientHelper.DiscordAppModel.ClientId;
        //        var guildId = socketGuildUser.Guild.Id.ToString();

        //        var discordGuildModuleModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId && a.Module == "ServerHound").FirstOrDefaultAsync();

        //        if (!discordGuildModuleModel?.Active ?? true)
        //            return;

        //        var serverHoundModel = await SQLiteService.SQLiteAsyncConnection.Table<ServerHoundModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

        //        if (!serverHoundModel?.DBans ?? true)
        //            return;

        //        // Check DBans :)
        //        var httpClient = new HttpClient();

        //        var values = new Dictionary<string, string>
        //        {
        //            { "token", "Ln3SIwdcIu" },
        //            { "userid", socketGuildUser.Id.ToString() }
        //        };

        //        var content = new FormUrlEncodedContent(values);
        //        var response = await httpClient.PostAsync("https://bans.discordlist.net/api", content);
        //        var responseString = await response.Content.ReadAsStringAsync();

        //        if (responseString.Equals("true", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var permission = socketGuildUser.Guild.GetUser(discordShardedClientHelper.DiscordShardedClient.CurrentUser.Id).GuildPermissions;
        //            var dmChannel = await socketGuildUser.GetOrCreateDMChannelAsync();

        //            if (permission.BanMembers)
        //            {
        //                await socketGuildUser.Guild.AddBanAsync(socketGuildUser, reason: "You have been banned due to ServerHound DBans listing.");
        //                await dmChannel.SendMessageAsync("You have been banned from the guild due to ServerHound DBans listing. If you think that this is a false positive, please goto: https://discord.gg/MPDPnEw and make an appeal to de-list your account.");
        //            }
        //            else if (permission.KickMembers)
        //            {
        //                await socketGuildUser.KickAsync("You have been kicked due to ServerHound DBans listing.");
        //                await dmChannel.SendMessageAsync("You have been kicked from the guild due to ServerHound DBans listing. If you think that this is a false positive, please goto: https://discord.gg/MPDPnEw and make an appeal to de-list your account.");
        //            }
        //        }
        //    }).ConfigureAwait(false);

        //    return Task.CompletedTask;
        //}

        //private Task DiscordShardedClientModelOnShardReady(IDiscordShardedClient discordShardedClientHelper, DiscordSocketClient discordSocketClient)
        //{
        //    Task.Run(async () =>
        //    {
        //        await LoggerService.LogMessageAsync($"[Bot {discordShardedClientHelper.DiscordAppModel.ClientId}] {discordShardedClientHelper.DiscordShardedClient.CurrentUser.Username} have started!");

        //        if (!discordShardedClientHelper.DiscordAppModel.Verified)
        //        {
        //            if (discordShardedClientHelper.DiscordAppModel.ClientId != discordShardedClientHelper.DiscordShardedClient.CurrentUser.Id.ToString())
        //            {
        //                await LoggerService.LogMessageAsync($"[Bot {discordShardedClientHelper.DiscordAppModel.ClientId}] Client Id mismatch. Verification failed!");
        //                await StopDiscordAppAsync(Convert.ToUInt64(discordShardedClientHelper.DiscordAppModel.ClientId));

        //                //await RemoveDiscordAppAsync(Convert.ToUInt64(discordShardedClientHelper.DiscordAppModel.ClientId));
        //                return;
        //            }

        //            discordShardedClientHelper.DiscordAppModel.Verified = true;
        //            await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordShardedClientHelper.DiscordAppModel);
        //        }

        //        await discordShardedClientHelper.DiscordShardedClient.SetGameAsync($"@{discordShardedClientHelper.DiscordShardedClient.CurrentUser.Username} help");

        //        discordShardedClientHelper.DiscordAppModel.AppName = discordShardedClientHelper.DiscordShardedClient.CurrentUser.Username;
        //        await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordShardedClientHelper.DiscordAppModel);

        //        //discordShardedClientHelper.IsReady = true;
        //    }).ConfigureAwait(false);

        //    return Task.CompletedTask;
        //}
    }
}