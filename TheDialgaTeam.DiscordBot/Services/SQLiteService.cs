using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Extension.System.IO;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;

namespace TheDialgaTeam.DiscordBot.Services
{
    public interface ISQLiteService
    {
        SQLiteAsyncConnection SQLiteAsyncConnection { get; }

        Task InitializeDatabaseAsync();
    }

    internal sealed class SQLiteService : ISQLiteService, IDisposable
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
            try
            {
                var directory = $"{Environment.CurrentDirectory}/Data".ResolveFullPath();

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                SQLiteAsyncConnection = new SQLiteAsyncConnection(SQLiteDataBasePath);

                await SQLiteAsyncConnection.CreateTableAsync<DiscordAppOwnerModel>();
                await SQLiteAsyncConnection.CreateTableAsync<DiscordAppModel>();
                await SQLiteAsyncConnection.CreateTableAsync<DiscordChannelModeratorModel>();
                await SQLiteAsyncConnection.CreateTableAsync<DiscordGuildModel>();
                await SQLiteAsyncConnection.CreateTableAsync<DiscordGuildModeratorModel>();
                await SQLiteAsyncConnection.CreateTableAsync<DiscordGuildModuleModel>();

                await SQLiteAsyncConnection.CreateTableAsync<FreeGameNotificationModel>();
                await SQLiteAsyncConnection.CreateTableAsync<ServerHoundModel>();
                await SQLiteAsyncConnection.CreateTableAsync<PollModel>();

                await SQLiteAsyncConnection.ExecuteAsync("VACUUM");

                await LoggerService.LogMessageAsync($"Database created at: {SQLiteDataBasePath}");
            }
            catch (Exception ex)
            {
                await LoggerService.LogErrorMessageAsync(ex);
            }
        }

        public void Dispose()
        {
            SQLiteAsyncConnection.CloseAsync().GetAwaiter().GetResult();
        }
    }
}