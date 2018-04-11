using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordGuildModeratorModel : IBaseTable
    {
        string ClientId { get; }

        string GuildId { get; }

        string RoleId { get; }

        string UserId { get; }
    }

    [Table("DiscordGuildModerators")]
    internal sealed class DiscordGuildModeratorModel : BaseTable, IDiscordGuildModeratorModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string RoleId { get; set; }

        public string UserId { get; set; }
    }
}