using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules
{
    public interface IFreeGameNotificationModel : IBaseTable, IClientId, IGuildId, IChannelId, IRoleId
    {
    }

    [Table("FreeGameNotificationModuleModels")]
    internal sealed class FreeGameNotificationModel : BaseTable, IFreeGameNotificationModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string ChannelId { get; set; }

        public string RoleId { get; set; }
    }
}