using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordGuildModuleTable
    {
        [Key]
        public ulong? Id { get; set; }

        [Required]
        public string Module { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        [ForeignKey(nameof(DiscordGuild))]
        public ulong DiscordGuildId { get; set; }

        public DiscordGuildTable DiscordGuild { get; set; }
    }
}