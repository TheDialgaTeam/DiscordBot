using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;

namespace TheDialgaTeam.DiscordBot.Model.Poll
{
    public interface IUserPollMessage
    {
        IUserMessage UserMessage { get; }

        IPollModel PollModel { get; }

        bool IsEnded();

        Task UpdatePollCount();
    }

    internal sealed class UserPollMessage : IUserPollMessage
    {
        public IUserMessage UserMessage { get; }

        public IPollModel PollModel { get; }

        public UserPollMessage(IUserMessage userMessage, IPollModel pollModel)
        {
            UserMessage = userMessage;
            PollModel = pollModel;
        }

        public bool IsEnded()
        {
            return DateTimeOffset.Now > PollModel.StartDateTime.Add(PollModel.Duration);
        }

        public async Task UpdatePollCount()
        {
            foreach (var userMessageEmbed in UserMessage.Embeds)
            {
                var embedBuilder = userMessageEmbed.ToEmbedBuilder()
                                                   .AddField("Poll Results:", GetReactionResult(UserMessage.Reactions));

                await UserMessage.ModifyAsync(a => a.Embed = embedBuilder.Build());
                await UserMessage.RemoveAllReactionsAsync();
                break;
            }
        }

        private static string GetReactionResult(IReadOnlyDictionary<IEmote, ReactionMetadata> reactions)
        {
            var result = new StringBuilder();

            foreach (var reaction in reactions)
            {
                switch (reaction.Key)
                {
                    case Emote emote:
                        result.AppendLine($"{emote}: {reaction.Value.ReactionCount - 1} votes.");
                        break;

                    case Emoji emoji:
                        result.AppendLine($"{emoji}: {reaction.Value.ReactionCount - 1} votes.");
                        break;
                }
            }

            return result.ToString();
        }
    }
}