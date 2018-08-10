using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Module
{
    public abstract class ModuleHelper : ModuleBase<ShardedCommandContext>
    {
        protected SQLiteService SqliteService { get; }

        protected ModuleHelper(SQLiteService sqliteService)
        {
            SqliteService = sqliteService;
        }

        protected override async Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            if (Context.Message.Channel is SocketDMChannel || Context.Message.Channel is SocketGroupChannel)
                return await Context.Channel.SendMessageAsync(message, isTTS, embed, options).ConfigureAwait(false);

            if (GetChannelPermissions().SendMessages)
                return await Context.Channel.SendMessageAsync(message, isTTS, embed, options).ConfigureAwait(false);

            return null;
        }

        protected override async void AfterExecute(CommandInfo command)
        {
            if (Context.Message.Channel is SocketDMChannel || Context.Message.Channel is SocketGroupChannel)
                return;

            var discordGuild = await SqliteService.GetOrCreateDiscordGuildTableAsync(Context.Client.CurrentUser.Id, Context.Guild.Id).ConfigureAwait(false);

            if ((discordGuild?.DeleteCommandAfterUse ?? false) && GetChannelPermissions().ManageMessages)
                await Context.Message.DeleteAsync().ConfigureAwait(false);
        }

        protected async Task<IUserMessage> ReplyDMAsync(string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            if (Context.Message.Channel is SocketDMChannel || Context.Message.Channel is SocketGroupChannel)
                return await ReplyAsync(text, isTTS, embed, options).ConfigureAwait(false);

            var dmChannel = await Context.Message.Author.GetOrCreateDMChannelAsync().ConfigureAwait(false);
            return await dmChannel.SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
        }

        protected ChannelPermissions GetChannelPermissions(ulong? channelId = null)
        {
            return Context.Guild.GetUser(Context.Client.CurrentUser.Id).GetPermissions(Context.Guild.GetChannel(channelId ?? Context.Channel.Id));
        }
    }
}