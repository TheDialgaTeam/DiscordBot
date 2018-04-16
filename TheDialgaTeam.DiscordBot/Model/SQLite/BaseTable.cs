using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite
{
    public interface IBaseTable : ITableId
    {
    }

    internal abstract class BaseTable : IBaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
    }
}