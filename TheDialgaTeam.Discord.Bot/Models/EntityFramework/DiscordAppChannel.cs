using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordAppChannel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordAppChannelId { get; set; }

        public int DiscordAppGuildId { get; set; }
        public DiscordAppGuild DiscordAppGuild { get; set; }

        public int DiscordChannelId { get; set; }
        public DiscordChannel DiscordChannel { get; set; }
    }
}