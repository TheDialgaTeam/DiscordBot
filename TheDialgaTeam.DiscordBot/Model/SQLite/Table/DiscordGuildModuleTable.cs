﻿using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordGuildModule")]
    internal sealed class DiscordGuildModuleTable : BaseTable, IDatabaseTable
    {
        public string Module { get; set; }

        public bool Active { get; set; }

        [Indexed]
        public long DiscordGuildId { get; set; }
    }
}