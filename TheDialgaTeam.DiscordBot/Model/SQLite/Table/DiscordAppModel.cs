using SQLite;
using TheDialgaTeam.DiscordBot.Extension.System.Security.Cryptography;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordAppModel
    {
        int Id { get; }

        string ClientId { get; set; }

        string GetBotToken();

        void SetBotToken(string botToken);
    }

    [Table("DiscordApps")]
    internal sealed class DiscordAppModel : IDiscordAppModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string ClientId { get; set; }

        public string BotToken { get; set; }

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