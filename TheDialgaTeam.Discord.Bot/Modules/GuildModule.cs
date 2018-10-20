using System.Threading.Tasks;
using Discord.Commands;
using TheDialgaTeam.Discord.Bot.Models.Discord.Command;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Modules
{
    [Name("Guild")]
    [RequireContext(ContextType.Guild)]
    public sealed class GuildModule : ModuleHelper
    {
        public GuildModule(Program program, SqliteDatabaseService sqliteDatabaseService) : base(sqliteDatabaseService)
        {
        }

        [Command("ChannelSay")]
        [Summary("Announce a message into this channel.")]
        [RequirePermission(RequiredPermission.ChannelModerator)]
        public async Task ChannelSayAsync([Remainder] [Summary("Message to send.")]
                                          string message)
        {
            await ReplyAsync(message).ConfigureAwait(false);
        }

        [Command("ShowCommandPrefix")]
        [Summary("Show the current command prefix for this guild.")]
        public async Task ShowCommandPrefixAsync()
        {
            using (var context = SqliteDatabaseService.GetContext(true))
            {
                var discordGuild = await context.GetDiscordGuildTableAsync(Context.Client.CurrentUser.Id, Context.Guild.Id).ConfigureAwait(false);

                if (discordGuild?.Prefix == null)
                    await ReplyAsync(CommandExecuteResult.FromError("There is no command prefix set for this guild!").BuildDiscordTextResponse()).ConfigureAwait(false);
                else
                    await ReplyAsync($"The current command prefix is `{discordGuild.Prefix}`.").ConfigureAwait(false);
            }
        }

        [Command("SetCommandPrefix")]
        [Summary("Set the current command prefix for this guild.")]
        [RequirePermission(RequiredPermission.GuildAdministrator)]
        public async Task SetCommandPrefixAsync([Remainder] [Summary("Custom prefix to use when using this bot commands.")]
                                                string prefix)
        {
            using (var context = SqliteDatabaseService.GetContext())
            {
                var discordGuild = await context.GetDiscordGuildTableAsync(Context.Client.CurrentUser.Id, Context.Guild.Id).ConfigureAwait(false);

                if (discordGuild == null)
                    return;

                discordGuild.Prefix = prefix;

                context.DiscordGuildTable.Update(discordGuild);
                await context.SaveChangesAsync().ConfigureAwait(false);

                await ReplyAsync(CommandExecuteResult.FromSuccess($"Changed the prefix to `{prefix}`.").BuildDiscordTextResponse()).ConfigureAwait(false);
            }
        }

        [Command("SetCommandDeleteAfterUse")]
        [Summary("Set if the command should be deleted after use for this guild.")]
        [RequirePermission(RequiredPermission.GuildAdministrator)]
        public async Task SetCommandDeleteAfterUse([Summary("True if you want to have command deleted after use.")]
                                                   bool active)
        {
            using (var context = SqliteDatabaseService.GetContext())
            {
                var discordGuild = await context.GetDiscordGuildTableAsync(Context.Client.CurrentUser.Id, Context.Guild.Id).ConfigureAwait(false);

                if (discordGuild == null)
                    return;

                discordGuild.DeleteCommandAfterUse = active;

                context.DiscordGuildTable.Update(discordGuild);
                await context.SaveChangesAsync().ConfigureAwait(false);

                await ReplyAsync(CommandExecuteResult.FromSuccess($"Delete command after use is `{(active ? "active" : "not active")}`.").BuildDiscordTextResponse()).ConfigureAwait(false);
            }
        }
    }
}