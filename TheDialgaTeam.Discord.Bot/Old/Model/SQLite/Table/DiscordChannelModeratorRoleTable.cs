using SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table
{
    [Table("DiscordChannelModeratorRole")]
    public sealed class DiscordChannelModeratorRoleTable : IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long? Id { get; set; }

        public string RoleId { get; set; }

        public long? DiscordChannelId { get; set; }
    }
}