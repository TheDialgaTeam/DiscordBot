using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordChannelModeratorUser")]
    internal sealed class DiscordChannelModeratorUserTable : BaseTable, IDatabaseTable
    {
        public string UserId { get; set; }

        [Indexed]
        public long DiscordChannelId { get; set; }
    }
}