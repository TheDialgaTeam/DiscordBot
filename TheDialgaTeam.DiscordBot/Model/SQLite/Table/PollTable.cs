using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("Poll")]
    internal sealed class PollTable : BaseTable, IDatabaseTable
    {
        public string MessageId { get; set; }

        public DateTimeOffset StartDateTime { get; set; }

        public TimeSpan Duration { get; set; }

        [ForeignKey(typeof(DiscordChannelTable))]
        public long DiscordChannelId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordChannelTable DiscordChannel { get; set; }
    }
}