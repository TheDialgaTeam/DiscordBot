using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TheDialgaTeam.Discord.Bot.Old.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Old.Service.Discord;
using TheDialgaTeam.Discord.Bot.Old.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Module
{
    [Name("Free Game Notification")]
    [SubscribableModule("Free Game Notification")]
    public sealed class FreeGameNotificationModule : ModuleHelper
    {
        private DiscordAppService DiscordAppService { get; }

        public FreeGameNotificationModule(DiscordAppService discordAppService, SQLiteService sqliteService) : base(sqliteService)
        {
            DiscordAppService = discordAppService;
        }

        [Command("FreeGameNotificationAbout")]
        [Alias("FGNAbout")]
        [Summary("Find out more about Free Game Notification module.")]
        public async Task FreeGameNotificationAboutAsync()
        {
            var helpMessage = new EmbedBuilder()
                              .WithTitle("Free Game Notification Module:")
                              .WithColor(Color.Orange)
                              .WithDescription(@"Free Game Notification is a feature that will notify users about any potential free game that you can grab before the promotion expires.

This is run by our community. You can also help us by joining <https://discord.aggressivegaming.org/invite/TheDialgaTeam> and report any potential free game that are not listed.

Blurgaro#1337 manage and announce the free game. You can find him at <https://discord.aggressivegaming.org/invite/bptavern>.");

            await ReplyAsync("", false, helpMessage.Build()).ConfigureAwait(false);
        }

        [Command("FreeGameNotificationSetup")]
        [Alias("FGNSetup")]
        [Summary("Setup free game notification for this guild.")]
        [RequirePermission(RequiredPermission.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        [RequireActiveModule]
        public async Task FreeGameNotificationSetupAsync([Summary("Channel to announce in for the free game announcement.")]
                                                         IChannel channel, [Summary("Role to mention for the free game announcement.")]
                                                         IRole role = null)
        {
            var discordChannelId = await SqliteService.GetDiscordChannelIdAsync(Context.Client.CurrentUser.Id, Context.Guild.Id, channel.Id).ConfigureAwait(false);
            var freeGameNotification = await SqliteService.SQLiteAsyncConnection.Table<FreeGameNotificationTable>().Where(a => a.DiscordChannelId == discordChannelId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (freeGameNotification == null)
                await SqliteService.SQLiteAsyncConnection.InsertAsync(new FreeGameNotificationTable { DiscordChannelId = discordChannelId, Active = true, RoleId = role?.Id.ToString() });
            else
            {
                freeGameNotification.RoleId = role?.Id.ToString();
                freeGameNotification.Active = true;
                freeGameNotification.DiscordChannelId = discordChannelId;

                await SqliteService.SQLiteAsyncConnection.UpdateAsync(freeGameNotification);
            }

            await ReplyAsync(CommandExecuteResult.FromSuccess($"Successfully setup free game notification in {MentionUtils.MentionChannel(channel.Id)}{(role == null ? "." : $" with {role.Mention}.")}").BuildDiscordTextResponse()).ConfigureAwait(false);
        }

        [Command("FreeGameNotificationAnnounce")]
        [Alias("FGNAnnounce")]
        [Summary("Announce free game notification to all subscribed guilds.")]
        [RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        public async Task FreeGameNotificationAnnounceAsync([Remainder] [Summary("Message to announce.")]
                                                            string message)
        {
            //await DiscordAppService.RequestDiscordAppInstanceAsync(async discordAppInstances =>
            //{
            //    var freeGameNotificationTables = await SqliteService.SQLiteAsyncConnection.Table<FreeGameNotificationTable>().Where(a => a.Active == true).ToArrayAsync().ConfigureAwait(false);

            //    foreach (var freeGameNotificationTable in freeGameNotificationTables)
            //    {
            //        var discordChannelTable = await SqliteService.SQLiteAsyncConnection.Table<DiscordChannelTable>().Where(a => a.Id == freeGameNotificationTable.DiscordChannelId).FirstOrDefaultAsync().ConfigureAwait(false);
            //        var discordGuildTable = await SqliteService.SQLiteAsyncConnection.Table<DiscordGuildTable>().Where(a => a.Id == discordChannelTable.DiscordGuildId).FirstOrDefaultAsync().ConfigureAwait(false);
            //        var discordAppTable = await SqliteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.Id == discordGuildTable.DiscordAppId).FirstOrDefaultAsync().ConfigureAwait(false);

            //        if (discordAppTable == null)
            //            continue;

            //        foreach (var discordAppInstance in discordAppInstances)
            //        {
            //            if (discordAppInstance.ClientId.ToString() != discordAppTable.ClientId)
            //                continue;

            //            var guild = discordAppInstance.DiscordShardedClient.GetGuild(Convert.ToUInt64(discordGuildTable.GuildId));
            //            var user = guild?.GetUser(Convert.ToUInt64(discordAppInstance.ClientId));

            //            if (user == null)
            //                break;

            //            var textChannel = guild.GetTextChannel(Convert.ToUInt64(discordChannelTable.ChannelId));

            //            if (textChannel == null)
            //                break;

            //            if (!user.GetPermissions(textChannel).SendMessages)
            //                break;

            //            if (freeGameNotificationTable.RoleId == guild.Id.ToString())
            //                await textChannel.SendMessageAsync($"@everyone {message}").ConfigureAwait(false);
            //            else if (!string.IsNullOrEmpty(freeGameNotificationTable.RoleId))
            //                await textChannel.SendMessageAsync($"{MentionUtils.MentionRole(Convert.ToUInt64(freeGameNotificationTable.RoleId))} {message}").ConfigureAwait(false);
            //            else
            //                await textChannel.SendMessageAsync(message).ConfigureAwait(false);
            //        }
            //    }
            //}).ConfigureAwait(false);
        }

        [Command("FreeGameNotificationTestAnnounce")]
        [Alias("FGNTestAnnounce")]
        [Summary("Announce free game notification to this guild. (Dry run)")]
        [RequirePermission(RequiredPermission.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        [RequireActiveModule]
        public async Task FreeGameNotificationTestAnnounceAsync([Remainder] [Summary("Message to announce.")]
                                                                string message)
        {
            //await DiscordAppService.RequestDiscordAppInstanceAsync(async discordAppInstances =>
            //{
            //    var discordChannelId = await SqliteService.GetDiscordChannelIdAsync(Context.Client.CurrentUser.Id, Context.Guild.Id, Context.Channel.Id).ConfigureAwait(false);
            //    var freeGameNotificationTable = await SqliteService.SQLiteAsyncConnection.Table<FreeGameNotificationTable>().Where(a => a.Active == true && a.DiscordChannelId == discordChannelId).FirstOrDefaultAsync().ConfigureAwait(false);

            //    if (freeGameNotificationTable == null)
            //        return;

            //    foreach (var discordAppInstance in discordAppInstances)
            //    {
            //        if (discordAppInstance.ClientId != Context.Client.CurrentUser.Id)
            //            continue;

            //        var guild = discordAppInstance.DiscordShardedClient.GetGuild(Context.Guild.Id);
            //        var user = guild?.GetUser(Convert.ToUInt64(discordAppInstance.ClientId));

            //        if (user == null)
            //            break;

            //        var textChannel = guild.GetTextChannel(freeGameNotificationTable);

            //        if (textChannel == null)
            //            break;

            //        if (!user.GetPermissions(textChannel).SendMessages)
            //            break;

            //        if (freeGameNotificationTable.RoleId == guild.Id.ToString())
            //            await textChannel.SendMessageAsync($"@everyone {message}").ConfigureAwait(false);
            //        else if (!string.IsNullOrEmpty(freeGameNotificationTable.RoleId))
            //            await textChannel.SendMessageAsync($"{MentionUtils.MentionRole(Convert.ToUInt64(freeGameNotificationTable.RoleId))} {message}").ConfigureAwait(false);
            //        else
            //            await textChannel.SendMessageAsync(message).ConfigureAwait(false);
            //    }
            //}).ConfigureAwait(false);

            //var clientId = Context.Client.CurrentUser.Id.ToString();
            //var guildId = Context.Guild.Id.ToString();

            //var freeGameNotificationModel = await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            //if (freeGameNotificationModel == null)
            //    return;

            //if (!Context.Guild.GetUser(Context.Client.CurrentUser.Id).GuildPermissions.SendMessages)
            //    return;

            //var textChannel = Context.Guild.GetTextChannel(Convert.ToUInt64(freeGameNotificationModel.ChannelId));

            //if (freeGameNotificationModel.RoleId == Context.Guild.Id.ToString())
            //    await textChannel.SendMessageAsync($"@everyone {message}");
            //else if (!string.IsNullOrEmpty(freeGameNotificationModel.RoleId))
            //    await textChannel.SendMessageAsync($"{MentionUtils.MentionRole(Convert.ToUInt64(freeGameNotificationModel.RoleId))} {message}");
            //else
            //    await textChannel.SendMessageAsync(message);
        }
    }
}