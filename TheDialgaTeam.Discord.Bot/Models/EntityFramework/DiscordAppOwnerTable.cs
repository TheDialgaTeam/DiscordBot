using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordAppOwnerTable
    {
        [Key]
        public ulong? Id { get; set; }

        [Required]
        public ulong UserId { get; set; }

        [ForeignKey(nameof(DiscordApp))]
        public ulong? DiscordAppId { get; set; }

        public DiscordAppTable DiscordApp { get; set; }
    }
}