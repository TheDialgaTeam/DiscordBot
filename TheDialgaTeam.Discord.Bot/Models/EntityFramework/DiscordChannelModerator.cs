using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public enum DiscordChannelModeratorType
    {
        Role = 0,
        User = 1
    }

    public sealed class DiscordChannelModerator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordChannelModeratorId { get; set; }

        public DiscordChannelModeratorType Type { get; set; }

        public ulong Value { get; set; }

        public int DiscordChannelId { get; set; }
        public DiscordChannel DiscordChannel { get; set; }
    }
}