using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Services.EntityFramework
{
    public sealed class SqliteDatabaseService
    {
        private FilePathService FilePathService { get; }

        public SqliteDatabaseService(FilePathService filePathService)
        {
            FilePathService = filePathService;
        }

        public SqliteContext GetContext(bool readOnly = false)
        {
            return new SqliteContext(FilePathService, readOnly);
        }
    }
}