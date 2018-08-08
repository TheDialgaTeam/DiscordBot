using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TheDialgaTeam.Discord.Bot.Module
{
    public abstract class ModuleHelper : ModuleBase<SocketCommandContext>
    {
        protected override async Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            if (Context.Message.Channel is SocketDMChannel || Context.Message.Channel is SocketGroupChannel)
                return await Context.Channel.SendMessageAsync(message, isTTS, embed, options);

            var perms = Context.Guild.GetUser(Context.Client.CurrentUser.Id).GetPermissions(Context.Guild.GetChannel(Context.Channel.Id));

            if (perms.SendMessages)
                return await Context.Channel.SendMessageAsync(message, isTTS, embed, options);

            return null;
        }

        protected override async void AfterExecute(CommandInfo command)
        {
            if (Context.Message.Channel is SocketDMChannel || Context.Message.Channel is SocketGroupChannel)
                return;

            var perms = Context.Guild.GetUser(Context.Client.CurrentUser.Id).GetPermissions(Context.Guild.GetChannel(Context.Channel.Id));

            if (perms.ManageMessages)
                await Context.Message.DeleteAsync();
        }

        protected async Task<IUserMessage> ReplyDMAsync(string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            if (Context.Message.Channel is SocketDMChannel || Context.Message.Channel is SocketGroupChannel)
                return await ReplyAsync(text, isTTS, embed, options);

            var dmChannel = await Context.Message.Author.GetOrCreateDMChannelAsync();
            return await dmChannel.SendMessageAsync(text, isTTS, embed, options);
        }
    }
}