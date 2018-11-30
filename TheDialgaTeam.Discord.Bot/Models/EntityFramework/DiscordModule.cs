using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordModuleId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Module { get; set; }

        public List<DiscordModuleRequirement> DiscordModuleRequirements { get; set; }

        public List<DiscordAppGuildModule> DiscordAppGuildModules { get; set; }
    }
}