using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public enum DiscordGuildModeratorType
    {
        Role = 0,
        User = 1
    }

    public sealed class DiscordGuildModerator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordGuildModeratorId { get; set; }

        public DiscordGuildModeratorType Type { get; set; }

        public ulong Value { get; set; }

        public int DiscordGuildId { get; set; }
        public DiscordGuild DiscordGuild { get; set; }
    }
}