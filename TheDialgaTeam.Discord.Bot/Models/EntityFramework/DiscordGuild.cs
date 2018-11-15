using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordGuild
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong DiscordGuildId { get; set; }

        public ulong GuildId { get; set; }

        public string Prefix { get; set; }

        public bool DeleteCommandAfterUse { get; set; }

        public ulong DiscordAppId { get; set; }
        public DiscordApp DiscordApp { get; set; }

        public List<DiscordGuildModerator> DiscordGuildModerators { get; set; }

        public List<DiscordGuildModule> DiscordGuildModules { get; set; }

        public List<DiscordChannel> DiscordChannels { get; set; }
    }
}