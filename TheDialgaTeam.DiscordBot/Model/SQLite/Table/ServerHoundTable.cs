using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("ServerHound")]
    internal sealed class ServerHoundTable : BaseTable, IDatabaseTable
    {
        public bool Dbans { get; set; }

        public DateTimeOffset LastChecked { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordGuildTable DiscordGuild { get; set; }
    }
}