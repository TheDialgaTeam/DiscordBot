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

        event Func<IDiscordSocketClientModel, SocketGuild, SocketGuild, Task> GuildUpdated;

        event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;

        event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;

        event Func<IDiscordSocketClientModel, SocketRole, Task> RoleCreated;

        event Func<IDiscordSocketClientModel, SocketRole, Task> RoleDeleted;

        event Func<IDiscordSocketClientModel, SocketRole, SocketRole, Task> RoleUpdated;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> JoinedGuild;

        event Func<IDiscordSocketClientModel, SocketUser, ISocketMessageChannel, Task> UserIsTyping;

        event Func<IDiscordSocketClientModel, SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;

        event Func<IDiscordSocketClientModel, SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;

        event Func<IDiscordSocketClientModel, SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;

        event Func<IDiscordSocketClientModel, SocketUser, SocketUser, Task> UserUpdated;

        event Func<IDiscordSocketClientModel, SocketUser, SocketGuild, Task> UserUnbanned;

        event Func<IDiscordSocketClientModel, SocketUser, SocketGuild, Task> UserBanned;

        event Func<IDiscordSocketClientModel, SocketGuildUser, Task> UserLeft;

        event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> LeftGuild;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildAvailable;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildUnavailable;

        event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildMembersDownloaded;

        event Func<IDiscordSocketClientModel, SocketGuildUser, Task> UserJoined;

        event Func<IDiscordSocketClientModel, Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;

        event Func<IDiscordSocketClientModel, int, int, Task> LatencyUpdated;

        event Func<IDiscordSocketClientModel, SocketMessage, Task> MessageReceived;

        event Func<IDiscordSocketClientModel, Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;

        event Func<IDiscordSocketClientModel, Task> Connected;

        event Func<IDiscordSocketClientModel, Exception, Task> Disconnected;

        event Func<IDiscordSocketClientModel, Task> Ready;

        event Func<IDiscordSocketClientModel, SocketGroupUser, Task> RecipientRemoved;

        event Func<IDiscordSocketClientModel, SocketChannel, Task> ChannelCreated;

        event Func<IDiscordSocketClientModel, SocketChannel, Task> ChannelDestroyed;

        event Func<IDiscordSocketClientModel, SocketChannel, SocketChannel, Task> ChannelUpdated;

        event Func<IDiscordSocketClientModel, SocketGroupUser, Task> RecipientAdded;

        DiscordSocketClient DiscordSocketClient { get; }

        IDiscordAppModel DiscordAppModel { get; }

        Task StartListening();

        Task StopListening();
    }

    internal sealed class DiscordSocketClientModel : IDiscordSocketClientModel
    {
        public event Func<IDiscordSocketClientModel, LogMessage, Task> Log;

        public event Func<IDiscordSocketClientModel, Task> LoggedIn;

        public event Func<IDiscordSocketClientModel, Task> LoggedOut;

        public event Func<IDiscordSocketClientModel, SocketGuild, SocketGuild, Task> GuildUpdated;

        public event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;

        public event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;

        public event Func<IDiscordSocketClientModel, SocketRole, Task> RoleCreated;

        public event Func<IDiscordSocketClientModel, SocketRole, Task> RoleDeleted;

        public event Func<IDiscordSocketClientModel, SocketRole, SocketRole, Task> RoleUpdated;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> JoinedGuild;

        public event Func<IDiscordSocketClientModel, SocketUser, ISocketMessageChannel, Task> UserIsTyping;

        public event Func<IDiscordSocketClientModel, SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;

        public event Func<IDiscordSocketClientModel, SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;

        public event Func<IDiscordSocketClientModel, SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;

        public event Func<IDiscordSocketClientModel, SocketUser, SocketUser, Task> UserUpdated;

        public event Func<IDiscordSocketClientModel, SocketUser, SocketGuild, Task> UserUnbanned;

        public event Func<IDiscordSocketClientModel, SocketUser, SocketGuild, Task> UserBanned;

        public event Func<IDiscordSocketClientModel, SocketGuildUser, Task> UserLeft;

        public event Func<IDiscordSocketClientModel, Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> LeftGuild;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildAvailable;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildUnavailable;

        public event Func<IDiscordSocketClientModel, SocketGuild, Task> GuildMembersDownloaded;

        public event Func<IDiscordSocketClientModel, SocketGuildUser, Task> UserJoined;

        public event Func<IDiscordSocketClientModel, Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;

        public event Func<IDiscordSocketClientModel, int, int, Task> LatencyUpdated;

        public event Func<IDiscordSocketClientModel, SocketMessage, Task> MessageReceived;

        public event Func<IDiscordSocketClientModel, Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;

        public event Func<IDiscordSocketClientModel, Task> Connected;

        public event Func<IDiscordSocketClientModel, Exception, Task> Disconnected;

        public event Func<IDiscordSocketClientModel, Task> Ready;

        public event Func<IDiscordSocketClientModel, SocketGroupUser, Task> RecipientRemoved;

        public event Func<IDiscordSocketClientModel, SocketChannel, Task> ChannelCreated;

        public event Func<IDiscordSocketClientModel, SocketChannel, Task> ChannelDestroyed;

        public event Func<IDiscordSocketClientModel, SocketChannel, SocketChannel, Task> ChannelUpdated;

        public event Func<IDiscordSocketClientModel, SocketGroupUser, Task> RecipientAdded;

        public DiscordSocketClient DiscordSocketClient { get; }

        public IDiscordAppModel DiscordAppModel { get; }

        public DiscordSocketClientModel(IDiscordAppModel discordAppModel)
        {
            DiscordAppModel = discordAppModel;
            DiscordSocketClient = new DiscordSocketClient();
        }

        public async Task StartListening()
        {
            while (DiscordSocketClient.LoginState == LoginState.LoggingOut || DiscordSocketClient.ConnectionState == ConnectionState.Disconnecting)
                await Task.Delay(1000);

            if (DiscordSocketClient.LoginState == LoginState.LoggedOut && DiscordSocketClient.ConnectionState == ConnectionState.Disconnected)
            {
                DiscordSocketClient.Log += DiscordSocketClientOnLog;
                DiscordSocketClient.LoggedIn += DiscordSocketClientOnLoggedIn;
                DiscordSocketClient.LoggedOut += DiscordSocketClientOnLoggedOut;
                DiscordSocketClient.GuildUpdated += DiscordSocketClientOnGuildUpdated;
                DiscordSocketClient.ReactionRemoved += DiscordSocketClientOnReactionRemoved;
                DiscordSocketClient.ReactionsCleared += DiscordSocketClientOnReactionsCleared;
                DiscordSocketClient.RoleCreated += DiscordSocketClientOnRoleCreated;
                DiscordSocketClient.RoleDeleted += DiscordSocketClientOnRoleDeleted;
                DiscordSocketClient.RoleUpdated += DiscordSocketClientOnRoleUpdated;
                DiscordSocketClient.JoinedGuild += DiscordSocketClientOnJoinedGuild;
                DiscordSocketClient.UserIsTyping += DiscordSocketClientOnUserIsTyping;
                DiscordSocketClient.CurrentUserUpdated += DiscordSocketClientOnCurrentUserUpdated;
                DiscordSocketClient.UserVoiceStateUpdated += DiscordSocketClientOnUserVoiceStateUpdated;
                DiscordSocketClient.GuildMemberUpdated += DiscordSocketClientOnGuildMemberUpdated;
                DiscordSocketClient.UserUpdated += DiscordSocketClientOnUserUpdated;
                DiscordSocketClient.UserUnbanned += DiscordSocketClientOnUserUnbanned;
                DiscordSocketClient.UserBanned += DiscordSocketClientOnUserBanned;
                DiscordSocketClient.UserLeft += DiscordSocketClientOnUserLeft;
                DiscordSocketClient.ReactionAdded += DiscordSocketClientOnReactionAdded;
                DiscordSocketClient.LeftGuild += DiscordSocketClientOnLeftGuild;
                DiscordSocketClient.GuildAvailable += DiscordSocketClientOnGuildAvailable;
                DiscordSocketClient.GuildUnavailable += DiscordSocketClientOnGuildUnavailable;
                DiscordSocketClient.GuildMembersDownloaded += DiscordSocketClientOnGuildMembersDownloaded;
                DiscordSocketClient.UserJoined += DiscordSocketClientOnUserJoined;
                DiscordSocketClient.MessageUpdated += DiscordSocketClientOnMessageUpdated;
                DiscordSocketClient.LatencyUpdated += DiscordSocketClientOnLatencyUpdated;
                DiscordSocketClient.MessageReceived += DiscordSocketClientOnMessageReceived;
                DiscordSocketClient.MessageDeleted += DiscordSocketClientOnMessageDeleted;
                DiscordSocketClient.Connected += DiscordSocketClientOnConnected;
                DiscordSocketClient.Disconnected += DiscordSocketClientOnDisconnected;
                DiscordSocketClient.Ready += DiscordSocketClientOnReady;
                DiscordSocketClient.RecipientRemoved += DiscordSocketClientOnRecipientRemoved;
                DiscordSocketClient.ChannelCreated += DiscordSocketClientOnChannelCreated;
                DiscordSocketClient.ChannelDestroyed += DiscordSocketClientOnChannelDestroyed;
                DiscordSocketClient.ChannelUpdated += DiscordSocketClientOnChannelUpdated;
                DiscordSocketClient.RecipientAdded += DiscordSocketClientOnRecipientAdded;

                await DiscordSocketClient.LoginAsync(TokenType.Bot, DiscordAppModel.GetBotToken());
                await DiscordSocketClient.StartAsync();
            }
        }

        public async Task StopListening()
        {
            while (DiscordSocketClient.LoginState == LoginState.LoggingIn || DiscordSocketClient.ConnectionState == ConnectionState.Connecting)
                await Task.Delay(1000);

            if (DiscordSocketClient.LoginState == LoginState.LoggedIn && DiscordSocketClient.ConnectionState == ConnectionState.Connected)
            {
                await DiscordSocketClient.LogoutAsync();
                await DiscordSocketClient.StopAsync();

                DiscordSocketClient.Log -= DiscordSocketClientOnLog;
                DiscordSocketClient.LoggedIn -= DiscordSocketClientOnLoggedIn;
                DiscordSocketClient.LoggedOut -= DiscordSocketClientOnLoggedOut;
                DiscordSocketClient.GuildUpdated -= DiscordSocketClientOnGuildUpdated;
                DiscordSocketClient.ReactionRemoved -= DiscordSocketClientOnReactionRemoved;
                DiscordSocketClient.ReactionsCleared -= DiscordSocketClientOnReactionsCleared;
                DiscordSocketClient.RoleCreated -= DiscordSocketClientOnRoleCreated;
                DiscordSocketClient.RoleDeleted -= DiscordSocketClientOnRoleDeleted;
                DiscordSocketClient.RoleUpdated -= DiscordSocketClientOnRoleUpdated;
                DiscordSocketClient.JoinedGuild -= DiscordSocketClientOnJoinedGuild;
                DiscordSocketClient.UserIsTyping -= DiscordSocketClientOnUserIsTyping;
                DiscordSocketClient.CurrentUserUpdated -= DiscordSocketClientOnCurrentUserUpdated;
                DiscordSocketClient.UserVoiceStateUpdated -= DiscordSocketClientOnUserVoiceStateUpdated;
                DiscordSocketClient.GuildMemberUpdated -= DiscordSocketClientOnGuildMemberUpdated;
                DiscordSocketClient.UserUpdated -= DiscordSocketClientOnUserUpdated;
                DiscordSocketClient.UserUnbanned -= DiscordSocketClientOnUserUnbanned;
                DiscordSocketClient.UserBanned -= DiscordSocketClientOnUserBanned;
                DiscordSocketClient.UserLeft -= DiscordSocketClientOnUserLeft;
                DiscordSocketClient.ReactionAdded -= DiscordSocketClientOnReactionAdded;
                DiscordSocketClient.LeftGuild -= DiscordSocketClientOnLeftGuild;
                DiscordSocketClient.GuildAvailable -= DiscordSocketClientOnGuildAvailable;
                DiscordSocketClient.GuildUnavailable -= DiscordSocketClientOnGuildUnavailable;
                DiscordSocketClient.GuildMembersDownloaded -= DiscordSocketClientOnGuildMembersDownloaded;
                DiscordSocketClient.UserJoined -= DiscordSocketClientOnUserJoined;
                DiscordSocketClient.MessageUpdated -= DiscordSocketClientOnMessageUpdated;
                DiscordSocketClient.LatencyUpdated -= DiscordSocketClientOnLatencyUpdated;
                DiscordSocketClient.MessageReceived -= DiscordSocketClientOnMessageReceived;
                DiscordSocketClient.MessageDeleted -= DiscordSocketClientOnMessageDeleted;
                DiscordSocketClient.Connected -= DiscordSocketClientOnConnected;
                DiscordSocketClient.Disconnected -= DiscordSocketClientOnDisconnected;
                DiscordSocketClient.Ready -= DiscordSocketClientOnReady;
                DiscordSocketClient.RecipientRemoved -= DiscordSocketClientOnRecipientRemoved;
                DiscordSocketClient.ChannelCreated -= DiscordSocketClientOnChannelCreated;
                DiscordSocketClient.ChannelDestroyed -= DiscordSocketClientOnChannelDestroyed;
                DiscordSocketClient.ChannelUpdated -= DiscordSocketClientOnChannelUpdated;
                DiscordSocketClient.RecipientAdded -= DiscordSocketClientOnRecipientAdded;
            }
        }

        private async Task DiscordSocketClientOnLog(LogMessage logMessage)
        {
            if (Log != null)
                await Log.Invoke(this, logMessage);
        }

        private async Task DiscordSocketClientOnLoggedIn()
        {
            if (LoggedIn != null)
                await LoggedIn.Invoke(this);
        }

        private async Task DiscordSocketClientOnLoggedOut()
        {
            if (LoggedOut != null)
                await LoggedOut.Invoke(this);
        }

        private async Task DiscordSocketClientOnGuildUpdated(SocketGuild socketGuild, SocketGuild guild)
        {
            if (GuildUpdated != null)
                await GuildUpdated.Invoke(this, socketGuild, guild);
        }

        private async Task DiscordSocketClientOnReactionRemoved(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction arg3)
        {
            if (ReactionRemoved != null)
                await ReactionRemoved.Invoke(this, cacheable, socketMessageChannel, arg3);
        }

        private async Task DiscordSocketClientOnReactionsCleared(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel)
        {
            if (ReactionsCleared != null)
                await ReactionsCleared.Invoke(this, cacheable, socketMessageChannel);
        }

        private async Task DiscordSocketClientOnRoleCreated(SocketRole socketRole)
        {
            if (RoleCreated != null)
                await RoleCreated.Invoke(this, socketRole);
        }

        private async Task DiscordSocketClientOnRoleDeleted(SocketRole socketRole)
        {
            if (RoleDeleted != null)
                await RoleDeleted.Invoke(this, socketRole);
        }

        private async Task DiscordSocketClientOnRoleUpdated(SocketRole socketRole, SocketRole role)
        {
            if (RoleUpdated != null)
                await RoleUpdated.Invoke(this, socketRole, role);
        }

        private async Task DiscordSocketClientOnJoinedGuild(SocketGuild socketGuild)
        {
            if (JoinedGuild != null)
                await JoinedGuild.Invoke(this, socketGuild);
        }

        private async Task DiscordSocketClientOnUserIsTyping(SocketUser socketUser, ISocketMessageChannel socketMessageChannel)
        {
            if (UserIsTyping != null)
                await UserIsTyping.Invoke(this, socketUser, socketMessageChannel);
        }

        private async Task DiscordSocketClientOnCurrentUserUpdated(SocketSelfUser socketSelfUser, SocketSelfUser selfUser)
        {
            if (CurrentUserUpdated != null)
                await CurrentUserUpdated.Invoke(this, socketSelfUser, selfUser);
        }

        private async Task DiscordSocketClientOnUserVoiceStateUpdated(SocketUser socketUser, SocketVoiceState socketVoiceState, SocketVoiceState arg3)
        {
            if (UserVoiceStateUpdated != null)
                await UserVoiceStateUpdated.Invoke(this, socketUser, socketVoiceState, arg3);
        }

        private async Task DiscordSocketClientOnGuildMemberUpdated(SocketGuildUser socketGuildUser, SocketGuildUser guildUser)
        {
            if (GuildMemberUpdated != null)
                await GuildMemberUpdated.Invoke(this, socketGuildUser, guildUser);
        }

        private async Task DiscordSocketClientOnUserUpdated(SocketUser socketUser, SocketUser user)
        {
            if (UserUpdated != null)
                await UserUpdated.Invoke(this, socketUser, user);
        }

        private async Task DiscordSocketClientOnUserUnbanned(SocketUser socketUser, SocketGuild socketGuild)
        {
            if (UserUnbanned != null)
                await UserUnbanned.Invoke(this, socketUser, socketGuild);
        }

        private async Task DiscordSocketClientOnUserBanned(SocketUser socketUser, SocketGuild socketGuild)
        {
            if (UserBanned != null)
                await UserBanned.Invoke(this, socketUser, socketGuild);
        }

        private async Task DiscordSocketClientOnUserLeft(SocketGuildUser socketGuildUser)
        {
            if (UserLeft != null)
                await UserLeft.Invoke(this, socketGuildUser);
        }

        private async Task DiscordSocketClientOnReactionAdded(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction arg3)
        {
            if (ReactionAdded != null)
                await ReactionAdded.Invoke(this, cacheable, socketMessageChannel, arg3);
        }

        private async Task DiscordSocketClientOnLeftGuild(SocketGuild socketGuild)
        {
            if (LeftGuild != null)
                await LeftGuild.Invoke(this, socketGuild);
        }

        private async Task DiscordSocketClientOnGuildAvailable(SocketGuild socketGuild)
        {
            if (GuildAvailable != null)
                await GuildAvailable.Invoke(this, socketGuild);
        }

        private async Task DiscordSocketClientOnGuildUnavailable(SocketGuild socketGuild)
        {
            if (GuildUnavailable != null)
                await GuildUnavailable.Invoke(this, socketGuild);
        }

        private async Task DiscordSocketClientOnGuildMembersDownloaded(SocketGuild socketGuild)
        {
            if (GuildMembersDownloaded != null)
                await GuildMembersDownloaded.Invoke(this, socketGuild);
        }

        private async Task DiscordSocketClientOnUserJoined(SocketGuildUser socketGuildUser)
        {
            if (UserJoined != null)
                await UserJoined.Invoke(this, socketGuildUser);
        }

        private async Task DiscordSocketClientOnMessageUpdated(Cacheable<IMessage, ulong> cacheable, SocketMessage socketMessage, ISocketMessageChannel arg3)
        {
            if (MessageUpdated != null)
                await MessageUpdated.Invoke(this, cacheable, socketMessage, arg3);
        }

        private async Task DiscordSocketClientOnLatencyUpdated(int i, int i1)
        {
            if (LatencyUpdated != null)
                await LatencyUpdated.Invoke(this, i, i1);
        }

        private async Task DiscordSocketClientOnMessageReceived(SocketMessage socketMessage)
        {
            if (MessageReceived != null)
                await MessageReceived.Invoke(this, socketMessage);
        }

        private async Task DiscordSocketClientOnMessageDeleted(Cacheable<IMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel)
        {
            if (MessageDeleted != null)
                await MessageDeleted.Invoke(this, cacheable, socketMessageChannel);
        }

        private async Task DiscordSocketClientOnConnected()
        {
            if (Connected != null)
                await Connected.Invoke(this);
        }

        private async Task DiscordSocketClientOnDisconnected(Exception exception)
        {
            if (Disconnected != null)
                await Disconnected.Invoke(this, exception);
        }

        private async Task DiscordSocketClientOnReady()
        {
            if (Ready != null)
                await Ready.Invoke(this);
        }

        private async Task DiscordSocketClientOnRecipientRemoved(SocketGroupUser socketGroupUser)
        {
            if (RecipientRemoved != null)
                await RecipientRemoved.Invoke(this, socketGroupUser);
        }

        private async Task DiscordSocketClientOnChannelCreated(SocketChannel socketChannel)
        {
            if (ChannelCreated != null)
                await ChannelCreated.Invoke(this, socketChannel);
        }

        private async Task DiscordSocketClientOnChannelDestroyed(SocketChannel socketChannel)
        {
            if (ChannelDestroyed != null)
                await ChannelDestroyed.Invoke(this, socketChannel);
        }

        private async Task DiscordSocketClientOnChannelUpdated(SocketChannel socketChannel, SocketChannel channel)
        {
            if (ChannelUpdated != null)
                await ChannelUpdated.Invoke(this, socketChannel, channel);
        }

        private async Task DiscordSocketClientOnRecipientAdded(SocketGroupUser socketGroupUser)
        {
            if (RecipientAdded != null)
                await RecipientAdded.Invoke(this, socketGroupUser);
        }
    }
}