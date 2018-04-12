using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules
{
    public interface IFreeGameNotificationModel : IBaseTable
    {
        string ClientId { get; }

        string GuildId { get; }

        string RoleId { get; }

        string ChannelId { get; }
    }

    [Table("FreeGameNotification")]
    internal sealed class FreeGameNotificationModel : BaseTable, IFreeGameNotificationModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string RoleId { get; set; }

        public string ChannelId { get; set; }
    }
}