using Discord;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;
using TheDialgaTeam.DiscordBot.Services.Discord;
using TheDialgaTeam.DiscordBot.Services.SQLite;

namespace TheDialgaTeam.DiscordBot.Services
{
    public interface IPollHandlerService
    {
        void UpdatePollTask();
    }

    internal sealed class PollHandlerService : IPollHandlerService
    {
        private ISQLiteService SQLiteService { get; }

        private IDiscordAppService DiscordAppService { get; }

        public PollHandlerService(ISQLiteService sqliteService, IDiscordAppService discordAppService)
        {
            SQLiteService = sqliteService;
            DiscordAppService = discordAppService;
        }

        public async void UpdatePollTask()
        {
            var lastUpdateTime = DateTime.Now;

            do
            {
                if (DateTime.Now <= lastUpdateTime.AddSeconds(1))
                    await Task.Delay(1);

                lastUpdateTime = DateTime.Now;

                var pollModels = await SQLiteService.SQLiteAsyncConnection.Table<PollModel>().ToArrayAsync();

                if (pollModels.Length == 0)
                    continue;

                foreach (var pollModel in pollModels)
                {
                    if (DateTimeOffset.Now <= pollModel.StartDateTime.Add(pollModel.Duration))
                        continue;

                    foreach (var discordSocketClientModel in DiscordAppService.DiscordShardedClientModels)
                    {
                        if (discordSocketClientModel.DiscordAppModel.ClientId != pollModel.ClientId)
                            continue;

                        //if (!discordSocketClientModel.IsReady)
                        //    continue;

                        var guild = discordSocketClientModel.DiscordShardedClient.GetGuild(Convert.ToUInt64(pollModel.GuildId));
                        var channel = guild?.GetTextChannel(Convert.ToUInt64(pollModel.ChannelId));

                        if (channel == null)
                        {
                            await SQLiteService.SQLiteAsyncConnection.DeleteAsync(pollModel);
                            break;
                        }

                        var message = await channel.GetMessageAsync(Convert.ToUInt64(pollModel.MessageId));

                        if (message == null || !(message is RestUserMessage restUserMessage))
                        {
                            await SQLiteService.SQLiteAsyncConnection.DeleteAsync(pollModel);
                            break;
                        }

                        foreach (var userMessageEmbed in restUserMessage.Embeds)
                        {
                            var embedBuilder = userMessageEmbed.ToEmbedBuilder()
                                                               .AddField("Poll Results:", GetReactionResult(restUserMessage.Reactions))
                                                               .WithFooter("Poll ended");

                            await restUserMessage.ModifyAsync(a => a.Embed = embedBuilder.Build());
                            await restUserMessage.RemoveAllReactionsAsync();
                            break;
                        }

                        await SQLiteService.SQLiteAsyncConnection.DeleteAsync(pollModel);
                        break;
                    }
                }
            } while (true);
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

            return result.Length == 0 ? "Unexpected error have occured!" : result.ToString();
        }
    }
}