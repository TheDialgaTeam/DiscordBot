using Discord.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Poll;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;

namespace TheDialgaTeam.DiscordBot.Services
{
    public interface IPollHandlerService
    {
        Task UpdatePollTask();
    }

    internal sealed class PollHandlerService : IPollHandlerService
    {
        private ISQLiteService SQLiteService { get; }

        private IDiscordAppService DiscordAppService { get; }

        private ICollection UserPollMessagesCollection { get; }

        private List<IUserPollMessage> UserPollMessages { get; } = new List<IUserPollMessage>();

        public PollHandlerService(ISQLiteService sqliteService, IDiscordAppService discordAppService)
        {
            SQLiteService = sqliteService;
            DiscordAppService = discordAppService;
            UserPollMessagesCollection = UserPollMessages;
        }

        public async Task UpdatePollTask()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                if (stopwatch.Elapsed.TotalMilliseconds < 1000)
                {
                    Thread.Sleep(1);
                    continue;
                }

                var pollModels = await SQLiteService.SQLiteAsyncConnection.Table<PollModel>().ToArrayAsync();

                if (pollModels.Length == 0)
                {
                    stopwatch.Restart();
                    continue;
                }

                foreach (var pollModel in pollModels)
                    await AddPollToWatch(pollModel);

                foreach (var userPollMessage in UserPollMessages)
                {
                    if (!userPollMessage.IsEnded())
                    {
                        stopwatch.Restart();
                        continue;
                    }

                    await userPollMessage.UpdatePollCount();
                    await SQLiteService.SQLiteAsyncConnection.DeleteAsync(userPollMessage.PollModel);
                }

                UserPollMessages.Clear();
                stopwatch.Restart();
            } while (true);
        }

        private async Task AddPollToWatch(IPollModel pollModel)
        {
            foreach (var discordSocketClientModel in DiscordAppService.DiscordSocketClientModels)
            {
                if (discordSocketClientModel.DiscordAppModel.ClientId != pollModel.ClientId)
                    continue;

                var guild = discordSocketClientModel.DiscordSocketClient.GetGuild(Convert.ToUInt64(pollModel.GuildId));
                var channel = guild?.GetTextChannel(Convert.ToUInt64(pollModel.ChannelId));

                if (channel == null)
                {
                    await SQLiteService.SQLiteAsyncConnection.DeleteAsync(pollModel);
                    return;
                }

                var message = await channel.GetMessageAsync(Convert.ToUInt64(pollModel.MessageId));

                if (message is RestUserMessage restUserMessage)
                    AddPollToWatch(new UserPollMessage(restUserMessage, pollModel));
                else
                    await SQLiteService.SQLiteAsyncConnection.DeleteAsync(pollModel);

                break;
            }
        }

        private void AddPollToWatch(IUserPollMessage userPollMessage)
        {
            lock (UserPollMessagesCollection.SyncRoot)
                UserPollMessages.Add(userPollMessage);
        }
    }
}