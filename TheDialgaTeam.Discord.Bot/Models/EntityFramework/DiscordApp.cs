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
        public ulong DiscordAppId { get; set; }

        public ulong ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        public string AppName { get; set; }

        public string AppDescription { get; set; }

        [Required]
        public string BotToken { get; set; }

        public DateTimeOffset? LastUpdateCheck { get; set; }

        public List<DiscordAppOwner> DiscordAppOwners { get; set; }

        public List<DiscordAppModule> DiscordAppModules { get; set; }

        public List<DiscordGuild> DiscordGuilds { get; set; }
    }
}