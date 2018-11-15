using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordChannel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong DiscordChannelId { get; set; }

        public ulong ChannelId { get; set; }

        public ulong DiscordGuildId { get; set; }
        public DiscordGuild DiscordGuild { get; set; }

        public List<DiscordChannelModerator> DiscordChannelModerators { get; set; }
    }
}