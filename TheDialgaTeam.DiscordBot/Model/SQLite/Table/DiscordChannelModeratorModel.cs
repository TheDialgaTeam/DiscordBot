using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordChannelModeratorModel : IBaseTable
    {
        string ClientId { get; }

        string ChannelId { get; }

        string RoleId { get; }

        string UserId { get; }
    }

    [Table("DiscordChannelModerators")]
    internal sealed class DiscordChannelModeratorModel : BaseTable, IDiscordChannelModeratorModel
    {
        public string ClientId { get; set; }

        public string ChannelId { get; set; }

        public string RoleId { get; set; }

        public string UserId { get; set; }
    }
}