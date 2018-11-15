using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordModuleRequirement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong DiscordModuleRequirementId { get; set; }

        public ulong GuildId { get; set; }

        public ulong DiscordModuleId { get; set; }
        public DiscordModule DiscordModule { get; set; }
    }
}