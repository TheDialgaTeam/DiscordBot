using System;
using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("VanityLinkIPJoinLog")]
    public sealed class VanityLinkIPJoinLogTable : IDatabaseTable
    {
        public long? Id { get; set; }

        public string IP { get; set; }

        public DateTimeOffset? LastJoined { get; set; }

        public long? VanityLinkModuleId { get; set; }
    }
}