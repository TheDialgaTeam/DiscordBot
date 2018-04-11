using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordAppOwnerModel : IBaseTable
    {
        string ClientId { get; }

        string UserId { get; }
    }

    [Table("DiscordAppOwners")]
    internal sealed class DiscordAppOwnerModel : BaseTable, IDiscordAppOwnerModel
    {
        public string ClientId { get; set; }

        public string UserId { get; set; }
    }
}