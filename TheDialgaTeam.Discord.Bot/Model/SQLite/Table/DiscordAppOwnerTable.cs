using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordAppOwner")]
    public sealed class DiscordAppOwnerTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string UserId { get; set; }

        public long? DiscordAppId { get; set; }
    }
}