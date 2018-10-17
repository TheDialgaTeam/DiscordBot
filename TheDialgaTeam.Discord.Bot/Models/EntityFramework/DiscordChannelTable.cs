using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordChannelTable
    {
        [Key]
        public ulong? Id { get; set; }

        [Required]
        public ulong ChannelId { get; set; }

        [Required]
        [ForeignKey(nameof(DiscordGuild))]
        public ulong DiscordGuildId { get; set; }

        public DiscordGuildTable DiscordGuild { get; set; }

        [InverseProperty(nameof(DiscordChannelModeratorTable.DiscordChannel))]
        public List<DiscordChannelModeratorTable> DiscordChannelModerators { get; set; }
    }
}