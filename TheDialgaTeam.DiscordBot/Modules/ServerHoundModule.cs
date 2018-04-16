using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("ServerHound")]
    public sealed class ServerHoundModule : ModuleHelper
    {
        private ISQLiteService SQLiteService { get; }

        public ServerHoundModule(ISQLiteService sqliteService)
        {
            SQLiteService = sqliteService;
        }

        [Command("ServerHoundAbout")]
        [Alias("SHAbout")]
        [Summary("Find out more about ServerHound module.")]
        public async Task ServerHoundAboutAsync()
        {
            var helpMessage = new EmbedBuilder()
                              .WithTitle("ServerHound Module:")
                              .WithColor(Color.Orange)
                              .WithDescription(@"ServerHound is a security and server listing bot developed by SV (API & Security) and Marto (Doggo Pics).
You can visit https://discord.gg/yYRa2Uz for more details.

Discord Bans (DBans) is part of ServerHound security feature. This bot allows you to utilize DBans to protect your server if you do not wish to have full ServerHound features.
If you are raided, make sure to screenshot them as proof and visit the invite link below to report them.
https://discord.gg/VExdtUb

Please note that this is not a replacement of ServerHound as a whole. It is named this way as it is part of ServerHound feature. The devs of this bot do not have control over the data from them.");

            await ReplyAsync("", false, helpMessage.Build());
        }

        [Command("EnableServerHoundModule")]
        [Alias("EnableSHModule")]
        [Summary("Enable ServerHound module.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        public async Task SubscribeServerHoundModuleAsync()
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var discordGuildModuleModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (discordGuildModuleModel == null)
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(new DiscordGuildModuleModel { ClientId = clientId, GuildId = guildId, Module = "ServerHound", Active = true });
            else
            {
                discordGuildModuleModel.Active = true;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordGuildModuleModel);
            }

            await ReplyAsync(@":white_check_mark: Successfully enable ServerHound module.");
        }

        [Command("DisableServerHoundModule")]
        [Alias("DisableSHModule")]
        [Summary("Disable ServerHound module.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        public async Task UnsubscribeServerHoundModuleAsync()
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var discordGuildModuleModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (discordGuildModuleModel == null)
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(new DiscordGuildModuleModel { ClientId = clientId, GuildId = guildId, Module = "ServerHound", Active = false });
            else
            {
                discordGuildModuleModel.Active = false;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordGuildModuleModel);
            }

            await ReplyAsync(@":white_check_mark: Successfully disable ServerHound module.");
        }

        [Command("ServerHoundEnableDBans")]
        [Alias("SHEnableDBans")]
        [Summary("Enable ServerHound DBans feature.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        [RequireActiveModule]
        public async Task ServerHoundActivateDBansAsync()
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

            await ReplyAsync(":white_check_mark: Successfully enable ServerHound DBans feature.");
        }

        [Command("ServerHoundDisableDBans")]
        [Alias("SHDisableDBans")]
        [Summary("Disable ServerHound DBans feature.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        [RequireContext(ContextType.Guild)]
        [RequireActiveModule]
        public async Task ServerHoundDeactivateDBansAsync()
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

            await ReplyAsync(":white_check_mark: Successfully disable ServerHound DBans feature.");
        }
    }
}