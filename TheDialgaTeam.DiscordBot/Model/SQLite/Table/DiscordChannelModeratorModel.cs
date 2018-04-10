using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordChannelModeratorModel
    {
        int Id { get; }

        string ClientId { get; }

        string ChannelId { get; }

        string MentionId { get; }

        string UserId { get; }
    }

    [Table("DiscordChannelModerators")]
    internal sealed class DiscordChannelModeratorModel : IDiscordChannelModeratorModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string ClientId { get; set; }

        public string ChannelId { get; set; }

        public string MentionId { get; set; }

        public string UserId { get; set; }
    }
}