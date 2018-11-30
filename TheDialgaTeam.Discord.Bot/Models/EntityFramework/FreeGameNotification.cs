using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class FreeGameNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FreeGameNotificationId { get; set; }

        public ulong RoleId { get; set; }

        public int DiscordChannelId { get; set; }
        public DiscordChannel DiscordChannel { get; set; }
    }
}