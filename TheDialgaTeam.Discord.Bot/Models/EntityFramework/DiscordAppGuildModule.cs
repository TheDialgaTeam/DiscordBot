using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordAppGuildModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordAppGuildModuleId { get; set; }

        public bool Active { get; set; }

        public int DiscordAppGuildId { get; set; }
        public DiscordAppGuild DiscordAppGuild { get; set; }

        public int DiscordModuleId { get; set; }
        public DiscordModule DiscordModule { get; set; }
    }
}