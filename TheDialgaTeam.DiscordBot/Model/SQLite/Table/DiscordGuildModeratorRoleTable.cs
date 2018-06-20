using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordGuildModeratorRole")]
    internal sealed class DiscordGuildModeratorRoleTable : BaseTable, IDatabaseTable
    {
        public string RoleId { get; set; }

        [Indexed]
        public long DiscordGuildId { get; set; }
    }
}