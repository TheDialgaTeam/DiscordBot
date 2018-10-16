using System.ComponentModel.DataAnnotations;

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

        public DiscordGuildModeratorType Type { get; set; }

        public ulong? Value { get; set; }

        public ulong? DiscordGuildId { get; set; }

        public DiscordGuildTable DiscordGuild { get; set; }
    }
}