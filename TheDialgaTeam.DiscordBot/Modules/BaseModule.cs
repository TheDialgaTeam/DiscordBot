using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("Base Module")]
    public sealed class BaseModule : ModuleBase<SocketCommandContext>
    {
        public IDiscordSocketClientService DiscordSocketClientService { get; }

        public BaseModule(IDiscordSocketClientService discordSocketClientService)
        {
            DiscordSocketClientService = discordSocketClientService;
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
            await dmChannel.SendMessageAsync("", false, helpMessage);
        }

        [Command("GlobalBotAnnounce")]
        [Summary("Announce a message into all of the guilds of all bot instances.")]
        [RequireOwner]
        public async Task GlobalBotAnnounce([Remainder] [Summary("Message to send.")] string message)
        {
            var ignoreGuilds = new List<ulong>();

            foreach (var discordSocketClientModel in DiscordSocketClientService.DiscordSocketClientModels)
            {
                foreach (var socketGuild in discordSocketClientModel.DiscordSocketClient.Guilds)
                {
                    if (ignoreGuilds.Contains(socketGuild.Id)) continue;

                    var perms = socketGuild.CurrentUser.GetPermissions(socketGuild.DefaultChannel);

                    if (perms.SendMessages)
                        await socketGuild.DefaultChannel.SendMessageAsync(message);

                    ignoreGuilds.Add(socketGuild.Id);
                }
            }
        }
    }
}