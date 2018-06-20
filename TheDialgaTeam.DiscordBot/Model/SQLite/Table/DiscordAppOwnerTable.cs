using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordAppOwner")]
    internal sealed class DiscordAppOwnerTable : BaseTable, IDatabaseTable
    {
        public string UserId { get; set; }

        [Indexed]
        public string DiscordAppId { get; set; }
    }
}