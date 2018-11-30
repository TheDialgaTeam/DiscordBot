using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordAppGuild
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordAppGuildId { get; set; }

        [MaxLength(255)]
        public string Prefix { get; set; }

        public bool DeleteCommandAfterUse { get; set; }

        public int DiscordAppId { get; set; }
        public DiscordApp DiscordApp { get; set; }

        public int DiscordGuildId { get; set; }
        public DiscordGuild DiscordGuild { get; set; }

        public List<DiscordAppGuildModule> DiscordAppGuildModules { get; set; }

        public List<DiscordAppChannel> DiscordAppChannels { get; set; }
    }
}