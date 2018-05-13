using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("FreeGameNotification")]
    internal sealed class FreeGameNotificationTable : BaseTable, IDatabaseTable
    {
        public bool Active { get; set; }

        public string RoleId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordChannelTable DiscordChannel { get; set; }
    }
}