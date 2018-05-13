using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordChannelModeratorRole")]
    internal sealed class DiscordChannelModeratorRoleTable : BaseTable, IDatabaseTable
    {
        public string RoleId { get; set; }

        [ForeignKey(typeof(DiscordChannelTable))]
        public long DiscordChannelId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordChannelTable DiscordChannel { get; set; }
    }
}