using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TheDialgaTeam.Discord.Bot.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Service.Discord;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Module
{
    [Name("Free Game Notification")]
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
                              .WithDescription($@"Free Game Notification is a feature that will notify users about any potential free game that you can grab before the promotion expires.

This is run by our community. You can also help us by joining <https://discord.me/TheDialgaTeam> and report any potential free game that are not listed.

Blurgaro#1337 manage and announce the free game. You can find him at <https://discord.me/bptavern>.

To enable this module, use `{Context.Client.CurrentUser.Mention} EnableFreeGameNotificationModule`.");

            await ReplyAsync("", false, helpMessage.Build()).ConfigureAwait(false);
        }

        [Command("EnableFreeGameNotificationModule")]
        [Alias("EnableFGNModule")]
        [Summary("Enable Free Game Notification module.")]
        [RequirePermission(RequiredPermission.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        public async Task SubscribeFreeGameNotificationModuleAsync()
        {
            var discordGuildId = await SqliteService.GetDiscordGuildIdAsync(Context.Client.CurrentUser.Id, Context.Guild.Id).ConfigureAwait(false);
            var discordGuildModule = await SqliteService.SQLiteAsyncConnection.Table<DiscordGuildModuleTable>().Where(a => a.DiscordGuildId == discordGuildId && a.Module == "Free Game Notification").FirstOrDefaultAsync().ConfigureAwait(false);

            if (discordGuildModule == null)
                await SqliteService.SQLiteAsyncConnection.InsertAsync(new DiscordGuildModuleTable { Module = "Free Game Notification", Active = true, DiscordGuildId = discordGuildId }).ConfigureAwait(false);
            else
            {
                discordGuildModule.Active = true;
                await SqliteService.SQLiteAsyncConnection.UpdateAsync(discordGuildModule).ConfigureAwait(false);
            }

            await ReplyAsync(CommandExecuteResult.FromSuccess("Enabled Free Game Notification module.").BuildDiscordTextResponse()).ConfigureAwait(false);
        }

        [Command("DisableFreeGameNotificationModule")]
        [Alias("DisableFGNModule")]
        [Summary("Disable Free Game Notification module.")]
        [RequirePermission(RequiredPermission.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        [RequireActiveModule]
        public async Task UnsubscribeFreeGameNotificationModuleAsync()
        {
            var discordGuildId = await SqliteService.GetDiscordGuildIdAsync(Context.Client.CurrentUser.Id, Context.Guild.Id).ConfigureAwait(false);
            var discordGuildModule = await SqliteService.SQLiteAsyncConnection.Table<DiscordGuildModuleTable>().Where(a => a.DiscordGuildId == discordGuildId && a.Module == "Free Game Notification").FirstOrDefaultAsync().ConfigureAwait(false);

            if (discordGuildModule == null)
                await SqliteService.SQLiteAsyncConnection.InsertAsync(new DiscordGuildModuleTable { Module = "Free Game Notification", Active = false, DiscordGuildId = discordGuildId }).ConfigureAwait(false);
            else
            {
                discordGuildModule.Active = false;
                await SqliteService.SQLiteAsyncConnection.UpdateAsync(discordGuildModule).ConfigureAwait(false);
            }

            await ReplyAsync(CommandExecuteResult.FromSuccess("Disabled Free Game Notification module.").BuildDiscordTextResponse()).ConfigureAwait(false);
        }

        //[Command("FreeGameNotificationSetup")]
        //[Alias("FGNSetup")]
        //[Summary("Setup free game notification for this guild.")]
        //[RequirePermission(RequiredPermission.GuildAdministrator)]
        //[RequireContext(ContextType.Guild)]
        //[RequireActiveModule]
        //public async Task FreeGameNotificationSetupAsync([Summary("Channel to announce in for the free game annonucement.")]
        //                                                 IChannel channel, [Summary("Role to mention for the free game announcement.")]
        //                                                 IRole role = null)
        //{
        //    var discordChannelId = await SqliteService.GetDiscordChannelIdAsync(Context.Client.CurrentUser.Id, Context.Guild.Id, Context.Channel.Id).ConfigureAwait(false);
        //    var freeGameNotification = await SqliteService.SQLiteAsyncConnection.Table<FreeGameNotificationTable>().Where(a => a.DiscordChannelId == discordChannelId).FirstOrDefaultAsync().ConfigureAwait(false);

        //    if (freeGameNotification == null)
        //        await SqliteService.SQLiteAsyncConnection.InsertAsync(new FreeGameNotificationTable { })

        //    var clientId = Context.Client.CurrentUser.Id.ToString();
        //    var guildId = Context.Guild.Id.ToString();

        //    var freeGameNotificationModel = await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

        //    if (freeGameNotificationModel == null)
        //        await SQLiteService.SQLiteAsyncConnection.InsertAsync(new FreeGameNotificationModel { ClientId = clientId, GuildId = guildId, RoleId = role?.Id.ToString() ?? "", ChannelId = channel.Id.ToString() });
        //    else
        //    {
        //        freeGameNotificationModel.RoleId = role?.Id.ToString() ?? "";
        //        freeGameNotificationModel.ChannelId = channel.Id.ToString();
        //        await SQLiteService.SQLiteAsyncConnection.UpdateAsync(freeGameNotificationModel);
        //    }

        //    if (role != null)
        //        await ReplyAsync($":white_check_mark: Successfully setup free game notification in {MentionUtils.MentionChannel(channel.Id)} with {role.Mention}.");
        //    else
        //        await ReplyAsync($":white_check_mark: Successfully setup free game notification in {MentionUtils.MentionChannel(channel.Id)}.");
        //}

        //[Command("FreeGameNotificationAnnounce")]
        //[Alias("FGNAnnounce")]
        //[Summary("Announce free game notification to all subscribed guilds.")]
        //[RequirePermission(RequiredPermissions.GlobalDiscordAppOwner)]
        //public async Task FreeGameNotificationAnnounceAsync([Remainder] [Summary("Message to announce.")]
        //                                                    string message)
        //{
        //    foreach (var discordSocketClientModel in DiscordAppService.DiscordShardedClientHelpers)
        //    {
        //        var clientId = discordSocketClientModel.DiscordAppModel.ClientId;
        //        var freeGameNotificationModels = await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().Where(a => a.ClientId == clientId).OrderBy(a => a.GuildId).ToArrayAsync();

        //        foreach (var freeGameNotificationModel in freeGameNotificationModels)
        //        {
        //            var guild = discordSocketClientModel.DiscordShardedClient.GetGuild(Convert.ToUInt64(freeGameNotificationModel.GuildId));

        //            // Guild does not exist.
        //            if (guild == null)
        //                continue;

        //            var discordGuildModuleModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().Where(a => a.ClientId == clientId && a.GuildId == freeGameNotificationModel.GuildId && a.Module == "Free Game Notification").FirstOrDefaultAsync();

        //            // Free Game Notification not subscribed.
        //            if (discordGuildModuleModel == null || !discordGuildModuleModel.Active)
        //                continue;

        //            var user = guild.GetUser(Convert.ToUInt64(discordSocketClientModel.DiscordAppModel.ClientId));

        //            // Bot does not exist.
        //            if (user == null)
        //                continue;

        //            var textChannel = guild.GetTextChannel(Convert.ToUInt64(freeGameNotificationModel.ChannelId));

        //            // Text channel does not exist.
        //            if (textChannel == null)
        //                continue;

        //            // Bot cannot send message.
        //            if (!user.GetPermissions(textChannel).SendMessages)
        //                continue;

        //            if (freeGameNotificationModel.RoleId == guild.Id.ToString())
        //                await textChannel.SendMessageAsync($"@everyone {message}");
        //            else if (!string.IsNullOrEmpty(freeGameNotificationModel.RoleId))
        //                await textChannel.SendMessageAsync($"{MentionUtils.MentionRole(Convert.ToUInt64(freeGameNotificationModel.RoleId))} {message}");
        //            else
        //                await textChannel.SendMessageAsync(message);
        //        }
        //    }
        //}

        //[Command("FreeGameNotificationTestAnnounce")]
        //[Alias("FGNTestAnnounce")]
        //[Summary("Announce free game notification to this guild. (Dry run)")]
        //[RequirePermission(RequiredPermissions.GuildAdministrator)]
        //[RequireContext(ContextType.Guild)]
        //[RequireActiveModule]
        //public async Task FreeGameNotificationTestAnnounceAsync([Remainder] [Summary("Message to announce.")]
        //                                                        string message)
        //{
        //    var clientId = Context.Client.CurrentUser.Id.ToString();
        //    var guildId = Context.Guild.Id.ToString();

        //    var freeGameNotificationModel = await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

        //    if (freeGameNotificationModel == null)
        //        return;

        //    if (!Context.Guild.GetUser(Context.Client.CurrentUser.Id).GuildPermissions.SendMessages)
        //        return;

        //    var textChannel = Context.Guild.GetTextChannel(Convert.ToUInt64(freeGameNotificationModel.ChannelId));

        //    if (freeGameNotificationModel.RoleId == Context.Guild.Id.ToString())
        //        await textChannel.SendMessageAsync($"@everyone {message}");
        //    else if (!string.IsNullOrEmpty(freeGameNotificationModel.RoleId))
        //        await textChannel.SendMessageAsync($"{MentionUtils.MentionRole(Convert.ToUInt64(freeGameNotificationModel.RoleId))} {message}");
        //    else
        //        await textChannel.SendMessageAsync(message);
        //}
    }
}