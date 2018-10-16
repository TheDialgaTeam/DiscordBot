using SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table
{
    [Table("DiscordChannel")]
    public sealed class DiscordChannelTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string ChannelId { get; set; }

        public long? DiscordGuildId { get; set; }
    }
}