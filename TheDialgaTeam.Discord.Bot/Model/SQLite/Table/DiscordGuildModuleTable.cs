using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordGuildModule")]
    public sealed class DiscordGuildModuleTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string Module { get; set; }

        public bool Active { get; set; }

        [Indexed]
        public long DiscordGuildId { get; set; }
    }
}