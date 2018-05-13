using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordBotDetail")]
    internal sealed class DiscordBotDetailTable : BaseTable, IDatabaseTable
    {
        public string BotToken { get; set; }

        public bool Verified { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordAppTable DiscordApp { get; set; }
    }
}