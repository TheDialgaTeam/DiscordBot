using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules
{
    public interface IServerHoundModel : IBaseTable, IClientId, IGuildId
    {
        bool DBans { get; }
    }

    [Table("ServerHoundModuleModels")]
    internal sealed class ServerHoundModel : BaseTable, IServerHoundModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public bool DBans { get; set; }
    }
}