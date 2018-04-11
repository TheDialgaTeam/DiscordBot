using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IBaseTable
    {
        int Id { get; }
    }

    internal abstract class BaseTable : IBaseTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
    }
}