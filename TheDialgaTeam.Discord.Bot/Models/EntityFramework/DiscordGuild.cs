using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordGuild
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordGuildId { get; set; }

        public ulong GuildId { get; set; }

        public List<DiscordGuildModerator> DiscordGuildModerators { get; set; }

        public List<DiscordAppGuild> DiscordAppGuilds { get; set; }

        public List<DiscordChannel> DiscordChannels { get; set; }

        public List<DiscordModuleRequirement> DiscordModuleRequirements { get; set; }

        public static async Task<DiscordGuild> SyncTableAsync(SqliteContext sqliteContext, SocketGuild socketGuild)
        {
            var discordGuild = await sqliteContext.DiscordGuildTable.Where(a => a.GuildId == socketGuild.Id).FirstOrDefaultAsync().ConfigureAwait(false);

            if (discordGuild != null)
                return discordGuild;

            discordGuild = new DiscordGuild { GuildId = socketGuild.Id };
            sqliteContext.DiscordGuildTable.Add(discordGuild);

            await sqliteContext.SaveChangesAsync().ConfigureAwait(false);

            return discordGuild;
        }
    }
}