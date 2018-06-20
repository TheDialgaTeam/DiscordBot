using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordChannel")]
    internal sealed class DiscordChannelTable : BaseTable, IDatabaseTable
    {
        public string ChannelId { get; set; }

        [Indexed]
        public long DiscordGuildId { get; set; }
    }
}