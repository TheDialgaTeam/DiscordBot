using SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table
{
    [Table("DiscordAppOwnerTable")]
    public sealed class DiscordAppOwnerTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string UserId { get; set; }

        public long? DiscordAppId { get; set; }
    }
}