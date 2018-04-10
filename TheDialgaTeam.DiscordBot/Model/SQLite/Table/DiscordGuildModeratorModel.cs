using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordGuildModeratorModel
    {
        int Id { get; }

        string ClientId { get; }

        string GuildId { get; }

        string MentionId { get; }

        string UserId { get; }
    }

    [Table("DiscordGuildModerators")]
    internal sealed class DiscordGuildModeratorModel : IDiscordGuildModeratorModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string MentionId { get; set; }

        public string UserId { get; set; }
    }
}