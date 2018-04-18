using SQLite;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Interface;

namespace TheDialgaTeam.DiscordBot.Services.SQLite
{
    internal abstract class SQLiteDatabaseTableService<T> where T : IDatabaseTable, new()
    {
        protected AsyncTableQuery<T> DataTable => SQLiteService.SQLiteAsyncConnection.Table<T>();

        private ISQLiteService SQLiteService { get; }

        protected SQLiteDatabaseTableService(ISQLiteService sqliteService)
        {
            SQLiteService = sqliteService;
        }

        protected async Task<int> InsertAsync(T model)
        {
            return await SQLiteService.SQLiteAsyncConnection.InsertAsync(model);
        }

        protected async Task<int> UpdateAsync(T model)
        {
            return await SQLiteService.SQLiteAsyncConnection.UpdateAsync(model);
        }

        protected async Task<int> DeleteAsync(T model)
        {
            return await SQLiteService.SQLiteAsyncConnection.DeleteAsync(model);
        }
    }
}