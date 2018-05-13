using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordChannelModeratorUser")]
    internal sealed class DiscordChannelModeratorUserTable : BaseTable, IDatabaseTable
    {
        public string UserId { get; set; }

        [ForeignKey(typeof(DiscordChannelTable))]
        public long DiscordChannelId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordChannelTable DiscordChannel { get; set; }
    }
}