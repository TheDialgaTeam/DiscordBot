using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.Discord.Bot.Models.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Services.EntityFramework
{
    public static class SqliteContextExtension
    {
        public static async Task<DiscordGuildTable> GetDiscordGuildTableAsync(this SqliteContext sqliteContext, ulong clientId, ulong guildId,
                                                                              bool includeDiscordApp = false, bool includeDiscordAppOwner = false,
                                                                              bool includeDiscordGuildModerators = false,
                                                                              bool includeDiscordGuildModules = false,
                                                                              bool includeDiscordChannels = false)
        {
            var data = (await sqliteContext.DiscordAppTable.Where(a => a.ClientId == clientId)
                                           .Select(a => new
                                           {
                                               discordApp = a,
                                               discordAppOwner = a.DiscordAppOwners,
                                               discordGuild = a.DiscordGuilds.FirstOrDefault(b => b.GuildId == guildId),
                                               discordGuildModerators = a.DiscordGuilds.FirstOrDefault(b => b.GuildId == guildId).DiscordGuildModerators,
                                               discordGuildModules = a.DiscordGuilds.FirstOrDefault(b => b.GuildId == guildId).DiscordGuildModules,
                                               discordChannels = a.DiscordGuilds.FirstOrDefault(b => b.GuildId == guildId).DiscordChannels
                                           }).ToListAsync().ConfigureAwait(false)).FirstOrDefault();

            var discordGuild = data?.discordGuild;

            if (discordGuild == null)
                return null;

            if (includeDiscordApp)
                discordGuild.DiscordApp = data.discordApp;

            if (includeDiscordApp && includeDiscordAppOwner)
            {
                foreach (var discordAppOwnerTable in data.discordAppOwner)
                    discordAppOwnerTable.DiscordApp = data.discordApp;

                discordGuild.DiscordApp.DiscordAppOwners = data.discordAppOwner;
            }

            if (includeDiscordGuildModerators)
            {
                foreach (var discordGuildModeratorTable in data.discordGuildModerators)
                    discordGuildModeratorTable.DiscordGuild = discordGuild;

                discordGuild.DiscordGuildModerators = data.discordGuildModerators;
            }

            if (includeDiscordGuildModules)
            {
                foreach (var discordGuildModuleTable in data.discordGuildModules)
                    discordGuildModuleTable.DiscordGuild = discordGuild;

                discordGuild.DiscordGuildModules = data.discordGuildModules;
            }

            if (includeDiscordChannels)
            {
                foreach (var discordChannelTable in data.discordChannels)
                    discordChannelTable.DiscordGuild = discordGuild;

                discordGuild.DiscordChannels = data.discordChannels;
            }

            return discordGuild;
        }
    }
}