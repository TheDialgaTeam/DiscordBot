using SQLite;
using TheDialgaTeam.DiscordBot.Extension.System.Security.Cryptography;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordAppModel : IBaseTable
    {
        string ClientId { get; }

        bool Verified { get; set; }

        string GetBotToken();
    }

    [Table("DiscordApps")]
    internal sealed class DiscordAppModel : BaseTable, IDiscordAppModel
    {
        [Unique]
        public string ClientId { get; set; }

        public string BotToken { get; set; }

        public bool Verified { get; set; }

        public string GetBotToken()
        {
            return BotToken.DecryptString("TheDialgaTeam");
        }

        public void SetBotToken(string botToken)
        {
            BotToken = botToken.EncryptString("TheDialgaTeam");
        }
    }
}