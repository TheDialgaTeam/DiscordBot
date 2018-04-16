using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordGuildModel : IBaseTable, IClientId, IGuildId
    {
        string StringPrefix { get; set; }
    }

    [Table("DiscordGuildModels")]
    internal sealed class DiscordGuildModel : BaseTable, IDiscordGuildModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string StringPrefix { get; set; }
    }
}