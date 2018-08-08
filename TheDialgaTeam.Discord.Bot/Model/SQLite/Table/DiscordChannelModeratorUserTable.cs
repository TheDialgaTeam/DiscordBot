using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordChannelModeratorUser")]
    public sealed class DiscordChannelModeratorUserTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string UserId { get; set; }

        [Indexed]
        public long DiscordChannelId { get; set; }
    }
}