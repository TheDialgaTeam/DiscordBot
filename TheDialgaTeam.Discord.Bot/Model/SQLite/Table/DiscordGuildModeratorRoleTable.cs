using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordGuildModeratorRole")]
    internal sealed class DiscordGuildModeratorRoleTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        public string RoleId { get; set; }

        [Indexed]
        public long DiscordGuildId { get; set; }
    }
}