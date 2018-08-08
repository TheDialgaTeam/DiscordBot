using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordGuild")]
    public sealed class DiscordGuildTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string GuildId { get; set; }

        public string Prefix { get; set; }

        [Indexed]
        public long DiscordAppId { get; set; }
    }
}