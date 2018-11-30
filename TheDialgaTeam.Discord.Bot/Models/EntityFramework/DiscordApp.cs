using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordApp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordAppId { get; set; }

        public ulong ClientId { get; set; }

        [Required]
        [MaxLength(32)]
        public string ClientSecret { get; set; }

        [MaxLength(32)]
        public string AppName { get; set; }

        [MaxLength(400)]
        public string AppDescription { get; set; }

        [Required]
        [MaxLength(59)]
        public string BotToken { get; set; }

        public DateTimeOffset? LastUpdateCheck { get; set; }

        public List<DiscordAppOwner> DiscordAppOwners { get; set; }

        public List<DiscordAppGuild> DiscordAppGuilds { get; set; }
    }
}