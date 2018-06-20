using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordChannelModeratorRole")]
    internal sealed class DiscordChannelModeratorRoleTable : BaseTable, IDatabaseTable
    {
        public string RoleId { get; set; }

        [Indexed]
        public long DiscordChannelId { get; set; }
    }
}