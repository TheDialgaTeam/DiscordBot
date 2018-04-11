using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordGuildModel : IBaseTable
    {
        string ClientId { get; }

        string GuildId { get; }

        string CharPrefix { get; }
    }

    [Table("DiscordGuilds")]
    internal sealed class DiscordGuildModel : BaseTable, IDiscordGuildModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string CharPrefix { get; set; }
    }
}