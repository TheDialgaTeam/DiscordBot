using SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table
{
    [Table("DiscordGuildModuleTable")]
    public sealed class DiscordGuildModuleTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string Module { get; set; }

        public bool? Active { get; set; }

        public long? DiscordGuildId { get; set; }
    }
}