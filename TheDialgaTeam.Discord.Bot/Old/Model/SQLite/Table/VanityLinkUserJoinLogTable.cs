using System;
using SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table
{
    [Table("VanityLinkUserJoinLog")]
    public sealed class VanityLinkUserJoinLogTable : IDatabaseTable
    {
        public long? Id { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset? LastJoined { get; set; }

        public long? VanityLinkModuleId { get; set; }
    }
}