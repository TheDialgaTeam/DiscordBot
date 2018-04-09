using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.Modules.System.IO;

namespace TheDialgaTeam.DiscordBot.Services
{
    public interface ISQLiteService
    {
        SQLiteAsyncConnection SQLiteAsyncConnection { get; }

        Task InitializeDatabaseAsync();
    }

    internal sealed class SQLiteService : ISQLiteService
    {
        public SQLiteAsyncConnection SQLiteAsyncConnection { get; private set; }

        private ILoggerService LoggerService { get; }

        private string SQLiteDataBasePath { get; } = $"{Environment.CurrentDirectory}/Data/Application.db".ResolveFullPath();

        public SQLiteService(ILoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        public async Task InitializeDatabaseAsync()
        {
            var directory = $"{Environment.CurrentDirectory}/Data".ResolveFullPath();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            SQLiteAsyncConnection = new SQLiteAsyncConnection(SQLiteDataBasePath);
            await SQLiteAsyncConnection.CreateTableAsync<BotInstanceModel>();

            await LoggerService.LogMessageAsync($"Database created at: {SQLiteDataBasePath}");
        }
    }
}