using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Model.Discord.User.Enum;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("Base Module")]
    public sealed class BaseModule : ModuleBase<SocketCommandContext>
    {
        private IDiscordAppService DiscordAppService { get; }

        public BaseModule(IDiscordAppService discordAppService)
        {
            DiscordAppService = discordAppService;
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

You can invite this bot using this link: https://discordapp.com/api/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&permissions=0&scope=bot");

            var dmChannel = await Context.Message.Author.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("", false, helpMessage.Build());
        }

        [Command("GlobalBotAnnounce")]
        [Summary("Announce a message into all of the guilds of all bot instances.")]
        [RequirePermission(UserPermissions.GlobalDiscordAppOwner)]
        public async Task GlobalBotAnnounce([Remainder] [Summary("Message to send.")] string message)
        {
            var ignoreGuilds = new List<ulong>();

            foreach (var discordSocketClientModel in DiscordAppService.DiscordSocketClientModels)
            {
                foreach (var socketGuild in discordSocketClientModel.DiscordSocketClient.Guilds)
                {
                    if (ignoreGuilds.Contains(socketGuild.Id))
                        continue;

                    var perms = socketGuild.CurrentUser.GetPermissions(socketGuild.DefaultChannel);

                    if (perms.SendMessages)
                        await socketGuild.DefaultChannel.SendMessageAsync(message);

                    ignoreGuilds.Add(socketGuild.Id);
                }
            }
        }

        [Command("BotAnnounce")]
        [Summary("Announce a message into all of the guilds of this bot instance.")]
        [RequirePermission(UserPermissions.DiscordAppOwner)]
        public async Task BotAnnounce([Remainder] [Summary("Message to send.")] string message)
        {
            foreach (var socketGuild in Context.Client.Guilds)
            {
                var perms = socketGuild.CurrentUser.GetPermissions(socketGuild.DefaultChannel);

                if (perms.SendMessages)
                    await socketGuild.DefaultChannel.SendMessageAsync(message);
            }
        }

        [Command("GuildSay")]
        [Summary("Announce a message into this guild.")]
        [RequirePermission(UserPermissions.GuildModerator)]
        [RequireContext(ContextType.Guild)]
        public async Task GuildSay([Remainder] [Summary("Message to send.")] string message)
        {
            var perms = Context.Guild.CurrentUser.GetPermissions(Context.Guild.DefaultChannel);

            if (perms.SendMessages)
                await Context.Guild.DefaultChannel.SendMessageAsync(message);
        }

        [Command("ChannelSay")]
        [Summary("Announce a message into this channel.")]
        [RequirePermission(UserPermissions.ChannelModerator)]
        [RequireContext(ContextType.Guild)]
        public async Task ChannelSay([Remainder] [Summary("Message to send.")] string message)
        {
            await ReplyAsync(message);
        }

        protected override async void AfterExecute(CommandInfo command)
        {
            if (Context.Message.Channel is IDMChannel || Context.Message.Channel is IGroupChannel)
                return;

            var perms = Context.Guild.CurrentUser.GetPermissions(Context.Guild.DefaultChannel);

            if (perms.ManageMessages)
                await Context.Message.DeleteAsync();
        }
    }
}