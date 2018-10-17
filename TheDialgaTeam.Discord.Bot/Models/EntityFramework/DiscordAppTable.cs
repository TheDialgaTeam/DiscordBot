using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordAppTable
    {
        [Key]
        public ulong? Id { get; set; }

        [Required]
        public ulong ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        public string AppName { get; set; }

        public string AppDescription { get; set; }

        [Required]
        public string BotToken { get; set; }

        public DateTimeOffset? LastUpdateCheck { get; set; }

        [InverseProperty(nameof(DiscordAppOwnerTable.DiscordApp))]
        public List<DiscordAppOwnerTable> DiscordAppOwners { get; set; }

        [InverseProperty(nameof(DiscordGuildTable.DiscordApp))]
        public List<DiscordGuildTable> DiscordGuilds { get; set; }
    }
}