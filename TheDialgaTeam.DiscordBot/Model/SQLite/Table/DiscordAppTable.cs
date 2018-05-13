using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordApp")]
    internal sealed class DiscordAppTable : BaseTable, IDatabaseTable
    {
        public string AppName { get; set; }

        public string AppDescription { get; set; }

        public DateTimeOffset LastUpdateCheck { get; set; }

        [ForeignKey(typeof(DiscordAppDetailTable))]
        public long DiscordAppDetailId { get; set; }

        [ForeignKey(typeof(DiscordBotDetailTable))]
        public long DiscordBotDetailId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordAppDetailTable DiscordAppDetail { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordBotDetailTable DiscordBotDetail { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<DiscordAppOwnerTable> DiscordAppOwner { get; set; }
    }
}