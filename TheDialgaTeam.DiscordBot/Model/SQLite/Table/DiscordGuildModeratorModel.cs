using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordGuildModeratorModel : IBaseTable, IClientId, IGuildId, IRoleId, IUserId
    {
    }

    [Table("DiscordGuildModeratorModels")]
    internal sealed class DiscordGuildModeratorModel : BaseTable, IDiscordGuildModeratorModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string RoleId { get; set; }

        public string UserId { get; set; }
    }
}