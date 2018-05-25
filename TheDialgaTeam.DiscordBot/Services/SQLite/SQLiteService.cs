using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SQLite;
using TheDialgaTeam.DiscordBot.Extension.System.IO;
using TheDialgaTeam.DiscordBot.Model.SQLite;
using TheDialgaTeam.DiscordBot.Services.Logger;

namespace TheDialgaTeam.DiscordBot.Services.SQLite
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
            try
            {
                var directory = $"{Environment.CurrentDirectory}/Data".ResolveFullPath();

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                SQLiteAsyncConnection = new SQLiteAsyncConnection(SQLiteDataBasePath);

                var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

                foreach (var assemblyType in assemblyTypes)
                {
                    if (!assemblyType.IsClass || !typeof(IDatabaseTable).IsAssignableFrom(assemblyType))
                        continue;

                    await SQLiteAsyncConnection.CreateTableAsync(assemblyType);
                }

                await SQLiteAsyncConnection.ExecuteAsync("VACUUM;");
                await LoggerService.LogMessageAsync($"Database created at: {SQLiteDataBasePath}");
            }
            catch (Exception ex)
            {
                await LoggerService.LogErrorMessageAsync(ex);
            }
        }
    }
}