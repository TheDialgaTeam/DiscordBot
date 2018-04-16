using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordAppModel : IBaseTable, IClientId
    {
        string ClientSecret { get; set; }

        string AppName { get; set; }

        string BotToken { get; set; }

        bool Verified { get; set; }
    }

    [Table("DiscordAppModels")]
    internal sealed class DiscordAppModel : BaseTable, IDiscordAppModel
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AppName { get; set; }

        public string BotToken { get; set; }

        public bool Verified { get; set; }
    }
}