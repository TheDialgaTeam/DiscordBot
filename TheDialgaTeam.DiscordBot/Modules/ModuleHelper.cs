using Discord.Commands;
using Discord.WebSocket;

namespace TheDialgaTeam.DiscordBot.Modules
{
    public abstract class ModuleHelper : ModuleBase<SocketCommandContext>
    {
        protected override async void AfterExecute(CommandInfo command)
        {
            if (Context.Message.Channel is SocketDMChannel || Context.Message.Channel is SocketGroupChannel)
                return;

            var perms = Context.Guild.GetUser(Context.Client.CurrentUser.Id).GetPermissions(Context.Guild.GetChannel(Context.Message.Channel.Id));

            if (perms.ManageMessages)
                await Context.Message.DeleteAsync();
        }
    }
}