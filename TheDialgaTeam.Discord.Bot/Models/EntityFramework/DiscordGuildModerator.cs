using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordGuildModerator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong DiscordGuildModeratorId { get; set; }

        public DiscordGuildModeratorType Type { get; set; }

        public ulong Value { get; set; }

        public ulong DiscordGuildId { get; set; }
        public DiscordGuild DiscordGuild { get; set; }
    }

    public enum DiscordGuildModeratorType
    {
        Role = 0,
        User = 1
    }
}