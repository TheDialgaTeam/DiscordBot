using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordChannelTable
    {
        [Key]
        public ulong? Id { get; set; }

        public ulong? ChannelId { get; set; }

        public ulong? DiscordGuildId { get; set; }

        public DiscordGuildTable DiscordGuild { get; set; }

        [InverseProperty(nameof(DiscordChannelModeratorTable.DiscordChannel))]
        public List<DiscordChannelModeratorTable> DiscordChannelModerators { get; set; }
    }
}