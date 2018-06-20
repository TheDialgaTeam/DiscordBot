using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordChannelModeratorRole")]
    internal sealed class DiscordChannelModeratorRoleTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        public string RoleId { get; set; }

        [Indexed]
        public long DiscordChannelId { get; set; }
    }
}