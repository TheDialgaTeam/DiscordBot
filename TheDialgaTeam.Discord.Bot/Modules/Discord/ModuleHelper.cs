using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Modules.Discord
{
    public abstract class ModuleHelper : ModuleBase<ShardedCommandContext>
    {
        protected SqliteDatabaseService SqliteDatabaseService { get; }

        protected ModuleHelper(SqliteDatabaseService sqliteDatabaseService)
        {
            SqliteDatabaseService = sqliteDatabaseService;
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

            using (var databaseContext = SqliteDatabaseService.GetContext(true))
            {
                var deleteCommandAfterUse = (await databaseContext.DiscordAppTable.Where(a => a.ClientId == Context.Client.CurrentUser.Id)
                                                                  .Select(a => new
                                                                  {
                                                                      discordGuild = a.DiscordGuilds.Where(b => b.GuildId == Context.Guild.Id)
                                                                                      .Select(b => b.DeleteCommandAfterUse).FirstOrDefault()
                                                                  }).ToListAsync().ConfigureAwait(false))
                                            .Select(a => a.discordGuild).FirstOrDefault();

                if (deleteCommandAfterUse && GetChannelPermissions().ManageMessages)
                    await Context.Message.DeleteAsync().ConfigureAwait(false);
            }
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