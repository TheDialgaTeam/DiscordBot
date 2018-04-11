using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordGuildModuleModel : IBaseTable
    {
        string ClientId { get; }

        string GuildId { get; }

        string Module { get; }

        bool Active { get; }
    }

    [Table("DiscordGuildModules")]
    internal sealed class DiscordGuildModuleModel : BaseTable, IDiscordGuildModuleModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string Module { get; set; }

        public bool Active { get; set; }
    }
}