using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules
{
    public interface IServerHoundModel
    {
        string ClientId { get; }

        string GuildId { get; }

        bool DBans { get; }
    }

    [Table("ServerHound")]
    internal sealed class ServerHoundModel : BaseTable, IServerHoundModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public bool DBans { get; set; }
    }
}