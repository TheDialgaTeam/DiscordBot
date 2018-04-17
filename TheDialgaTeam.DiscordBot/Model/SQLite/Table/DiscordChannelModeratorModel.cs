using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordChannelModeratorModel : IBaseTable, IClientId, IGuildId, IChannelId, IRoleId, IUserId
    {
    }

    [Table("DiscordChannelModeratorModels")]
    internal sealed class DiscordChannelModeratorModel : BaseTable, IDiscordChannelModeratorModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string ChannelId { get; set; }

        public string RoleId { get; set; }

        public string UserId { get; set; }
    }
}