using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordGuildModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong DiscordGuildModuleId { get; set; }

        public bool Active { get; set; }

        public ulong DiscordGuildId { get; set; }
        public DiscordGuild DiscordGuild { get; set; }

        public ulong DiscordModuleId { get; set; }
        public DiscordModule DiscordModule { get; set; }
    }
}