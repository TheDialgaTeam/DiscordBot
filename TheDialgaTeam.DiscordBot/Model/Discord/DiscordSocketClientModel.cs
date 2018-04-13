using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;

namespace TheDialgaTeam.DiscordBot.Model.Discord
{
    public interface IDiscordSocketClientModel
    {
        event Func<IDiscordSocketClientModel, LogMessage, Task> Log;

        event Func<IDiscordSocketClientModel, Task> LoggedIn;

        event Func<IDiscordSocketClientModel, Task> LoggedOut;

        event Func<IDiscordSocketClientModel, SocketChannel, Task> ChannelCreated;

        event Func<IDiscordSocketClientModel, SocketChannel, Task> ChannelDestroyed;

        event Func<IDiscordSocketClientModel, SocketChannel, SocketChannel, Task> ChannelUpdated;

        event Func<IDiscordSocketClientModel, SocketMessage, Task> MessageReceived;

        event Func<IDiscordSocketClientModel, Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;

        event Func<IDiscordSocketClientModel, Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;

        event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;

        event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;

        event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;

        event Func<IDiscordSocketClientModel, SocketRole, Task> RoleCreated;

        event Func<IDiscordSocketClientModel, SocketRole, Task> RoleDeleted;

        event Func<IDiscordSocketClientModel, SocketRole, SocketRole, Task> RoleUpdated;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> JoinedGuild;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> LeftGuild;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildAvailable;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildUnavailable;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildMembersDownloaded;

        event Func<IDiscordSocketClientModel, SocketGuild, SocketGuild, Task> GuildUpdated;

        event Func<IDiscordSocketClientModel, SocketGuildUser, Task> UserJoined;

        event Func<IDiscordSocketClientModel, SocketGuildUser, Task> UserLeft;

        event Func<IDiscordSocketClientModel, SocketUser, SocketGuild, Task> UserBanned;

        event Func<IDiscordSocketClientModel, SocketUser, SocketGuild, Task> UserUnbanned;

        event Func<IDiscordSocketClientModel, SocketUser, SocketUser, Task> UserUpdated;

        event Func<IDiscordSocketClientModel, SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;

        event Func<IDiscordSocketClientModel, SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;

        event Func<IDiscordSocketClientModel, SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;

        event Func<IDiscordSocketClientModel, SocketUser, ISocketMessageChannel, Task> UserIsTyping;

        event Func<IDiscordSocketClientModel, SocketGroupUser, Task> RecipientAdded;

        event Func<IDiscordSocketClientModel, SocketGroupUser, Task> RecipientRemoved;

        event Func<IDiscordSocketClientModel, Task> Connected;

        event Func<IDiscordSocketClientModel, Exception, Task> Disconnected;

        event Func<IDiscordSocketClientModel, Task> Ready;

        event Func<IDiscordSocketClientModel, int, int, Task> LatencyUpdated;

        DiscordSocketClient DiscordSocketClient { get; }

        IDiscordAppModel DiscordAppModel { get; }

        Task StartListeningAsync();

        Task StopListeningAsync();
    }

    internal sealed class DiscordSocketClientModel : IDiscordSocketClientModel
    {
        public event Func<IDiscordSocketClientModel, LogMessage, Task> Log;

        public event Func<IDiscordSocketClientModel, Task> LoggedIn;

        public event Func<IDiscordSocketClientModel, Task> LoggedOut;

        public event Func<IDiscordSocketClientModel, SocketChannel, Task> ChannelCreated;

        public event Func<IDiscordSocketClientModel, SocketChannel, Task> ChannelDestroyed;

        public event Func<IDiscordSocketClientModel, SocketChannel, SocketChannel, Task> ChannelUpdated;

        public event Func<IDiscordSocketClientModel, SocketMessage, Task> MessageReceived;

        public event Func<IDiscordSocketClientModel, Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;

        public event Func<IDiscordSocketClientModel, Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;

        public event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;

        public event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;

        public event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;

        public event Func<IDiscordSocketClientModel, SocketRole, Task> RoleCreated;

        public event Func<IDiscordSocketClientModel, SocketRole, Task> RoleDeleted;

        public event Func<IDiscordSocketClientModel, SocketRole, SocketRole, Task> RoleUpdated;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> JoinedGuild;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> LeftGuild;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildAvailable;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildUnavailable;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildMembersDownloaded;

        public event Func<IDiscordSocketClientModel, SocketGuild, SocketGuild, Task> GuildUpdated;

        public event Func<IDiscordSocketClientModel, SocketGuildUser, Task> UserJoined;

        public event Func<IDiscordSocketClientModel, SocketGuildUser, Task> UserLeft;

        public event Func<IDiscordSocketClientModel, SocketUser, SocketGuild, Task> UserBanned;

        public event Func<IDiscordSocketClientModel, SocketUser, SocketGuild, Task> UserUnbanned;

        public event Func<IDiscordSocketClientModel, SocketUser, SocketUser, Task> UserUpdated;

        public event Func<IDiscordSocketClientModel, SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;

        public event Func<IDiscordSocketClientModel, SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;

        public event Func<IDiscordSocketClientModel, SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;

        public event Func<IDiscordSocketClientModel, SocketUser, ISocketMessageChannel, Task> UserIsTyping;

        public event Func<IDiscordSocketClientModel, SocketGroupUser, Task> RecipientAdded;

        public event Func<IDiscordSocketClientModel, SocketGroupUser, Task> RecipientRemoved;

        public event Func<IDiscordSocketClientModel, Task> Connected;

        public event Func<IDiscordSocketClientModel, Exception, Task> Disconnected;

        public event Func<IDiscordSocketClientModel, Task> Ready;

        public event Func<IDiscordSocketClientModel, int, int, Task> LatencyUpdated;

        public DiscordSocketClient DiscordSocketClient { get; }

        public IDiscordAppModel DiscordAppModel { get; }

        public DiscordSocketClientModel(IDiscordAppModel discordAppModel)
        {
            DiscordAppModel = discordAppModel;
            DiscordSocketClient = new DiscordSocketClient();
        }

        public async Task StartListeningAsync()
        {
            while (DiscordSocketClient.LoginState == LoginState.LoggingOut || DiscordSocketClient.ConnectionState == ConnectionState.Disconnecting)
                await Task.Delay(1000);

            if (DiscordSocketClient.LoginState == LoginState.LoggedOut && DiscordSocketClient.ConnectionState == ConnectionState.Disconnected)
            {
                DiscordSocketClient.Log += DiscordSocketClientOnLog;
                DiscordSocketClient.LoggedIn += DiscordSocketClientOnLoggedIn;
                DiscordSocketClient.LoggedOut += DiscordSocketClientOnLoggedOut;
                DiscordSocketClient.ChannelCreated += DiscordSocketClientOnChannelCreated;
                DiscordSocketClient.ChannelDestroyed += DiscordSocketClientOnChannelDestroyed;
                DiscordSocketClient.ChannelUpdated += DiscordSocketClientOnChannelUpdated;
                DiscordSocketClient.MessageReceived += DiscordSocketClientOnMessageReceived;
                DiscordSocketClient.MessageDeleted += DiscordSocketClientOnMessageDeleted;
                DiscordSocketClient.MessageUpdated += DiscordSocketClientOnMessageUpdated;
                DiscordSocketClient.ReactionAdded += DiscordSocketClientOnReactionAdded;
                DiscordSocketClient.ReactionRemoved += DiscordSocketClientOnReactionRemoved;
                DiscordSocketClient.ReactionsCleared += DiscordSocketClientOnReactionsCleared;
                DiscordSocketClient.RoleCreated += DiscordSocketClientOnRoleCreated;
                DiscordSocketClient.RoleDeleted += DiscordSocketClientOnRoleDeleted;
                DiscordSocketClient.RoleUpdated += DiscordSocketClientOnRoleUpdated;
                DiscordSocketClient.JoinedGuild += DiscordSocketClientOnJoinedGuild;
                DiscordSocketClient.LeftGuild += DiscordSocketClientOnLeftGuild;
                DiscordSocketClient.GuildAvailable += DiscordSocketClientOnGuildAvailable;
                DiscordSocketClient.GuildUnavailable += DiscordSocketClientOnGuildUnavailable;
                DiscordSocketClient.GuildMembersDownloaded += DiscordSocketClientOnGuildMembersDownloaded;
                DiscordSocketClient.GuildUpdated += DiscordSocketClientOnGuildUpdated;
                DiscordSocketClient.UserJoined += DiscordSocketClientOnUserJoined;
                DiscordSocketClient.UserLeft += DiscordSocketClientOnUserLeft;
                DiscordSocketClient.UserBanned += DiscordSocketClientOnUserBanned;
                DiscordSocketClient.UserUnbanned += DiscordSocketClientOnUserUnbanned;
                DiscordSocketClient.UserUpdated += DiscordSocketClientOnUserUpdated;
                DiscordSocketClient.GuildMemberUpdated += DiscordSocketClientOnGuildMemberUpdated;
                DiscordSocketClient.UserVoiceStateUpdated += DiscordSocketClientOnUserVoiceStateUpdated;
                DiscordSocketClient.CurrentUserUpdated += DiscordSocketClientOnCurrentUserUpdated;
                DiscordSocketClient.UserIsTyping += DiscordSocketClientOnUserIsTyping;
                DiscordSocketClient.RecipientAdded += DiscordSocketClientOnRecipientAdded;
                DiscordSocketClient.RecipientRemoved += DiscordSocketClientOnRecipientRemoved;
                DiscordSocketClient.Connected += DiscordSocketClientOnConnected;
                DiscordSocketClient.Disconnected += DiscordSocketClientOnDisconnected;
                DiscordSocketClient.Ready += DiscordSocketClientOnReady;
                DiscordSocketClient.LatencyUpdated += DiscordSocketClientOnLatencyUpdated;

                await DiscordSocketClient.LoginAsync(TokenType.Bot, DiscordAppModel.GetBotToken());
                await DiscordSocketClient.StartAsync();
            }
        }

        public async Task StopListeningAsync()
        {
            while (DiscordSocketClient.LoginState == LoginState.LoggingIn || DiscordSocketClient.ConnectionState == ConnectionState.Connecting)
                await Task.Delay(1000);

            if (DiscordSocketClient.LoginState == LoginState.LoggedIn && DiscordSocketClient.ConnectionState == ConnectionState.Connected)
            {
                await DiscordSocketClient.LogoutAsync();
                await DiscordSocketClient.StopAsync();

                DiscordSocketClient.LatencyUpdated -= DiscordSocketClientOnLatencyUpdated;
                DiscordSocketClient.Ready -= DiscordSocketClientOnReady;
                DiscordSocketClient.Disconnected -= DiscordSocketClientOnDisconnected;
                DiscordSocketClient.Connected -= DiscordSocketClientOnConnected;
                DiscordSocketClient.RecipientRemoved -= DiscordSocketClientOnRecipientRemoved;
                DiscordSocketClient.RecipientAdded -= DiscordSocketClientOnRecipientAdded;
                DiscordSocketClient.UserIsTyping -= DiscordSocketClientOnUserIsTyping;
                DiscordSocketClient.CurrentUserUpdated -= DiscordSocketClientOnCurrentUserUpdated;
                DiscordSocketClient.UserVoiceStateUpdated -= DiscordSocketClientOnUserVoiceStateUpdated;
                DiscordSocketClient.GuildMemberUpdated -= DiscordSocketClientOnGuildMemberUpdated;
                DiscordSocketClient.UserUpdated -= DiscordSocketClientOnUserUpdated;
                DiscordSocketClient.UserUnbanned -= DiscordSocketClientOnUserUnbanned;
                DiscordSocketClient.UserBanned -= DiscordSocketClientOnUserBanned;
                DiscordSocketClient.UserLeft -= DiscordSocketClientOnUserLeft;
                DiscordSocketClient.UserJoined -= DiscordSocketClientOnUserJoined;
                DiscordSocketClient.GuildUpdated -= DiscordSocketClientOnGuildUpdated;
                DiscordSocketClient.GuildMembersDownloaded -= DiscordSocketClientOnGuildMembersDownloaded;
                DiscordSocketClient.GuildUnavailable -= DiscordSocketClientOnGuildUnavailable;
                DiscordSocketClient.GuildAvailable -= DiscordSocketClientOnGuildAvailable;
                DiscordSocketClient.LeftGuild -= DiscordSocketClientOnLeftGuild;
                DiscordSocketClient.JoinedGuild -= DiscordSocketClientOnJoinedGuild;
                DiscordSocketClient.RoleUpdated -= DiscordSocketClientOnRoleUpdated;
                DiscordSocketClient.RoleDeleted -= DiscordSocketClientOnRoleDeleted;
                DiscordSocketClient.RoleCreated -= DiscordSocketClientOnRoleCreated;
                DiscordSocketClient.ReactionsCleared -= DiscordSocketClientOnReactionsCleared;
                DiscordSocketClient.ReactionRemoved -= DiscordSocketClientOnReactionRemoved;
                DiscordSocketClient.ReactionAdded -= DiscordSocketClientOnReactionAdded;
                DiscordSocketClient.MessageUpdated -= DiscordSocketClientOnMessageUpdated;
                DiscordSocketClient.MessageDeleted -= DiscordSocketClientOnMessageDeleted;
                DiscordSocketClient.MessageReceived -= DiscordSocketClientOnMessageReceived;
                DiscordSocketClient.ChannelUpdated -= DiscordSocketClientOnChannelUpdated;
                DiscordSocketClient.ChannelDestroyed -= DiscordSocketClientOnChannelDestroyed;
                DiscordSocketClient.ChannelCreated -= DiscordSocketClientOnChannelCreated;
                DiscordSocketClient.LoggedOut -= DiscordSocketClientOnLoggedOut;
                DiscordSocketClient.LoggedIn -= DiscordSocketClientOnLoggedIn;
                DiscordSocketClient.Log -= DiscordSocketClientOnLog;
            }
        }

        private async Task DiscordSocketClientOnLog(LogMessage arg)
        {
            if (Log != null)
                await Log.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnLoggedIn()
        {
            if (LoggedIn != null)
                await LoggedIn.Invoke(this).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnLoggedOut()
        {
            if (LoggedOut != null)
                await LoggedOut.Invoke(this).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnChannelCreated(SocketChannel arg)
        {
            if (ChannelCreated != null)
                await ChannelCreated.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnChannelDestroyed(SocketChannel arg)
        {
            if (ChannelDestroyed != null)
                await ChannelDestroyed.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            if (ChannelUpdated != null)
                await ChannelUpdated.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnMessageReceived(SocketMessage arg)
        {
            if (MessageReceived != null)
                await MessageReceived.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnMessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (MessageDeleted != null)
                await MessageDeleted.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnMessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            if (MessageUpdated != null)
                await MessageUpdated.Invoke(this, arg1, arg2, arg3).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (ReactionAdded != null)
                await ReactionAdded.Invoke(this, arg1, arg2, arg3).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (ReactionRemoved != null)
                await ReactionRemoved.Invoke(this, arg1, arg2, arg3).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnReactionsCleared(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (ReactionsCleared != null)
                await ReactionsCleared.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnRoleCreated(SocketRole arg)
        {
            if (RoleCreated != null)
                await RoleCreated.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnRoleDeleted(SocketRole arg)
        {
            if (RoleDeleted != null)
                await RoleDeleted.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnRoleUpdated(SocketRole arg1, SocketRole arg2)
        {
            if (RoleUpdated != null)
                await RoleUpdated.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnJoinedGuild(SocketGuild arg)
        {
            if (JoinedGuild != null)
                await JoinedGuild.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnLeftGuild(SocketGuild arg)
        {
            if (LeftGuild != null)
                await LeftGuild.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnGuildAvailable(SocketGuild arg)
        {
            if (GuildAvailable != null)
                await GuildAvailable.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnGuildUnavailable(SocketGuild arg)
        {
            if (GuildUnavailable != null)
                await GuildUnavailable.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnGuildMembersDownloaded(SocketGuild arg)
        {
            if (GuildMembersDownloaded != null)
                await GuildMembersDownloaded.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnGuildUpdated(SocketGuild arg1, SocketGuild arg2)
        {
            if (GuildUpdated != null)
                await GuildUpdated.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnUserJoined(SocketGuildUser arg)
        {
            if (UserJoined != null)
                await UserJoined.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnUserLeft(SocketGuildUser arg)
        {
            if (UserLeft != null)
                await UserLeft.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnUserBanned(SocketUser arg1, SocketGuild arg2)
        {
            if (UserBanned != null)
                await UserBanned.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnUserUnbanned(SocketUser arg1, SocketGuild arg2)
        {
            if (UserUnbanned != null)
                await UserUnbanned.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnUserUpdated(SocketUser arg1, SocketUser arg2)
        {
            if (UserUpdated != null)
                await UserUpdated.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnGuildMemberUpdated(SocketGuildUser arg1, SocketGuildUser arg2)
        {
            if (GuildMemberUpdated != null)
                await GuildMemberUpdated.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnUserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            if (UserVoiceStateUpdated != null)
                await UserVoiceStateUpdated.Invoke(this, arg1, arg2, arg3).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnCurrentUserUpdated(SocketSelfUser arg1, SocketSelfUser arg2)
        {
            if (CurrentUserUpdated != null)
                await CurrentUserUpdated.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnUserIsTyping(SocketUser arg1, ISocketMessageChannel arg2)
        {
            if (UserIsTyping != null)
                await UserIsTyping.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnRecipientAdded(SocketGroupUser arg)
        {
            if (RecipientAdded != null)
                await RecipientAdded.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnRecipientRemoved(SocketGroupUser arg)
        {
            if (RecipientRemoved != null)
                await RecipientRemoved.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnConnected()
        {
            if (Connected != null)
                await Connected.Invoke(this).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnDisconnected(Exception arg)
        {
            if (Disconnected != null)
                await Disconnected.Invoke(this, arg).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnReady()
        {
            if (Ready != null)
                await Ready.Invoke(this).ConfigureAwait(false);
        }

        private async Task DiscordSocketClientOnLatencyUpdated(int arg1, int arg2)
        {
            if (LatencyUpdated != null)
                await LatencyUpdated.Invoke(this, arg1, arg2).ConfigureAwait(false);
        }
    }
}