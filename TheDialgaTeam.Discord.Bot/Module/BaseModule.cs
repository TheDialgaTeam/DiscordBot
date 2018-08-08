using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TheDialgaTeam.Discord.Bot.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Service.Discord;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Module
{
    [Name("Base")]
    public sealed class BaseModule : ModuleHelper
    {
        private DiscordAppService DiscordAppService { get; }

        private SQLiteService SQLiteService { get; }

        public BaseModule(DiscordAppService discordAppService, SQLiteService sqliteService)
        {
            DiscordAppService = discordAppService;
            SQLiteService = sqliteService;
        }

        [Command("Ping")]
        [Summary("Check the connection speed between the bot and the server.")]
        public async Task PingAsync()
        {
            await ReplyAsync($"Ping: {Context.Client.Latency} ms");
        }

        [Command("About")]
        [Summary("Get the bot information.")]
        public async Task AboutAsync()
        {
            var applicationInfo = await Context.Client.GetApplicationInfoAsync();

            var helpMessage = new EmbedBuilder()
                              .WithTitle("The Dialga Team Discord Bot:")
                              .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                              .WithColor(Color.Orange)
                              .WithDescription($@"Hello, I am **{Context.Client.CurrentUser.Username}**, a multipurpose bot that is created by jianmingyong#4964.

I am owned by **{applicationInfo.Owner}**.

Type `{Context.Client.CurrentUser.Mention} help` to see my command. You can also type `help` in this DM to see my command as well.

You can invite this bot by using this link: <https://discordapp.com/api/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&permissions=0&scope=bot>

If you want to have a custom avatar and bot name, feel free to join our bot discord server <https://discord.me/TheDialgaTeam> and read the invite section.");

            await ReplyDMAsync("", false, helpMessage.Build());
        }

        //[Command("GlobalBotAnnounce")]
        //[Summary("Announce a message into all of the guilds of all bot instances.")]
        //[RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        //public async Task GlobalBotAnnounceAsync([Remainder] [Summary("Message to send.")] string message)
        //{
        //    var ignoreGuilds = new List<ulong>();

        //    foreach (var discordSocketClientModel in DiscordAppService.DiscordShardedClientHelpers)
        //    {
        //        foreach (var socketGuild in discordSocketClientModel.DiscordShardedClient.Guilds)
        //        {
        //            if (ignoreGuilds.Contains(socketGuild.Id))
        //                continue;

        //            var perms = socketGuild.GetUser(discordSocketClientModel.DiscordShardedClient.CurrentUser.Id).GetPermissions(socketGuild.DefaultChannel);

        //            if (perms.SendMessages)
        //                await socketGuild.DefaultChannel.SendMessageAsync(message);

        //            ignoreGuilds.Add(socketGuild.Id);
        //        }
        //    }
        //}

        [Command("BotAnnounce")]
        [Summary("Announce a message into all of the guilds of this bot instance.")]
        [RequirePermission(RequiredPermission.DiscordAppOwner)]
        public async Task BotAnnounceAsync([Remainder] [Summary("Message to send.")]
                                           string message)
        {
            foreach (var socketGuild in Context.Client.Guilds)
            {
                var perms = Context.Guild.GetUser(Context.Client.CurrentUser.Id).GetPermissions(Context.Guild.DefaultChannel);

                if (perms.SendMessages)
                    await socketGuild.DefaultChannel.SendMessageAsync(message);
            }
        }

        //[Command("AddDiscordApp")]
        //[Summary("Add a new bot instance.")]
        //[RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        //public async Task AddDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId, [Summary("Discord bot client secret.")] string clientSecret, [Remainder] [Summary("Bot secret token.")] string botToken)
        //{
        //    var stringClientId = clientId.ToString();
        //    var discordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == stringClientId).FirstOrDefaultAsync();

        //    if (discordAppModel == null)
        //    {
        //        var discordAppModelNew = new DiscordAppModel { ClientId = clientId.ToString(), ClientSecret = clientSecret, BotToken = botToken };

        //        await SQLiteService.SQLiteAsyncConnection.InsertAsync(discordAppModelNew);
        //        await ReplyAsync(":white_check_mark: Successfully added new discord app.");
        //    }
        //    else
        //    {
        //        discordAppModel.ClientSecret = clientSecret;
        //        discordAppModel.BotToken = botToken;

        //        await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordAppModel);
        //        await ReplyAsync(":white_check_mark: Successfully updated the discord app.");
        //    }
        //}

        //[Command("RemoveDiscordApp")]
        //[Summary("Remove a bot instance.")]
        //[RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        //public async Task RemoveDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId)
        //{
        //    var stringClientId = clientId.ToString();
        //    var discordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == stringClientId).FirstOrDefaultAsync();

        //    if (discordAppModel == null)
        //    {
        //        await ReplyAsync(":negative_squared_cross_mark: This discord app is does not exist!");
        //        return;
        //    }

        //    await SQLiteService.SQLiteAsyncConnection.DeleteAsync(discordAppModel);
        //    await ReplyAsync(":white_check_mark: Successfully remove the discord app.");
        //}

        [Command("StartDiscordApp")]
        [Summary("Start a new bot instance")]
        [RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        public async Task StartDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId)
        {
            var result = await DiscordAppService.StartDiscordAppAsync(clientId).ConfigureAwait(false);
            await ReplyAsync(result.BuildDiscordTextResponse());
        }

        [Command("StopDiscordApp")]
        [Summary("Stop a bot instance")]
        [RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        public async Task StopDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId)
        {
            var result = await DiscordAppService.StopDiscordAppAsync(clientId).ConfigureAwait(false);
            await ReplyAsync(result.BuildDiscordTextResponse());
        }
    }
}