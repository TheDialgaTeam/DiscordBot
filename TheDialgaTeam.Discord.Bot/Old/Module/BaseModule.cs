using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TheDialgaTeam.Discord.Bot.Old.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Old.Service.Discord;
using TheDialgaTeam.Discord.Bot.Old.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Module
{
    [Name("Base")]
    public sealed class BaseModule : ModuleHelper
    {
        private DiscordAppService DiscordAppService { get; }

        public BaseModule(DiscordAppService discordAppService, SQLiteService sqliteService) : base(sqliteService)
        {
            DiscordAppService = discordAppService;
        }

        [Command("Ping")]
        [Summary("Gets the estimated round-trip latency, in milliseconds, to the gateway server.")]
        public async Task PingAsync()
        {
            await ReplyAsync($"Ping: {Context.Client.Latency} ms").ConfigureAwait(false);
        }

        [Command("About")]
        [Summary("Get the bot information.")]
        public async Task AboutAsync()
        {
            var applicationInfo = await Context.Client.GetApplicationInfoAsync().ConfigureAwait(false);

            var helpMessage = new EmbedBuilder()
                              .WithTitle("The Dialga Team Discord Bot:")
                              .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                              .WithColor(Color.Orange)
                              .WithDescription($@"Hello, I am **{Context.Client.CurrentUser.Username}**, a multipurpose bot that is created by jianmingyong#4964.

I am owned by **{applicationInfo.Owner}**.

Type `@{Context.Client.CurrentUser.Username} help` to see my command. You can also type `help` in this DM to see any command that can be used in this DM.

You can invite this bot by using this link: <https://discordapp.com/api/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&permissions=0&scope=bot>

If you want to have a custom avatar and bot name, feel free to join our bot discord server <https://discord.aggressivegaming.org/invite/TheDialgaTeam> and read the invite section.");

            await ReplyDMAsync("", false, helpMessage.Build()).ConfigureAwait(false);
        }

        [Command("GlobalBotAnnounce")]
        [Summary("Announce a message into all of the guilds of all bot instances.")]
        [RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        public async Task GlobalBotAnnounceAsync([Remainder] [Summary("Message to send.")]
                                                 string message)
        {
            var ignoreGuilds = new List<ulong>();

            //await DiscordAppService.RequestDiscordAppInstanceAsync(async discordAppInstances =>
            //{
            //    foreach (var discordAppInstance in discordAppInstances)
            //    {
            //        foreach (var socketGuild in discordAppInstance.DiscordShardedClient.Guilds)
            //        {
            //            if (ignoreGuilds.Contains(socketGuild.Id))
            //                continue;

            //            if (GetChannelPermissions(socketGuild.DefaultChannel.Id).SendMessages)
            //                await socketGuild.DefaultChannel.SendMessageAsync(message).ConfigureAwait(false);

            //            ignoreGuilds.Add(socketGuild.Id);
            //        }
            //    }
            //}).ConfigureAwait(false);
        }

        [Command("BotAnnounce")]
        [Summary("Announce a message into all of the guilds of this bot instance.")]
        [RequirePermission(RequiredPermission.DiscordAppOwner)]
        public async Task BotAnnounceAsync([Remainder] [Summary("Message to send.")]
                                           string message)
        {
            foreach (var socketGuild in Context.Client.Guilds)
            {
                if (GetChannelPermissions(Context.Guild.DefaultChannel.Id).SendMessages)
                    await socketGuild.DefaultChannel.SendMessageAsync(message).ConfigureAwait(false);
            }
        }

        [Command("AddDiscordApp")]
        [Summary("Add a new bot instance.")]
        [RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        public async Task AddDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId,
                                             [Summary("Discord bot client secret.")]
                                             string clientSecret,
                                             [Remainder] [Summary("Bot secret token.")]
                                             string botToken)
        {
            var discordApp = new DiscordAppTable { ClientId = clientId.ToString(), ClientSecret = clientSecret, BotToken = botToken };
            await SqliteService.SQLiteAsyncConnection.InsertOrReplaceAsync(discordApp).ConfigureAwait(false);
            await ReplyAsync(CommandExecuteResult.FromSuccess("Discord App have been added.").BuildDiscordTextResponse()).ConfigureAwait(false);
        }

        [Command("RemoveDiscordApp")]
        [Summary("Remove a bot instance.")]
        [RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        public async Task RemoveDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId)
        {
            var clientIdString = clientId.ToString();
            var discordApp = await SqliteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);

            if (discordApp == null)
            {
                await ReplyAsync(CommandExecuteResult.FromError("Could not find any Discord App with this ID!").BuildDiscordTextResponse()).ConfigureAwait(false);
                return;
            }

            await SqliteService.SQLiteAsyncConnection.DeleteAsync(discordApp).ConfigureAwait(false);
            await ReplyAsync(CommandExecuteResult.FromSuccess("Discord App have been deleted.").BuildDiscordTextResponse()).ConfigureAwait(false);
        }

        [Command("StartDiscordApp")]
        [Summary("Start a new bot instance")]
        [RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        public async Task StartDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId)
        {
            var result = await DiscordAppService.StartDiscordAppAsync(clientId).ConfigureAwait(false);
            await ReplyAsync(result.BuildDiscordTextResponse()).ConfigureAwait(false);
        }

        [Command("StopDiscordApp")]
        [Summary("Stop a bot instance")]
        [RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
        public async Task StopDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId)
        {
            var result = await DiscordAppService.StopDiscordAppAsync(clientId).ConfigureAwait(false);
            await ReplyAsync(result.BuildDiscordTextResponse()).ConfigureAwait(false);
        }
    }
}