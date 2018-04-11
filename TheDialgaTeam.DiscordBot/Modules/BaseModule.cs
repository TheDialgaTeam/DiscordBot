using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("Base")]
    public sealed class BaseModule : ModuleHelper
    {
        private IDiscordAppService DiscordAppService { get; }

        private ISQLiteService SQLiteService { get; }

        public BaseModule(IDiscordAppService discordAppService, ISQLiteService sqliteService)
        {
            DiscordAppService = discordAppService;
            SQLiteService = sqliteService;
        }

        [Command("Ping")]
        [Summary("Check the connection speed between the bot and the server.")]
        public async Task Ping()
        {
            await ReplyAsync($"Ping: {Context.Client.Latency} ms");
        }

        [Command("About")]
        [Summary("Get the bot information.")]
        public async Task About()
        {
            var helpMessage = new EmbedBuilder()
                              .WithTitle("The Dialga Team Discord Bot:")
                              .WithThumbnailUrl(Context.User.GetAvatarUrl())
                              .WithColor(Color.Orange)
                              .WithDescription($@"Hello, I am **{Context.Client.CurrentUser.Username}**, a multipurpose bot that is hosted in AGN.
I am owned by **{(await Context.Client.GetApplicationInfoAsync()).Owner.Username}**.
Type {Context.Client.CurrentUser.Mention} help to see my command.

You can invite this bot by using this link: <https://discordapp.com/api/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&permissions=0&scope=bot>

If you want to have a custom avatar and bot name, feel free to join our bot discord server <https://discord.me/TheDialgaTeam> and read the invite section.");

            if (Context.Message.Channel is SocketDMChannel || Context.Message.Channel is SocketGroupChannel)
                await ReplyAsync("", false, helpMessage.Build());
            else
            {
                var dmChannel = await Context.Message.Author.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync("", false, helpMessage.Build());
            }
        }

        [Command("GlobalBotAnnounce")]
        [Summary("Announce a message into all of the guilds of all bot instances.")]
        [RequirePermission(RequiredPermissions.GlobalDiscordAppOwner)]
        public async Task GlobalBotAnnounce([Remainder] [Summary("Message to send.")] string message)
        {
            var ignoreGuilds = new List<ulong>();

            foreach (var discordSocketClientModel in DiscordAppService.DiscordSocketClientModels)
            {
                foreach (var socketGuild in discordSocketClientModel.DiscordSocketClient.Guilds)
                {
                    if (ignoreGuilds.Contains(socketGuild.Id))
                        continue;

                    var perms = socketGuild.GetUser(discordSocketClientModel.DiscordSocketClient.CurrentUser.Id).GetPermissions(socketGuild.DefaultChannel);

                    if (perms.SendMessages)
                        await socketGuild.DefaultChannel.SendMessageAsync(message);

                    ignoreGuilds.Add(socketGuild.Id);
                }
            }
        }

        [Command("BotAnnounce")]
        [Summary("Announce a message into all of the guilds of this bot instance.")]
        [RequirePermission(RequiredPermissions.DiscordAppOwner)]
        public async Task BotAnnounce([Remainder] [Summary("Message to send.")] string message)
        {
            foreach (var socketGuild in Context.Client.Guilds)
            {
                var perms = Context.Guild.GetUser(Context.Client.CurrentUser.Id).GetPermissions(Context.Guild.DefaultChannel);

                if (perms.SendMessages)
                    await socketGuild.DefaultChannel.SendMessageAsync(message);
            }
        }

        [Command("GuildSay")]
        [Summary("Announce a message into this guild.")]
        [RequirePermission(RequiredPermissions.GuildModerator)]
        [RequireContext(ContextType.Guild)]
        public async Task GuildSay([Remainder] [Summary("Message to send.")] string message)
        {
            var perms = Context.Guild.GetUser(Context.Client.CurrentUser.Id).GetPermissions(Context.Guild.DefaultChannel);

            if (perms.SendMessages)
                await Context.Guild.DefaultChannel.SendMessageAsync(message);
        }

        [Command("ChannelSay")]
        [Summary("Announce a message into this channel.")]
        [RequirePermission(RequiredPermissions.ChannelModerator)]
        [RequireContext(ContextType.Guild)]
        public async Task ChannelSay([Remainder] [Summary("Message to send.")] string message)
        {
            await ReplyAsync(message);
        }

        [Command("AddDiscordApp")]
        [Summary("Add a new bot instance.")]
        [RequirePermission(RequiredPermissions.GlobalDiscordAppOwner)]
        public async Task AddDiscordApp([Summary("Discord bot client id.")] ulong clientId, [Remainder] [Summary("Bot secret token.")] string botToken)
        {
            var stringClientId = clientId.ToString();

            var discordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == stringClientId).FirstOrDefaultAsync() ?? new DiscordAppModel { ClientId = stringClientId };
            discordAppModel.SetBotToken(botToken);

            if (discordAppModel.Id == default(int))
            {
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(discordAppModel);
                await ReplyAsync(":white_check_mark: Successfully added new discord app.");
            }
            else
            {
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordAppModel);
                await ReplyAsync(":white_check_mark: Successfully updated the discord app.");
            }
        }

        [Command("RemoveDiscordApp")]
        [Summary("Remove a bot instance.")]
        [RequirePermission(RequiredPermissions.GlobalDiscordAppOwner)]
        public async Task RemoveDiscordApp([Summary("Discord bot client id.")] ulong clientId)
        {
            var stringClientId = clientId.ToString();
            var discordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == stringClientId).FirstOrDefaultAsync();

            if (discordAppModel == null)
            {
                await ReplyAsync(":negative_squared_cross_mark: This discord app is does not exist!");
                return;
            }

            await SQLiteService.SQLiteAsyncConnection.DeleteAsync(discordAppModel);
            await ReplyAsync(":white_check_mark: Successfully remove the discord app.");
        }

        [Command("StartDiscordApp")]
        [Summary("Start a new bot instance")]
        [RequirePermission(RequiredPermissions.GlobalDiscordAppOwner)]
        public async Task StartDiscordApp([Summary("Discord bot client id.")] ulong clientId)
        {
            if (await DiscordAppService.StartDiscordApp(clientId))
                await ReplyAsync(":white_check_mark: Successfully started the discord app.");
            else
                await ReplyAsync(":negative_squared_cross_mark: This discord app is does not exist!");
        }

        [Command("StopDiscordApp")]
        [Summary("Stop a bot instance")]
        [RequirePermission(RequiredPermissions.GlobalDiscordAppOwner)]
        public async Task StopDiscordApp([Summary("Discord bot client id.")] ulong clientId)
        {
            if (await DiscordAppService.StopDiscordApp(clientId))
                await ReplyAsync(":white_check_mark: Successfully stopped the discord app.");
            else
                await ReplyAsync(":negative_squared_cross_mark: This discord app is not running!");
        }
    }
}