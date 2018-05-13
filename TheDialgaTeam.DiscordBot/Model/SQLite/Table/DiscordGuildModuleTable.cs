using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordGuildModule")]
    internal sealed class DiscordGuildModuleTable : BaseTable, IDatabaseTable
    {
        public string Module { get; set; }

        public bool Active { get; set; }

        [ForeignKey(typeof(DiscordGuildTable))]
        public long DiscordGuildId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordGuildTable DiscordGuild { get; set; }
    }
}