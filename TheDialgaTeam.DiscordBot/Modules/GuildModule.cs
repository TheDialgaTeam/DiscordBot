using Discord.Commands;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("Guild")]
    [RequireContext(ContextType.Guild)]
    public sealed class GuildModule : ModuleHelper
    {
        private CommandService CommandService { get; }

        private ISQLiteService SQLiteService { get; }

        public GuildModule(IProgram program, ISQLiteService sqliteService)
        {
            CommandService = program.CommandService;
            SQLiteService = sqliteService;
        }

        [Command("GuildSay")]
        [Summary("Announce a message into this guild.")]
        [RequirePermission(RequiredPermissions.GuildModerator)]
        public async Task GuildSayAsync([Remainder] [Summary("Message to send.")] string message)
        {
            var perms = Context.Guild.GetUser(Context.Client.CurrentUser.Id).GetPermissions(Context.Guild.DefaultChannel);

            if (perms.SendMessages)
                await Context.Guild.DefaultChannel.SendMessageAsync(message);
        }

        [Command("ChannelSay")]
        [Summary("Announce a message into this channel.")]
        [RequirePermission(RequiredPermissions.ChannelModerator)]
        public async Task ChannelSayAsync([Remainder] [Summary("Message to send.")] string message)
        {
            await ReplyAsync(message);
        }

        [Command("ShowCommandPrefix")]
        [Summary("Show the current command prefix for this guild.")]
        public async Task ShowCommandPrefixAsync()
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var discordGuildModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (discordGuildModel == null)
                await ReplyAsync(":negative_squared_cross_mark: There is no command prefix set for this guild!");
            else
                await ReplyAsync($"The current command prefix is `{discordGuildModel.CharPrefix}`.");
        }

        [Command("SetCommandPrefix")]
        [Summary("Set the current command prefix for this guild.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        public async Task SetCommandPrefixAsync([Remainder] [Summary("Custom prefix to use when using this bot commands.")] string prefix)
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var discordGuildModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (discordGuildModel == null)
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(new DiscordGuildModel { ClientId = clientId, GuildId = guildId, CharPrefix = prefix });
            else
            {
                discordGuildModel.CharPrefix = prefix;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordGuildModel);
            }

            await ReplyAsync($":white_check_mark: Successfully changed the prefix to `{prefix}`.");
        }
    }
}