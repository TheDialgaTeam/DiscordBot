﻿using System;
using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("ServerHound")]
    internal sealed class ServerHoundTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        public bool Dbans { get; set; }

        public DateTimeOffset LastChecked { get; set; }

        [Indexed]
        public long DiscordGuildId { get; set; }
    }
}