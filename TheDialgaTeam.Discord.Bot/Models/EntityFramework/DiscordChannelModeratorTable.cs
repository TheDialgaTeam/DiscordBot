using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public enum DiscordChannelModeratorType
    {
        Role = 0,

        User = 1
    }

    public sealed class DiscordChannelModeratorTable
    {
        [Key]
        public ulong? Id { get; set; }

        [Required]
        public DiscordChannelModeratorType Type { get; set; }

        [Required]
        public ulong Value { get; set; }

        [Required]
        [ForeignKey(nameof(DiscordChannel))]
        public ulong DiscordChannelId { get; set; }

        public DiscordChannelTable DiscordChannel { get; set; }
    }
}