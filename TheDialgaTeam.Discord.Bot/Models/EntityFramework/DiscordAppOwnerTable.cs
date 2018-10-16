using System.ComponentModel.DataAnnotations;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordAppOwnerTable
    {
        [Key]
        public ulong? Id { get; set; }

        public ulong? UserId { get; set; }

        public ulong? DiscordAppId { get; set; }

        public DiscordAppTable DiscordApp { get; set; }
    }
}