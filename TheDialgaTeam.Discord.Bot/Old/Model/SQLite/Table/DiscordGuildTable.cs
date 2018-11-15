using SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table
{
    [Table("DiscordGuildTable")]
    public sealed class DiscordGuildTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string GuildId { get; set; }

        public string Prefix { get; set; }

        public bool? DeleteCommandAfterUse { get; set; }

        public long? DiscordAppId { get; set; }
    }
}