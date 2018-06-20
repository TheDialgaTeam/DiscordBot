using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("FreeGameNotification")]
    internal sealed class FreeGameNotificationTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        public bool Active { get; set; }

        public string RoleId { get; set; }

        [Indexed]
        public long DiscordChannelId { get; set; }
    }
}