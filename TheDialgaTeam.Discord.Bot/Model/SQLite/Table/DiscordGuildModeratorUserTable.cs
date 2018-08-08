using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordGuildModeratorUser")]
    public sealed class DiscordGuildModeratorUserTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string UserId { get; set; }

        [Indexed]
        public long DiscordGuildId { get; set; }
    }
}