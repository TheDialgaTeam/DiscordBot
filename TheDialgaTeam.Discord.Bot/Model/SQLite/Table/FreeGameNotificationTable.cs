using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("FreeGameNotification")]
    public sealed class FreeGameNotificationTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public bool? Active { get; set; }

        public string RoleId { get; set; }

        public long? DiscordChannelId { get; set; }
    }
}