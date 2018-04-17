using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite
{
    public interface IBaseTable : ITableId
    {
    }

    internal abstract class BaseTable : IBaseTable, IDatabaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
    }
}