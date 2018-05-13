using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordGuildModeratorRole")]
    internal sealed class DiscordGuildModeratorRoleTable : BaseTable, IDatabaseTable
    {
        public string RoleId { get; set; }

        [ForeignKey(typeof(DiscordGuildTable))]
        public long DiscordGuildId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordGuildTable DiscordGuild { get; set; }
    }
}