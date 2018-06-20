using System;
using System.Reflection;
using System.Threading.Tasks;
using SQLite;
using TheDialgaTeam.Discord.Bot.Model.SQLite;
using TheDialgaTeam.Discord.Bot.Service.IO;
using TheDialgaTeam.Discord.Bot.Service.Logger;

namespace TheDialgaTeam.Discord.Bot.Service.SQLite
{
    public interface ISQLiteService
    {
        SQLiteAsyncConnection SQLiteAsyncConnection { get; }

        Task InitializeDatabaseAsync();
    }

    internal sealed class SQLiteService : ISQLiteService
    {
        public SQLiteAsyncConnection SQLiteAsyncConnection { get; }

        private IFilePathService FilePathService { get; }

        private ILoggerService LoggerService { get; }

        public SQLiteService(IFilePathService filePathService, ILoggerService loggerService)
        {
            FilePathService = filePathService;
            LoggerService = loggerService;
            SQLiteAsyncConnection = new SQLiteAsyncConnection(filePathService.SQLiteDatabaseFilePath);
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

                foreach (var assemblyType in assemblyTypes)
                {
                    if (!assemblyType.IsClass || !typeof(IDatabaseTable).IsAssignableFrom(assemblyType))
                        continue;

                    await SQLiteAsyncConnection.CreateTableAsync(assemblyType).ConfigureAwait(false);
                }

                await SQLiteAsyncConnection.ExecuteAsync("VACUUM;").ConfigureAwait(false);
                await LoggerService.LogMessageAsync($"Database created at: {FilePathService.SQLiteDatabaseFilePath}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await LoggerService.LogErrorMessageAsync(ex).ConfigureAwait(false);
            }
        }
    }
}