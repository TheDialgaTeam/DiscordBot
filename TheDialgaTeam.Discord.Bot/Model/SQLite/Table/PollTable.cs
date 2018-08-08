using System;
using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("Poll")]
    public sealed class PollTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string MessageId { get; set; }

        public DateTimeOffset StartDateTime { get; set; }

        public TimeSpan Duration { get; set; }

        [Indexed]
        public long DiscordChannelId { get; set; }
    }
}