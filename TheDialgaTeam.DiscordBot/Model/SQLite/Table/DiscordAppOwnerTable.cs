using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordAppOwner")]
    internal sealed class DiscordAppOwnerTable : BaseTable, IDatabaseTable
    {
        public string UserId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordAppTable DiscordApp { get; set; }
    }
}