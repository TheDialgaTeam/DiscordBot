using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordGuildModel
    {
        int Id { get; }

        string ClientId { get; }

        string GuildId { get; }

        string CharPrefix { get; }
    }

    [Table("DiscordGuilds")]
    internal sealed class DiscordGuildModel : IDiscordGuildModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string CharPrefix { get; set; }
    }
}