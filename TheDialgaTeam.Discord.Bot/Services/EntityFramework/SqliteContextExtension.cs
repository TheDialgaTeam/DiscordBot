using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.Discord.Bot.Models.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Services.EntityFramework
{
    public static class SqliteContextExtension
    {
        public static async Task<DiscordApp> GetDiscordAppTableAsync(this SqliteContext sqliteContext, ulong clientId, DiscordAppTableIncludedEntities discordAppTableIncludedEntities = DiscordAppTableIncludedEntities.None)
        {
            var discordAppTable = await sqliteContext.DiscordAppTable.FirstOrDefaultAsync(a => a.ClientId == clientId).ConfigureAwait(false);

            if (discordAppTable == null)
                return null;

            if ((discordAppTableIncludedEntities & DiscordAppTableIncludedEntities.DiscordAppOwnerTable) == DiscordAppTableIncludedEntities.DiscordAppOwnerTable)
            {
                var discordAppOwnerTables = await sqliteContext.DiscordAppOwnerTable.Where(a => a.DiscordAppId == discordAppTable.DiscordAppId).ToListAsync().ConfigureAwait(false);
                discordAppTable.DiscordAppOwners = discordAppOwnerTables;

                foreach (var discordAppOwnerTable in discordAppTable.DiscordAppOwners)
                    discordAppOwnerTable.DiscordApp = discordAppTable;
            }

            if ((discordAppTableIncludedEntities & DiscordAppTableIncludedEntities.DiscordGuildTable) == DiscordAppTableIncludedEntities.DiscordGuildTable)
            {
                var discordGuildTables = await sqliteContext.DiscordGuildTable.Where(a => a.DiscordAppId == discordAppTable.DiscordAppId).ToListAsync().ConfigureAwait(false);
                discordAppTable.DiscordGuilds = discordGuildTables;

                foreach (var discordGuildTable in discordAppTable.DiscordGuilds)
                    discordGuildTable.DiscordApp = discordAppTable;
            }

            if ((discordAppTableIncludedEntities & DiscordAppTableIncludedEntities.DiscordGuildModeratorTable) == DiscordAppTableIncludedEntities.DiscordGuildModeratorTable)
            {
                foreach (var discordGuildTable in discordAppTable.DiscordGuilds)
                {
                    var discordGuildModeratorTables = await sqliteContext.DiscordGuildModeratorTable.Where(a => a.DiscordGuildId == discordGuildTable.DiscordGuildId).ToListAsync().ConfigureAwait(false);
                    discordGuildTable.DiscordGuildModerators = discordGuildModeratorTables;

                    foreach (var discordGuildModeratorTable in discordGuildTable.DiscordGuildModerators)
                        discordGuildModeratorTable.DiscordGuild = discordGuildTable;
                }
            }

            if ((discordAppTableIncludedEntities & DiscordAppTableIncludedEntities.DiscordGuildModuleTable) == DiscordAppTableIncludedEntities.DiscordGuildModuleTable)
            {
                foreach (var discordGuildTable in discordAppTable.DiscordGuilds)
                {
                    var discordGuildModuleTables = await sqliteContext.DiscordGuildModuleTable.Where(a => a.DiscordGuildId == discordGuildTable.DiscordGuildId).ToListAsync().ConfigureAwait(false);
                    discordGuildTable.DiscordGuildModules = discordGuildModuleTables;

                    foreach (var discordGuildModuleTable in discordGuildTable.DiscordGuildModules)
                        discordGuildModuleTable.DiscordGuild = discordGuildTable;
                }
            }

            if ((discordAppTableIncludedEntities & DiscordAppTableIncludedEntities.DiscordChannelTable) == DiscordAppTableIncludedEntities.DiscordChannelTable)
            {
                foreach (var discordGuildTable in discordAppTable.DiscordGuilds)
                {
                    var discordChannelTables = await sqliteContext.DiscordChannelTable.Where(a => a.DiscordGuildId == discordGuildTable.DiscordGuildId).ToListAsync().ConfigureAwait(false);
                    discordGuildTable.DiscordChannels = discordChannelTables;

                    foreach (var discordChannelTable in discordGuildTable.DiscordChannels)
                        discordChannelTable.DiscordGuild = discordGuildTable;
                }
            }

            if ((discordAppTableIncludedEntities & DiscordAppTableIncludedEntities.DiscordChannelModeratorTable) == DiscordAppTableIncludedEntities.DiscordChannelModeratorTable)
            {
                foreach (var discordGuildTable in discordAppTable.DiscordGuilds)
                {
                    foreach (var discordChannelTable in discordGuildTable.DiscordChannels)
                    {
                        var discordChannelModeratorTables = await sqliteContext.DiscordChannelModeratorTable.Where(a => a.DiscordChannelId == discordChannelTable.DiscordChannelId).ToListAsync().ConfigureAwait(false);
                        discordChannelTable.DiscordChannelModerators = discordChannelModeratorTables;

                        foreach (var discordChannelModeratorTable in discordChannelTable.DiscordChannelModerators)
                            discordChannelModeratorTable.DiscordChannel = discordChannelTable;
                    }
                }
            }

            return discordAppTable;
        }

        public static async Task<DiscordGuild> GetDiscordGuildTableAsync(this SqliteContext sqliteContext, ulong clientId, ulong guildId, DiscordGuildTableIncludedEntities discordGuildTableIncludedEntities = DiscordGuildTableIncludedEntities.None)
        {
            var data = await sqliteContext.DiscordAppTable.Where(a => a.ClientId == clientId).Select(a => new
            {
                discordAppTable = a,
                discordGuildTable = a.DiscordGuilds.FirstOrDefault(b => b.GuildId == guildId)
            }).FirstOrDefaultAsync().ConfigureAwait(false);

            var discordAppTable = data.discordAppTable;
            var discordGuildTable = data.discordGuildTable;

            if (discordGuildTable == null)
                return null;

            if ((discordGuildTableIncludedEntities & DiscordGuildTableIncludedEntities.DiscordAppTable) == DiscordGuildTableIncludedEntities.DiscordAppTable)
                discordGuildTable.DiscordApp = discordAppTable;

            if ((discordGuildTableIncludedEntities & DiscordGuildTableIncludedEntities.DiscordGuildModeratorTable) == DiscordGuildTableIncludedEntities.DiscordGuildModeratorTable)
            {
                var discordGuildModeratorTables = await sqliteContext.DiscordGuildModeratorTable.Where(a => a.DiscordGuildId == discordGuildTable.DiscordGuildId).ToListAsync().ConfigureAwait(false);
                discordGuildTable.DiscordGuildModerators = discordGuildModeratorTables;

                foreach (var discordGuildModeratorTable in discordGuildTable.DiscordGuildModerators)
                    discordGuildModeratorTable.DiscordGuild = discordGuildTable;
            }

            if ((discordGuildTableIncludedEntities & DiscordGuildTableIncludedEntities.DiscordGuildModuleTable) == DiscordGuildTableIncludedEntities.DiscordGuildModuleTable)
            {
                var discordGuildModuleTables = await sqliteContext.DiscordGuildModuleTable.Where(a => a.DiscordGuildId == discordGuildTable.DiscordGuildId).ToListAsync().ConfigureAwait(false);
                discordGuildTable.DiscordGuildModules = discordGuildModuleTables;

                foreach (var discordGuildModuleTable in discordGuildTable.DiscordGuildModules)
                    discordGuildModuleTable.DiscordGuild = discordGuildTable;
            }

            if ((discordGuildTableIncludedEntities & DiscordGuildTableIncludedEntities.DiscordChannelTable) == DiscordGuildTableIncludedEntities.DiscordChannelTable)
            {
                var discordChannelTables = await sqliteContext.DiscordChannelTable.Where(a => a.DiscordGuildId == discordGuildTable.DiscordGuildId).ToListAsync().ConfigureAwait(false);
                discordGuildTable.DiscordChannels = discordChannelTables;

                foreach (var discordChannelTable in discordGuildTable.DiscordChannels)
                    discordChannelTable.DiscordGuild = discordGuildTable;
            }

            if ((discordGuildTableIncludedEntities & DiscordGuildTableIncludedEntities.DiscordChannelModeratorTable) == DiscordGuildTableIncludedEntities.DiscordChannelModeratorTable)
            {
                foreach (var discordChannelTable in discordGuildTable.DiscordChannels)
                {
                    var discordChannelModeratorTables = await sqliteContext.DiscordChannelModeratorTable.Where(a => a.DiscordChannelId == discordChannelTable.DiscordChannelId).ToListAsync().ConfigureAwait(false);
                    discordChannelTable.DiscordChannelModerators = discordChannelModeratorTables;

                    foreach (var discordChannelModeratorTable in discordChannelTable.DiscordChannelModerators)
                        discordChannelModeratorTable.DiscordChannel = discordChannelTable;
                }
            }

            return discordGuildTable;
        }

        public static async Task<DiscordChannel> GetDiscordChannelTableAsync(this SqliteContext sqliteContext, ulong clientId, ulong guildId, ulong channelId, DiscordChannelTableIncludedEntities discordChannelTableIncludedEntities = DiscordChannelTableIncludedEntities.None)
        {
            var data = await sqliteContext.DiscordAppTable.Where(a => a.ClientId == clientId).Select(a => new
            {
                discordAppTable = a,
                discordGuildTable = a.DiscordGuilds.FirstOrDefault(b => b.GuildId == guildId),
                discordChannelTable = a.DiscordGuilds.FirstOrDefault(b => b.GuildId == guildId).DiscordChannels.FirstOrDefault(b => b.ChannelId == channelId)
            }).FirstOrDefaultAsync().ConfigureAwait(false);

            var discordAppTable = data.discordAppTable;
            var discordGuildTable = data.discordGuildTable;
            var discordChannelTable = data.discordChannelTable;

            if (discordChannelTable == null)
                return null;

            if ((discordChannelTableIncludedEntities & DiscordChannelTableIncludedEntities.DiscordGuildTable) == DiscordChannelTableIncludedEntities.DiscordGuildTable)
                discordChannelTable.DiscordGuild = discordGuildTable;

            if ((discordChannelTableIncludedEntities & DiscordChannelTableIncludedEntities.DiscordGuildModeratorTable) == DiscordChannelTableIncludedEntities.DiscordGuildModeratorTable)
            {
                var discordGuildModeratorTables = await sqliteContext.DiscordGuildModeratorTable.Where(a => a.DiscordGuildId == discordGuildTable.DiscordGuildId).ToListAsync().ConfigureAwait(false);
                discordChannelTable.DiscordGuild.DiscordGuildModerators = discordGuildModeratorTables;

                foreach (var discordGuildModeratorTable in discordGuildModeratorTables)
                    discordGuildModeratorTable.DiscordGuild = discordChannelTable.DiscordGuild;
            }

            if ((discordChannelTableIncludedEntities & DiscordChannelTableIncludedEntities.DiscordGuildModuleTable) == DiscordChannelTableIncludedEntities.DiscordGuildModuleTable)
            {
                var discordGuildModuleTables = await sqliteContext.DiscordGuildModuleTable.Where(a => a.DiscordGuildId == discordGuildTable.DiscordGuildId).ToListAsync().ConfigureAwait(false);
                discordChannelTable.DiscordGuild.DiscordGuildModules = discordGuildModuleTables;

                foreach (var discordGuildModuleTable in discordGuildModuleTables)
                    discordGuildModuleTable.DiscordGuild = discordChannelTable.DiscordGuild;
            }

            if ((discordChannelTableIncludedEntities & DiscordChannelTableIncludedEntities.DiscordAppTable) == DiscordChannelTableIncludedEntities.DiscordAppTable)
                discordChannelTable.DiscordGuild.DiscordApp = discordAppTable;

            if ((discordChannelTableIncludedEntities & DiscordChannelTableIncludedEntities.DiscordAppOwnerTable) == DiscordChannelTableIncludedEntities.DiscordAppOwnerTable)
            {
                var discordAppOwnerTables = await sqliteContext.DiscordAppOwnerTable.Where(a => a.DiscordAppId == discordAppTable.DiscordAppId).ToListAsync().ConfigureAwait(false);
                discordChannelTable.DiscordGuild.DiscordApp.DiscordAppOwners = discordAppOwnerTables;

                foreach (var discordAppOwnerTable in discordAppOwnerTables)
                    discordAppOwnerTable.DiscordApp = discordChannelTable.DiscordGuild.DiscordApp;
            }

            if ((discordChannelTableIncludedEntities & DiscordChannelTableIncludedEntities.DiscordChannelModeratorTable) == DiscordChannelTableIncludedEntities.DiscordChannelModeratorTable)
            {
                var discordChannelModeratorTables = await sqliteContext.DiscordChannelModeratorTable.Where(a => a.DiscordChannelId == discordChannelTable.DiscordChannelId).ToListAsync().ConfigureAwait(false);
                discordChannelTable.DiscordChannelModerators = discordChannelModeratorTables;

                foreach (var discordChannelModeratorTable in discordChannelModeratorTables)
                    discordChannelModeratorTable.DiscordChannel = discordChannelTable;
            }

            return discordChannelTable;
        }
    }

    [Flags]
    public enum DiscordAppTableIncludedEntities
    {
        None = 0,
        DiscordAppOwnerTable = 1 << 0,
        DiscordGuildTable = 1 << 1,
        DiscordGuildModeratorTable = DiscordGuildTable | (1 << 2),
        DiscordGuildModuleTable = DiscordGuildTable | (1 << 3),
        DiscordChannelTable = DiscordGuildTable | (1 << 4),
        DiscordChannelModeratorTable = DiscordChannelTable | (1 << 5),
        All = DiscordAppOwnerTable | DiscordGuildTable | DiscordGuildModeratorTable | DiscordGuildModuleTable | DiscordChannelTable | DiscordChannelModeratorTable
    }

    [Flags]
    public enum DiscordGuildTableIncludedEntities
    {
        None = 0,
        DiscordAppTable = 1 << 0,
        DiscordAppOwnerTable = DiscordAppTable | (1 << 1),
        DiscordGuildModeratorTable = 1 << 2,
        DiscordGuildModuleTable = 1 << 3,
        DiscordChannelTable = 1 << 4,
        DiscordChannelModeratorTable = DiscordChannelTable | (1 << 5),
        All = DiscordAppTable | DiscordAppOwnerTable | DiscordGuildModeratorTable | DiscordGuildModuleTable | DiscordChannelTable | DiscordChannelModeratorTable
    }

    [Flags]
    public enum DiscordChannelTableIncludedEntities
    {
        None = 0,
        DiscordAppTable = DiscordGuildTable | (1 << 0),
        DiscordAppOwnerTable = DiscordAppTable | (1 << 1),
        DiscordGuildTable = 1 << 2,
        DiscordGuildModeratorTable = DiscordGuildTable | (1 << 3),
        DiscordGuildModuleTable = DiscordGuildTable | (1 << 4),
        DiscordChannelModeratorTable = 1 << 5,
        All = DiscordAppTable | DiscordAppOwnerTable | DiscordGuildTable | DiscordGuildModeratorTable | DiscordGuildModuleTable | DiscordChannelModeratorTable
    }
}