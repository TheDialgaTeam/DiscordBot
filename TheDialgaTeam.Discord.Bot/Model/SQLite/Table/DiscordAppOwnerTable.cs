using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordAppOwner")]
    internal sealed class DiscordAppOwnerTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        public string UserId { get; set; }

        [Indexed]
        public string DiscordAppId { get; set; }
    }
}