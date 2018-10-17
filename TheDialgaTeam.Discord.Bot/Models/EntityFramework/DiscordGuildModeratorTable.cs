using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public enum DiscordGuildModeratorType
    {
        Role = 0,

        User = 1
    }

    public sealed class DiscordGuildModeratorTable
    {
        [Key]
        public ulong? Id { get; set; }

        [Required]
        public DiscordGuildModeratorType Type { get; set; }

        [Required]
        public ulong Value { get; set; }

        [Required]
        [ForeignKey(nameof(DiscordGuild))]
        public ulong DiscordGuildId { get; set; }

        public DiscordGuildTable DiscordGuild { get; set; }
    }
}