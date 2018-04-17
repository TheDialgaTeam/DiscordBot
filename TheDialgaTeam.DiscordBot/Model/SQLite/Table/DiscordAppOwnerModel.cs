using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordAppOwnerModel : IBaseTable, IClientId, IUserId
    {
    }

    [Table("DiscordAppOwnerModels")]
    internal sealed class DiscordAppOwnerModel : BaseTable, IDiscordAppOwnerModel
    {
        public string ClientId { get; set; }

        public string UserId { get; set; }
    }
}