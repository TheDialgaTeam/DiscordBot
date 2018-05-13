using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite
{
    internal abstract class BaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }
    }
}