using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordAppOwner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordAppOwnerId { get; set; }

        public ulong UserId { get; set; }

        public int DiscordAppId { get; set; }
        public DiscordApp DiscordApp { get; set; }
    }
}