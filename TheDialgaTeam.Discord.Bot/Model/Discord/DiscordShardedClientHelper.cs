using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace TheDialgaTeam.Discord.Bot.Model.Discord
{
    public interface IDiscordShardedClientHelper : IDisposable
    {
        event Func<DiscordShardedClient, LogMessage, Task> Log;

        event Func<DiscordShardedClient, Task> LoggedIn;

        event Func<DiscordShardedClient, Task> LoggedOut;

        event Func<DiscordShardedClient, SocketChannel, Task> ChannelCreated;

        event Func<DiscordShardedClient, SocketChannel, Task> ChannelDestroyed;

        event Func<DiscordShardedClient, SocketChannel, SocketChannel, Task> ChannelUpdated;

        event Func<DiscordShardedClient, SocketMessage, Task> MessageReceived;

        event Func<DiscordShardedClient, Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;

        event Func<DiscordShardedClient, Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;

        event Func<DiscordShardedClient, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;

        event Func<DiscordShardedClient, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;

        event Func<DiscordShardedClient, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;

        event Func<DiscordShardedClient, SocketRole, Task> RoleCreated;

        event Func<DiscordShardedClient, SocketRole, Task> RoleDeleted;

        event Func<DiscordShardedClient, SocketRole, SocketRole, Task> RoleUpdated;

        event Func<DiscordShardedClient, SocketGuild, Task> JoinedGuild;

        event Func<DiscordShardedClient, SocketGuild, Task> LeftGuild;

        event Func<DiscordShardedClient, SocketGuild, Task> GuildAvailable;

        event Func<DiscordShardedClient, SocketGuild, Task> GuildUnavailable;

        event Func<DiscordShardedClient, SocketGuild, Task> GuildMembersDownloaded;

        event Func<DiscordShardedClient, SocketGuild, SocketGuild, Task> GuildUpdated;

        event Func<DiscordShardedClient, SocketGuildUser, Task> UserJoined;

        event Func<DiscordShardedClient, SocketGuildUser, Task> UserLeft;

        event Func<DiscordShardedClient, SocketUser, SocketGuild, Task> UserBanned;

        event Func<DiscordShardedClient, SocketUser, SocketGuild, Task> UserUnbanned;

        event Func<DiscordShardedClient, SocketUser, SocketUser, Task> UserUpdated;

        event Func<DiscordShardedClient, SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;

        event Func<DiscordShardedClient, SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;

        event Func<DiscordShardedClient, SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;

        event Func<DiscordShardedClient, SocketUser, ISocketMessageChannel, Task> UserIsTyping;

        event Func<DiscordShardedClient, SocketGroupUser, Task> RecipientAdded;

        event Func<DiscordShardedClient, SocketGroupUser, Task> RecipientRemoved;

        event Func<DiscordShardedClient, DiscordSocketClient, Task> ShardConnected;

        event Func<DiscordShardedClient, Exception, DiscordSocketClient, Task> ShardDisconnected;

        event Func<DiscordShardedClient, DiscordSocketClient, Task> ShardReady;

        event Func<DiscordShardedClient, int, int, DiscordSocketClient, Task> ShardLatencyUpdated;

        DiscordShardedClient DiscordShardedClient { get; }

        Task StartListeningAsync();

        Task StopListeningAsync();
    }

    internal sealed class DiscordShardedClientHelper : IDiscordShardedClientHelper
    {
        public event Func<DiscordShardedClient, LogMessage, Task> Log;

        public event Func<DiscordShardedClient, Task> LoggedIn;

        public event Func<DiscordShardedClient, Task> LoggedOut;

        public event Func<DiscordShardedClient, SocketChannel, Task> ChannelCreated;

        public event Func<DiscordShardedClient, SocketChannel, Task> ChannelDestroyed;

        public event Func<DiscordShardedClient, SocketChannel, SocketChannel, Task> ChannelUpdated;

        public event Func<DiscordShardedClient, SocketMessage, Task> MessageReceived;

        public event Func<DiscordShardedClient, Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;

        public event Func<DiscordShardedClient, Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;

        public event Func<DiscordShardedClient, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;

        public event Func<DiscordShardedClient, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;

        public event Func<DiscordShardedClient, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;

        public event Func<DiscordShardedClient, SocketRole, Task> RoleCreated;

        public event Func<DiscordShardedClient, SocketRole, Task> RoleDeleted;

        public event Func<DiscordShardedClient, SocketRole, SocketRole, Task> RoleUpdated;

        public event Func<DiscordShardedClient, SocketGuild, Task> JoinedGuild;

        public event Func<DiscordShardedClient, SocketGuild, Task> LeftGuild;

        public event Func<DiscordShardedClient, SocketGuild, Task> GuildAvailable;

        public event Func<DiscordShardedClient, SocketGuild, Task> GuildUnavailable;

        public event Func<DiscordShardedClient, SocketGuild, Task> GuildMembersDownloaded;

        public event Func<DiscordShardedClient, SocketGuild, SocketGuild, Task> GuildUpdated;

        public event Func<DiscordShardedClient, SocketGuildUser, Task> UserJoined;

        public event Func<DiscordShardedClient, SocketGuildUser, Task> UserLeft;

        public event Func<DiscordShardedClient, SocketUser, SocketGuild, Task> UserBanned;

        public event Func<DiscordShardedClient, SocketUser, SocketGuild, Task> UserUnbanned;

        public event Func<DiscordShardedClient, SocketUser, SocketUser, Task> UserUpdated;

        public event Func<DiscordShardedClient, SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;

        public event Func<DiscordShardedClient, SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;

        public event Func<DiscordShardedClient, SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;

        public event Func<DiscordShardedClient, SocketUser, ISocketMessageChannel, Task> UserIsTyping;

        public event Func<DiscordShardedClient, SocketGroupUser, Task> RecipientAdded;

        public event Func<DiscordShardedClient, SocketGroupUser, Task> RecipientRemoved;

        public event Func<DiscordShardedClient, DiscordSocketClient, Task> ShardConnected;

        public event Func<DiscordShardedClient, Exception, DiscordSocketClient, Task> ShardDisconnected;

        public event Func<DiscordShardedClient, DiscordSocketClient, Task> ShardReady;

        public event Func<DiscordShardedClient, int, int, DiscordSocketClient, Task> ShardLatencyUpdated;

        public DiscordShardedClient DiscordShardedClient { get; }

        private ulong ClientId { get; }

        private string BotToken { get; }

        public DiscordShardedClientHelper(ulong clientId, string botToken, DiscordSocketConfig config = null)
        {
            DiscordShardedClient = new DiscordShardedClient(config ?? new DiscordSocketConfig { LogLevel = LogSeverity.Verbose });
            ClientId = clientId;
            BotToken = botToken;
        }

        public async Task StartListeningAsync()
        {
            try
            {
                AddListener();

                await DiscordShardedClient.LoginAsync(TokenType.Bot, BotToken).ConfigureAwait(false);
                await DiscordShardedClient.StartAsync().ConfigureAwait(false);

                if (DiscordShardedClient.LoginState == LoginState.LoggedIn)
                {
                    if (ClientId != DiscordShardedClient.CurrentUser.Id)
                        await StopListeningAsync().ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                await StopListeningAsync().ConfigureAwait(false);
            }
        }

        public async Task StopListeningAsync()
        {
            await DiscordShardedClient.LogoutAsync().ConfigureAwait(false);
            await DiscordShardedClient.StopAsync().ConfigureAwait(false);

            RemoveListener();
        }

        private void AddListener()
        {
            DiscordShardedClient.Log += DiscordShardedClientOnLog;
            DiscordShardedClient.LoggedIn += DiscordShardedClientOnLoggedIn;
            DiscordShardedClient.LoggedOut += DiscordShardedClientOnLoggedOut;
            DiscordShardedClient.ChannelCreated += DiscordShardedClientOnChannelCreated;
            DiscordShardedClient.ChannelDestroyed += DiscordShardedClientOnChannelDestroyed;
            DiscordShardedClient.ChannelUpdated += DiscordShardedClientOnChannelUpdated;
            DiscordShardedClient.MessageReceived += DiscordShardedClientOnMessageReceived;
            DiscordShardedClient.MessageDeleted += DiscordShardedClientOnMessageDeleted;
            DiscordShardedClient.MessageUpdated += DiscordShardedClientOnMessageUpdated;
            DiscordShardedClient.ReactionAdded += DiscordShardedClientOnReactionAdded;
            DiscordShardedClient.ReactionRemoved += DiscordShardedClientOnReactionRemoved;
            DiscordShardedClient.ReactionsCleared += DiscordShardedClientOnReactionsCleared;
            DiscordShardedClient.RoleCreated += DiscordShardedClientOnRoleCreated;
            DiscordShardedClient.RoleDeleted += DiscordShardedClientOnRoleDeleted;
            DiscordShardedClient.RoleUpdated += DiscordShardedClientOnRoleUpdated;
            DiscordShardedClient.JoinedGuild += DiscordShardedClientOnJoinedGuild;
            DiscordShardedClient.LeftGuild += DiscordShardedClientOnLeftGuild;
            DiscordShardedClient.GuildAvailable += DiscordShardedClientOnGuildAvailable;
            DiscordShardedClient.GuildUnavailable += DiscordShardedClientOnGuildUnavailable;
            DiscordShardedClient.GuildMembersDownloaded += DiscordShardedClientOnGuildMembersDownloaded;
            DiscordShardedClient.GuildUpdated += DiscordShardedClientOnGuildUpdated;
            DiscordShardedClient.UserJoined += DiscordShardedClientOnUserJoined;
            DiscordShardedClient.UserLeft += DiscordShardedClientOnUserLeft;
            DiscordShardedClient.UserBanned += DiscordShardedClientOnUserBanned;
            DiscordShardedClient.UserUnbanned += DiscordShardedClientOnUserUnbanned;
            DiscordShardedClient.UserUpdated += DiscordShardedClientOnUserUpdated;
            DiscordShardedClient.GuildMemberUpdated += DiscordShardedClientOnGuildMemberUpdated;
            DiscordShardedClient.UserVoiceStateUpdated += DiscordShardedClientOnUserVoiceStateUpdated;
            DiscordShardedClient.CurrentUserUpdated += DiscordShardedClientOnCurrentUserUpdated;
            DiscordShardedClient.UserIsTyping += DiscordShardedClientOnUserIsTyping;
            DiscordShardedClient.RecipientAdded += DiscordShardedClientOnRecipientAdded;
            DiscordShardedClient.RecipientRemoved += DiscordShardedClientOnRecipientRemoved;
            DiscordShardedClient.ShardConnected += DiscordShardedClientOnShardConnected;
            DiscordShardedClient.ShardDisconnected += DiscordShardedClientOnShardDisconnected;
            DiscordShardedClient.ShardReady += DiscordShardedClientOnShardReady;
            DiscordShardedClient.ShardLatencyUpdated += DiscordShardedClientOnShardLatencyUpdated;
        }

        private void RemoveListener()
        {
            DiscordShardedClient.ShardLatencyUpdated -= DiscordShardedClientOnShardLatencyUpdated;
            DiscordShardedClient.ShardReady -= DiscordShardedClientOnShardReady;
            DiscordShardedClient.ShardDisconnected -= DiscordShardedClientOnShardDisconnected;
            DiscordShardedClient.ShardConnected -= DiscordShardedClientOnShardConnected;
            DiscordShardedClient.RecipientRemoved -= DiscordShardedClientOnRecipientRemoved;
            DiscordShardedClient.RecipientAdded -= DiscordShardedClientOnRecipientAdded;
            DiscordShardedClient.UserIsTyping -= DiscordShardedClientOnUserIsTyping;
            DiscordShardedClient.CurrentUserUpdated -= DiscordShardedClientOnCurrentUserUpdated;
            DiscordShardedClient.UserVoiceStateUpdated -= DiscordShardedClientOnUserVoiceStateUpdated;
            DiscordShardedClient.GuildMemberUpdated -= DiscordShardedClientOnGuildMemberUpdated;
            DiscordShardedClient.UserUpdated -= DiscordShardedClientOnUserUpdated;
            DiscordShardedClient.UserUnbanned -= DiscordShardedClientOnUserUnbanned;
            DiscordShardedClient.UserBanned -= DiscordShardedClientOnUserBanned;
            DiscordShardedClient.UserLeft -= DiscordShardedClientOnUserLeft;
            DiscordShardedClient.UserJoined -= DiscordShardedClientOnUserJoined;
            DiscordShardedClient.GuildUpdated -= DiscordShardedClientOnGuildUpdated;
            DiscordShardedClient.GuildMembersDownloaded -= DiscordShardedClientOnGuildMembersDownloaded;
            DiscordShardedClient.GuildUnavailable -= DiscordShardedClientOnGuildUnavailable;
            DiscordShardedClient.GuildAvailable -= DiscordShardedClientOnGuildAvailable;
            DiscordShardedClient.LeftGuild -= DiscordShardedClientOnLeftGuild;
            DiscordShardedClient.JoinedGuild -= DiscordShardedClientOnJoinedGuild;
            DiscordShardedClient.RoleUpdated -= DiscordShardedClientOnRoleUpdated;
            DiscordShardedClient.RoleDeleted -= DiscordShardedClientOnRoleDeleted;
            DiscordShardedClient.RoleCreated -= DiscordShardedClientOnRoleCreated;
            DiscordShardedClient.ReactionsCleared -= DiscordShardedClientOnReactionsCleared;
            DiscordShardedClient.ReactionRemoved -= DiscordShardedClientOnReactionRemoved;
            DiscordShardedClient.ReactionAdded -= DiscordShardedClientOnReactionAdded;
            DiscordShardedClient.MessageUpdated -= DiscordShardedClientOnMessageUpdated;
            DiscordShardedClient.MessageDeleted -= DiscordShardedClientOnMessageDeleted;
            DiscordShardedClient.MessageReceived -= DiscordShardedClientOnMessageReceived;
            DiscordShardedClient.ChannelUpdated -= DiscordShardedClientOnChannelUpdated;
            DiscordShardedClient.ChannelDestroyed -= DiscordShardedClientOnChannelDestroyed;
            DiscordShardedClient.ChannelCreated -= DiscordShardedClientOnChannelCreated;
            DiscordShardedClient.LoggedOut -= DiscordShardedClientOnLoggedOut;
            DiscordShardedClient.LoggedIn -= DiscordShardedClientOnLoggedIn;
            DiscordShardedClient.Log -= DiscordShardedClientOnLog;
        }

        private async Task DiscordShardedClientOnLog(LogMessage arg)
        {
            if (Log != null)
                await Log.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnLoggedIn()
        {
            if (LoggedIn != null)
                await LoggedIn.Invoke(DiscordShardedClient).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnLoggedOut()
        {
            if (LoggedOut != null)
                await LoggedOut.Invoke(DiscordShardedClient).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnChannelCreated(SocketChannel arg)
        {
            if (ChannelCreated != null)
                await ChannelCreated.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnChannelDestroyed(SocketChannel arg)
        {
            if (ChannelDestroyed != null)
                await ChannelDestroyed.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            if (ChannelUpdated != null)
                await ChannelUpdated.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnMessageReceived(SocketMessage arg)
        {
            if (MessageReceived != null)
                await MessageReceived.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnMessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (MessageDeleted != null)
                await MessageDeleted.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnMessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            if (MessageUpdated != null)
                await MessageUpdated.Invoke(DiscordShardedClient, arg1, arg2, arg3).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (ReactionAdded != null)
                await ReactionAdded.Invoke(DiscordShardedClient, arg1, arg2, arg3).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (ReactionRemoved != null)
                await ReactionRemoved.Invoke(DiscordShardedClient, arg1, arg2, arg3).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnReactionsCleared(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (ReactionsCleared != null)
                await ReactionsCleared.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnRoleCreated(SocketRole arg)
        {
            if (RoleCreated != null)
                await RoleCreated.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnRoleDeleted(SocketRole arg)
        {
            if (RoleDeleted != null)
                await RoleDeleted.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnRoleUpdated(SocketRole arg1, SocketRole arg2)
        {
            if (RoleUpdated != null)
                await RoleUpdated.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnJoinedGuild(SocketGuild arg)
        {
            if (JoinedGuild != null)
                await JoinedGuild.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnLeftGuild(SocketGuild arg)
        {
            if (LeftGuild != null)
                await LeftGuild.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnGuildAvailable(SocketGuild arg)
        {
            if (GuildAvailable != null)
                await GuildAvailable.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnGuildUnavailable(SocketGuild arg)
        {
            if (GuildUnavailable != null)
                await GuildUnavailable.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnGuildMembersDownloaded(SocketGuild arg)
        {
            if (GuildMembersDownloaded != null)
                await GuildMembersDownloaded.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnGuildUpdated(SocketGuild arg1, SocketGuild arg2)
        {
            if (GuildUpdated != null)
                await GuildUpdated.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnUserJoined(SocketGuildUser arg)
        {
            if (UserJoined != null)
                await UserJoined.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnUserLeft(SocketGuildUser arg)
        {
            if (UserLeft != null)
                await UserLeft.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnUserBanned(SocketUser arg1, SocketGuild arg2)
        {
            if (UserBanned != null)
                await UserBanned.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnUserUnbanned(SocketUser arg1, SocketGuild arg2)
        {
            if (UserUnbanned != null)
                await UserUnbanned.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnUserUpdated(SocketUser arg1, SocketUser arg2)
        {
            if (UserUpdated != null)
                await UserUpdated.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnGuildMemberUpdated(SocketGuildUser arg1, SocketGuildUser arg2)
        {
            if (GuildMemberUpdated != null)
                await GuildMemberUpdated.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnUserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            if (UserVoiceStateUpdated != null)
                await UserVoiceStateUpdated.Invoke(DiscordShardedClient, arg1, arg2, arg3).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnCurrentUserUpdated(SocketSelfUser arg1, SocketSelfUser arg2)
        {
            if (CurrentUserUpdated != null)
                await CurrentUserUpdated.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnUserIsTyping(SocketUser arg1, ISocketMessageChannel arg2)
        {
            if (UserIsTyping != null)
                await UserIsTyping.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnRecipientAdded(SocketGroupUser arg)
        {
            if (RecipientAdded != null)
                await RecipientAdded.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnRecipientRemoved(SocketGroupUser arg)
        {
            if (RecipientRemoved != null)
                await RecipientRemoved.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnShardConnected(DiscordSocketClient arg)
        {
            if (ShardConnected != null)
                await ShardConnected.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnShardDisconnected(Exception arg1, DiscordSocketClient arg2)
        {
            if (ShardDisconnected != null)
                await ShardDisconnected.Invoke(DiscordShardedClient, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnShardReady(DiscordSocketClient arg)
        {
            if (ShardReady != null)
                await ShardReady.Invoke(DiscordShardedClient, arg).ConfigureAwait(false);
        }

        private async Task DiscordShardedClientOnShardLatencyUpdated(int arg1, int arg2, DiscordSocketClient arg3)
        {
            if (ShardLatencyUpdated != null)
                await ShardLatencyUpdated.Invoke(DiscordShardedClient, arg1, arg2, arg3).ConfigureAwait(false);
        }

        public void Dispose()
        {
            DiscordShardedClient?.Dispose();
        }
    }
}