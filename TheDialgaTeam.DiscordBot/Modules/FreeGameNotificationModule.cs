using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("Free Game Notification")]
    [RequireActiveModule]
    public sealed class FreeGameNotificationModule : ModuleHelper
    {
        private ISQLiteService SQLiteService { get; }

        private IDiscordAppService DiscordAppService { get; }

        public FreeGameNotificationModule(ISQLiteService sqliteService, IDiscordAppService discordAppService)
        {
            SQLiteService = sqliteService;
            DiscordAppService = discordAppService;
        }

        [Command("FGNSetup")]
        [Summary("Setup free game notification for this guild.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        public async Task FGNSetup([Summary("Role to mention for the free game announcement.")] IRole role, [Summary("Channel to announce in for the free game annonucement.")] IChannel channel)
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var freeGameNotificationModel = await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (freeGameNotificationModel == null)
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(new FreeGameNotificationModel { ClientId = clientId, GuildId = guildId, RoleId = role.Id.ToString(), ChannelId = channel.Id.ToString() });
            else
            {
                freeGameNotificationModel.RoleId = role.Id.ToString();
                freeGameNotificationModel.ChannelId = channel.Id.ToString();
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(freeGameNotificationModel);
            }

            await ReplyAsync($":white_check_mark: Successfully setup free game notification with {role.Mention} in {MentionUtils.MentionChannel(channel.Id)}");
        }

        [Command("FGNAnnounce")]
        [Summary("Announce free game notification to all subscribed guilds.")]
        [RequirePermission(RequiredPermissions.GlobalDiscordAppOwner)]
        public async Task FGNAnnounce([Remainder] [Summary("Message to announce.")] string message)
        {
            foreach (var discordSocketClientModel in DiscordAppService.DiscordSocketClientModels)
            {
                var clientId = discordSocketClientModel.DiscordAppModel.ClientId;
                var freeGameNotificationModels = await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().Where(a => a.ClientId == clientId).OrderBy(a => a.GuildId).ToArrayAsync();

                foreach (var freeGameNotificationModel in freeGameNotificationModels)
                {
                    var guild = discordSocketClientModel.DiscordSocketClient.GetGuild(Convert.ToUInt64(freeGameNotificationModel.GuildId));
                    var textChannel = guild.GetTextChannel(Convert.ToUInt64(freeGameNotificationModel.ChannelId));
                    var user = guild.GetUser(Convert.ToUInt64(discordSocketClientModel.DiscordAppModel.ClientId));

                    if (!user.GetPermissions(textChannel).SendMessages)
                        continue;

                    if (freeGameNotificationModel.RoleId == guild.Id.ToString())
                        await textChannel.SendMessageAsync($"@everyone {message}");
                    else
                        await textChannel.SendMessageAsync($"{MentionUtils.MentionRole(Convert.ToUInt64(freeGameNotificationModel.RoleId))} {message}");
                }
            }
        }

        [Command("FGNTestAnnounce")]
        [Summary("Announce free game notification to this guild. (Dry run)")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        public async Task FGNTestAnnounce([Remainder] [Summary("Message to announce.")] string message)
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var freeGameNotificationModel = await SQLiteService.SQLiteAsyncConnection.Table<FreeGameNotificationModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (freeGameNotificationModel == null)
                return;

            if (!Context.Guild.GetUser(Context.Client.CurrentUser.Id).GuildPermissions.SendMessages)
                return;

            var textChannel = Context.Guild.GetTextChannel(Convert.ToUInt64(freeGameNotificationModel.ChannelId));

            if (freeGameNotificationModel.RoleId == Context.Guild.Id.ToString())
                await textChannel.SendMessageAsync($"@everyone {message}");
            else
                await textChannel.SendMessageAsync($"{MentionUtils.MentionRole(Convert.ToUInt64(freeGameNotificationModel.RoleId))} {message}");
        }
    }
}