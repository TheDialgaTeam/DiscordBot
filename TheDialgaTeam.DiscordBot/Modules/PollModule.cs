using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;
using TheDialgaTeam.DiscordBot.Services;
using TheDialgaTeam.DiscordBot.Services.SQLite;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("Poll")]
    public sealed class PollModule : ModuleHelper
    {
        private ISQLiteService SQLiteService { get; }

        private IPollHandlerService PollHandlerService { get; }

        public PollModule(ISQLiteService sqliteService, IPollHandlerService pollHandlerService)
        {
            SQLiteService = sqliteService;
            PollHandlerService = pollHandlerService;
        }

        [Command("PollAbout")]
        [Summary("Find out more about Poll module.")]
        public async Task PollAboutAsync()
        {
            var helpMessage = new EmbedBuilder()
                              .WithTitle("Poll Module:")
                              .WithColor(Color.Orange)
                              .WithDescription(@"This module allow administrator or moderator to setup poll for people to vote for their choice.

Polls can be run without a time limit and allow multiple choices. (People can vote all options even if it is intended to be a single choice) This is still experimental but will be updated with better feature next time.");

            await ReplyAsync("", false, helpMessage.Build());
        }

        [Command("StartPoll")]
        [Summary("Start a new poll.")]
        [RequireContext(ContextType.Guild)]
        public async Task StartPollAsync([Summary("Title of the poll.")] string title, [Summary("Poll message.")] string message, [Summary("Poll options. Seperated by `;`")] string options, [Summary("Poll duration.")] TimeSpan duration, [Summary("List of emojis for the poll. (Do not need to be comma seperated)")] params IEmote[] emotes)
        {
            if (duration.TotalSeconds < 60)
            {
                await ReplyAsync(":negative_squared_cross_mark: Unable to build poll duration. Ensure that the duration is at least 1 minute long.");
                return;
            }

            var pollOptions = PollOptionsBuilder(options, emotes);

            if (pollOptions == null)
            {
                await ReplyAsync(":negative_squared_cross_mark: Unable to build poll options. Ensure that the number of options and emoji matches.");
                return;
            }

            var embedMessage = new EmbedBuilder()
                               .WithTitle(title)
                               .WithDescription(message)
                               .WithColor(Color.Orange)
                               .WithFooter("Building poll... Please wait. (If it takes too long, try again later)")
                               .AddField("Options:", pollOptions);

            var sentMessage = await ReplyAsync("", false, embedMessage.Build());

            foreach (var emote in emotes)
                await sentMessage.AddReactionAsync(emote);

            var startDateTime = DateTimeOffset.Now;

            // Start the poll after all reaction is ready.
            await sentMessage.ModifyAsync(a => a.Embed = embedMessage.WithFooter("Ends at").WithTimestamp(startDateTime.Add(duration)).Build());

            // Cache Poll to allow bot instance to restart with no issues.
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();
            var channelId = Context.Channel.Id.ToString();
            var messageId = sentMessage.Id.ToString();
            var pollModel = new PollModel { ClientId = clientId, GuildId = guildId, ChannelId = channelId, MessageId = messageId, StartDateTime = startDateTime, Duration = duration };

            await SQLiteService.SQLiteAsyncConnection.InsertAsync(pollModel);
        }

        private static string PollOptionsBuilder(string options, IReadOnlyList<IEmote> emotes)
        {
            var stringBuilder = new StringBuilder();
            var splitOptions = options.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (splitOptions.Length != emotes.Count)
                return null;

            for (var i = 0; i < splitOptions.Length; i++)
            {
                switch (emotes[i])
                {
                    case Emote emote:
                        stringBuilder.AppendLine($"{emote}: {splitOptions[i]}");
                        break;

                    case Emoji emoji:
                        stringBuilder.AppendLine($"{emoji}: {splitOptions[i]}");
                        break;
                }
            }

            return stringBuilder.ToString();
        }
    }
}