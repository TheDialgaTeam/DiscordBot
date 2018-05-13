using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordGuildModeratorUser")]
    internal sealed class DiscordGuildModeratorUserTable : BaseTable, IDatabaseTable
    {
        public string UserId { get; set; }

        [ForeignKey(typeof(DiscordGuildTable))]
        public long DiscordGuildId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordGuildTable DiscordGuild { get; set; }
    }
}