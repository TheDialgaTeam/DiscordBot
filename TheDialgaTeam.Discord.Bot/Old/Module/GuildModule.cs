using System.Threading.Tasks;
using Discord.Commands;
using TheDialgaTeam.Discord.Bot.Old.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Old.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Module
{
    [Name("Guild")]
    [RequireContext(ContextType.Guild)]
    public sealed class GuildModule : ModuleHelper
    {
        private CommandService CommandService { get; }

        public GuildModule(Program program, SQLiteService sqliteService) : base(sqliteService)
        {
            CommandService = program.CommandService;
        }

        [Command("GuildSay")]
        [Summary("Announce a message into this guild.")]
        [RequirePermission(RequiredPermission.GuildModerator)]
        public async Task GuildSayAsync([Remainder] [Summary("Message to send.")]
                                        string message)
        {
            if (GetChannelPermissions(Context.Guild.DefaultChannel.Id).SendMessages)
                await Context.Guild.DefaultChannel.SendMessageAsync(message).ConfigureAwait(false);
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
            var discordGuild = await SqliteService.GetOrCreateDiscordGuildTableAsync(Context.Client.CurrentUser.Id, Context.Guild.Id).ConfigureAwait(false);

            if (discordGuild.Prefix == null)
                await ReplyAsync(CommandExecuteResult.FromError("There is no command prefix set for this guild!").BuildDiscordTextResponse()).ConfigureAwait(false);
            else
                await ReplyAsync($"The current command prefix is `{discordGuild.Prefix}`.").ConfigureAwait(false);
        }

        [Command("SetCommandPrefix")]
        [Summary("Set the current command prefix for this guild.")]
        [RequirePermission(RequiredPermission.GuildAdministrator)]
        public async Task SetCommandPrefixAsync([Remainder] [Summary("Custom prefix to use when using this bot commands.")]
                                                string prefix)
        {
            var discordGuild = await SqliteService.GetOrCreateDiscordGuildTableAsync(Context.Client.CurrentUser.Id, Context.Guild.Id).ConfigureAwait(false);
            discordGuild.Prefix = prefix;

            await SqliteService.SQLiteAsyncConnection.UpdateAsync(discordGuild).ConfigureAwait(false);
            await ReplyAsync(CommandExecuteResult.FromSuccess($"Changed the prefix to `{prefix}`.").BuildDiscordTextResponse()).ConfigureAwait(false);
        }

        [Command("SetCommandDeleteAfterUse")]
        [Summary("Set if the command should be deleted after use for this guild.")]
        [RequirePermission(RequiredPermission.GuildAdministrator)]
        public async Task SetCommandDeleteAfterUse([Summary("True if you want to have command deleted after use.")]
                                                   bool active)
        {
            var discordGuild = await SqliteService.GetOrCreateDiscordGuildTableAsync(Context.Client.CurrentUser.Id, Context.Guild.Id).ConfigureAwait(false);
            discordGuild.DeleteCommandAfterUse = active;

            await SqliteService.SQLiteAsyncConnection.UpdateAsync(discordGuild).ConfigureAwait(false);
            await ReplyAsync(CommandExecuteResult.FromSuccess($"Delete command after use is `{(active ? "active" : "not active")}`.").BuildDiscordTextResponse()).ConfigureAwait(false);
        }
    }
}