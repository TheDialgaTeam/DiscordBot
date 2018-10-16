using System;
using SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table
{
    [Table("DiscordApp")]
    public sealed class DiscordAppTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        [Unique]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AppName { get; set; }

        public string AppDescription { get; set; }

        public string BotToken { get; set; }

        public DateTimeOffset? LastUpdateCheck { get; set; }
    }
}