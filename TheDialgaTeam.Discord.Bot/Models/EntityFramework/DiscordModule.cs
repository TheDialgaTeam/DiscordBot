using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong DiscordModuleId { get; set; }

        [Required]
        public string Module { get; set; }

        public List<DiscordAppModule> DiscordAppModules { get; set; }

        public List<DiscordGuildModule> DiscordGuildModules { get; set; }

        public List<DiscordModuleRequirement> DiscordModuleRequirements { get; set; }
    }
}