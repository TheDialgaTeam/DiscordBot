using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("ServerHound")]
    [RequireActiveModule]
    public sealed class ServerHoundModule : ModuleHelper
    {
        private ISQLiteService SQLiteService { get; }

        public ServerHoundModule(ISQLiteService sqliteService)
        {
            SQLiteService = sqliteService;
        }

        [Command("SHAbout")]
        [Summary("Find out more about this module.")]
        public async Task SHAboutAsync()
        {
            var helpMessage = new EmbedBuilder()
                              .WithTitle("ServerHound Module:")
                              .WithThumbnailUrl(Context.User.GetAvatarUrl())
                              .WithColor(Color.Orange)
                              .WithDescription(@"ServerHound is a security and server listing bot developed by SV (API & Security) and Marto (Doggo Pics).
You can visit https://discord.gg/yYRa2Uz for more details.

Discord Bans (DBans) is part of ServerHound security feature. This bot allows you to utilize DBans to protect your server if you do not wish to have full ServerHound features.
If you are raided, make sure to screenshot them as proof and visit the invite link below to report them.
https://discord.gg/VExdtUb

Please note that this is not a replacement of ServerHound as a whole. It is named this way as it is part of ServerHound feature. The devs of this bot do not have control over the data from them.");

            await ReplyAsync("", false, helpMessage.Build());
        }

        [Command("SHActivateDBans")]
        [Summary("Activate ServerHound DBans feature.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        public async Task SHActivateDBansAsync()
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var serverHoundModel = await SQLiteService.SQLiteAsyncConnection.Table<ServerHoundModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (serverHoundModel == null)
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(new ServerHoundModel { ClientId = clientId, GuildId = guildId, DBans = true });
            else
            {
                serverHoundModel.DBans = true;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(serverHoundModel);
            }

            await ReplyAsync(":white_check_mark: Successfully activate ServerHound DBans feature.");
        }

        [Command("SHDeactivateDBans")]
        [Summary("Deactivate ServerHound DBans feature.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        public async Task SHDeactivateDBansAsync()
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var serverHoundModel = await SQLiteService.SQLiteAsyncConnection.Table<ServerHoundModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (serverHoundModel == null)
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(new ServerHoundModel { ClientId = clientId, GuildId = guildId, DBans = false });
            else
            {
                serverHoundModel.DBans = false;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(serverHoundModel);
            }

            await ReplyAsync(":white_check_mark: Successfully deactivate ServerHound DBans feature.");
        }
    }
}