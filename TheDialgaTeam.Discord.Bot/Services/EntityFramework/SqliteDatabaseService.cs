using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TheDialgaTeam.Discord.Bot.Models.Discord;
using TheDialgaTeam.Discord.Bot.Models.EntityFramework;
using TheDialgaTeam.Discord.Bot.Services.Console;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Services.EntityFramework
{
    public sealed class SqliteDatabaseService
    {
        private FilePathService FilePathService { get; }

        private LoggerService LoggerService { get; }

        public SqliteDatabaseService(FilePathService filePathService, LoggerService loggerService)
        {
            FilePathService = filePathService;
            LoggerService = loggerService;
        }

        public SqliteContext GetContext(bool readOnly = false)
        {
            return new SqliteContext(FilePathService, readOnly);
        }

        public async Task RebuildGuildChannelTableAsync(DiscordAppInstance discordAppInstance, SocketGuild socketGuild)
        {
            using (var context = GetContext())
            {
                var discordAppTable = await context.GetDiscordAppTableAsync(discordAppInstance.ClientId).ConfigureAwait(false);
                var discordGuildTable = await context.GetDiscordGuildTableAsync(discordAppInstance.ClientId, socketGuild.Id, DiscordGuildTableIncludedEntities.DiscordChannelTable).ConfigureAwait(false);

                if (discordGuildTable == null)
                {
                    discordGuildTable = new DiscordGuild { GuildId = socketGuild.Id, DiscordAppId = discordAppTable?.DiscordAppId ?? 0, DiscordChannels = new List<DiscordChannel>() };

                    foreach (var socketGuildChannel in socketGuild.Channels)
                        discordGuildTable.DiscordChannels.Add(new DiscordChannel { ChannelId = socketGuildChannel.Id });

                    context.DiscordGuildTable.Add(discordGuildTable);
                }
                else
                {
                    if (discordGuildTable.DiscordChannels != null)
                    {
                        context.DiscordChannelTable.RemoveRange(discordGuildTable.DiscordChannels.Where(a => !socketGuild.Channels.Where(b => b is SocketTextChannel).Select(b => b.Id).Contains(a.ChannelId)));

                        foreach (var socketGuildChannel in socketGuild.Channels.Where(a => a is SocketTextChannel))
                        {
                            if (discordGuildTable.DiscordChannels.Any(a => a.ChannelId == socketGuildChannel.Id))
                                continue;

                            discordGuildTable.DiscordChannels.Add(new DiscordChannel { ChannelId = socketGuildChannel.Id });
                        }
                    }
                    else
                    {
                        discordGuildTable.DiscordChannels = new List<DiscordChannel>();

                        foreach (var socketGuildChannel in socketGuild.Channels.Where(a => a is SocketTextChannel))
                            discordGuildTable.DiscordChannels.Add(new DiscordChannel { ChannelId = socketGuildChannel.Id });
                    }

                    context.DiscordGuildTable.Update(discordGuildTable);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            LoggerService.LogMessage(discordAppInstance.DiscordShardedClient, new LogMessage(LogSeverity.Info, "", $"{socketGuild} have been synced into the database."));
        }

        public async Task RemoveGuildChannelAsync(DiscordAppInstance discordAppInstance, SocketGuild socketGuild)
        {
            using (var context = GetContext())
            {
                var discordGuild = await context.GetDiscordGuildTableAsync(discordAppInstance.ClientId, socketGuild.Id).ConfigureAwait(false);

                if (discordGuild != null)
                    context.Remove(discordGuild);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}