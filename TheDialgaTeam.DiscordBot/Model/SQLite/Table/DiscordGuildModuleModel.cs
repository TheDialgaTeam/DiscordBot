using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordGuildModuleModel : IBaseTable, IClientId, IGuildId
    {
        string Module { get; set; }

        bool Active { get; set; }
    }

    [Table("DiscordGuildModuleModels")]
    internal sealed class DiscordGuildModuleModel : BaseTable, IDiscordGuildModuleModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string Module { get; set; }

        public bool Active { get; set; }
    }
}