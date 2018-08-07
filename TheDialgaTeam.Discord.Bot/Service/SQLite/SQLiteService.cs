using System;
using System.Reflection;
using System.Threading.Tasks;
using SQLite;
using TheDialgaTeam.Discord.Bot.Model.SQLite;
using TheDialgaTeam.Discord.Bot.Service.IO;
using TheDialgaTeam.Discord.Bot.Service.Logger;

namespace TheDialgaTeam.Discord.Bot.Service.SQLite
{
    internal sealed class SQLiteService
    {
        public SQLiteAsyncConnection SQLiteAsyncConnection { get; }

        private FilePathService FilePathService { get; }

        private LoggerService LoggerService { get; }

        public SQLiteService(FilePathService filePathService, LoggerService loggerService)
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