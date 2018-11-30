using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordModuleRequirement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordModuleRequirementId { get; set; }

        public int DiscordModuleId { get; set; }
        public DiscordModule DiscordModule { get; set; }

        public int DiscordGuildId { get; set; }
        public DiscordGuild DiscordGuild { get; set; }
    }
}