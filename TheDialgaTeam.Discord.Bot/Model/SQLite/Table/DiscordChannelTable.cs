using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordChannel")]
    internal sealed class DiscordChannelTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        public string ChannelId { get; set; }

        [Indexed]
        public long DiscordGuildId { get; set; }
    }
}