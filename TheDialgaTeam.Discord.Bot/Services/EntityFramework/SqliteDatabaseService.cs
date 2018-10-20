using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.Discord.Bot.Models.Discord;
using TheDialgaTeam.Discord.Bot.Models.EntityFramework;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Services.EntityFramework
{
    public sealed class SqliteDatabaseService
    {
        private FilePathService FilePathService { get; }

        public SqliteDatabaseService(FilePathService filePathService)
        {
            FilePathService = filePathService;
        }

        public SqliteContext GetContext(bool readOnly = false)
        {
            return new SqliteContext(FilePathService, readOnly);
        }

        public async Task RebuildGuildChannelTableAsync(DiscordAppInstance discordAppInstance, SocketGuild socketGuild)
        {
            using (var context = GetContext())
            {
                var data = (await context.DiscordAppTable.Where(a => a.ClientId == discordAppInstance.ClientId)
                                         .Select(a => new
                                         {
                                             discordAppId = a.Id,
                                             discordGuild = a.DiscordGuilds.Where(b => b.GuildId == socketGuild.Id)
                                                             .Select(b => new
                                                             {
                                                                 discordGuild = b,
                                                                 discordChannel = b.DiscordChannels
                                                             })
                                         }).ToListAsync().ConfigureAwait(false))
                           .Select(a => new
                           {
                               a.discordAppId,
                               discordGuild = a.discordGuild.Select(b => b.discordGuild).FirstOrDefault(),
                               discordChannel = a.discordGuild.Select(b => b.discordChannel).FirstOrDefault()
                           }).FirstOrDefault();

                var discordGuildTable = data?.discordGuild;
                var discordChannelTables = data?.discordChannel;

                // Check if guild exist.
                if (discordGuildTable == null)
                {
                    if (discordChannelTables?.Count > 0)
                        context.DiscordChannelTable.RemoveRange(discordChannelTables);

                    discordGuildTable = new DiscordGuildTable { GuildId = socketGuild.Id, DiscordAppId = data?.discordAppId ?? 0 };
                    discordChannelTables = new List<DiscordChannelTable>();

                    foreach (var socketGuildChannel in socketGuild.Channels.Where(a => a is SocketTextChannel))
                        discordChannelTables.Add(new DiscordChannelTable { ChannelId = socketGuildChannel.Id });

                    discordGuildTable.DiscordChannels = discordChannelTables;

                    context.DiscordGuildTable.Add(discordGuildTable);
                }
                else
                {
                    discordGuildTable.DiscordChannels = new List<DiscordChannelTable>();

                    if (discordChannelTables != null)
                    {
                        context.DiscordChannelTable.RemoveRange(discordChannelTables.Where(a => !socketGuild.Channels.Where(b => b is SocketTextChannel).Select(b => b.Id).Contains(a.ChannelId)));

                        foreach (var socketGuildChannel in socketGuild.Channels.Where(a => a is SocketTextChannel))
                        {
                            if (discordChannelTables.Any(a => a.ChannelId == socketGuildChannel.Id))
                                continue;

                            discordGuildTable.DiscordChannels.Add(new DiscordChannelTable { ChannelId = socketGuildChannel.Id });
                        }
                    }
                    else
                    {
                        foreach (var socketGuildChannel in socketGuild.Channels.Where(a => a is SocketTextChannel))
                            discordGuildTable.DiscordChannels.Add(new DiscordChannelTable { ChannelId = socketGuildChannel.Id });
                    }

                    context.DiscordGuildTable.Update(discordGuildTable);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task RemoveGuildChannelAsync(DiscordAppInstance discordAppInstance, SocketGuild socketGuild)
        {
            using (var context = GetContext())
            {
                var discordGuild = (await context.DiscordAppTable.Where(a => a.ClientId == discordAppInstance.ClientId)
                                                 .Select(a => new
                                                 {
                                                     discordGuild = a.DiscordGuilds.FirstOrDefault(b => b.GuildId == socketGuild.Id)
                                                 }).ToListAsync().ConfigureAwait(false))
                                   .Select(a => a.discordGuild).FirstOrDefault();

                if (discordGuild != null)
                    context.Remove(discordGuild);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}