using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordAppDetail")]
    internal sealed class DiscordAppDetailTable : BaseTable, IDatabaseTable
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordAppTable DiscordApp { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordGuildTable DiscordGuild { get; set; }
    }
}