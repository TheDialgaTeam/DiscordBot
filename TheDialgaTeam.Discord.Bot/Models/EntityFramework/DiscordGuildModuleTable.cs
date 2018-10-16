using System.ComponentModel.DataAnnotations;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordGuildModuleTable
    {
        [Key]
        public ulong? Id { get; set; }

        public string Module { get; set; }

        public bool? Active { get; set; }

        public ulong? DiscordGuildId { get; set; }

        public DiscordGuildTable DiscordGuild { get; set; }
    }
}