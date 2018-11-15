using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordAppModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong DiscordAppModuleId { get; set; }

        public bool Active { get; set; }

        public ulong DiscordAppId { get; set; }
        public DiscordApp DiscordApp { get; set; }

        public ulong DiscordModuleId { get; set; }
        public DiscordModule DiscordModule { get; set; }
    }
}