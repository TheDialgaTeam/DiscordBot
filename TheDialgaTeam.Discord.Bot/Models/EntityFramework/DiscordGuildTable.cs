using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordGuildTable
    {
        [Key]
        public ulong? Id { get; set; }

        public ulong? GuildId { get; set; }

        public string Prefix { get; set; }

        public ulong? DiscordAppId { get; set; }

        public DiscordAppTable DiscordApp { get; set; }

        [InverseProperty(nameof(DiscordGuildModeratorTable.DiscordGuild))]
        public List<DiscordGuildModeratorTable> DiscordGuildModerators { get; set; }

        [InverseProperty(nameof(DiscordGuildModuleTable.DiscordGuild))]
        public List<DiscordGuildModuleTable> DiscordGuildModules { get; set; }

        [InverseProperty(nameof(DiscordChannelTable.DiscordGuild))]
        public List<DiscordChannelTable> DiscordChannels { get; set; }
    }
}