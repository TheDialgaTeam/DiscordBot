using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordGuildModeratorUser")]
    internal sealed class DiscordGuildModeratorUserTable : BaseTable, IDatabaseTable
    {
        public string UserId { get; set; }

        [Indexed]
        public long DiscordGuildId { get; set; }
    }
}