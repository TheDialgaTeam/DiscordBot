using System;
using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordApp")]
    internal sealed class DiscordAppTable : IDatabaseTable
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AppName { get; set; }

        public string AppDescription { get; set; }

        public string BotToken { get; set; }

        public DateTimeOffset LastUpdateCheck { get; set; }
    }
}