using SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table
{
    [Table("DiscordGuildModeratorRole")]
    public sealed class DiscordGuildModeratorRoleTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string RoleId { get; set; }

        public long? DiscordGuildId { get; set; }
    }
}