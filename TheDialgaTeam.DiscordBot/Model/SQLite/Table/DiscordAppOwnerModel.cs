using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IDiscordAppOwnerModel
    {
        int Id { get; }

        string UserId { get; }
    }

    [Table("DiscordAppOwners")]
    internal sealed class DiscordAppOwnerModel : IDiscordAppOwnerModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string ClientId { get; set; }

        [Unique]
        public string UserId { get; set; }
    }
}