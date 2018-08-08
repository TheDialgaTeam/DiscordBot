using System;
using System.IO;

namespace TheDialgaTeam.Discord.Bot.Service.IO
{
    public sealed class FilePathService
    {
        public string ConsoleLogFilePath { get; } = ResolveFullPath($"{Environment.CurrentDirectory}/Logs/{DateTime.UtcNow:yyyy-MM-dd}.txt");

        public string SQLiteDatabaseFilePath { get; } = ResolveFullPath($"{Environment.CurrentDirectory}/Data/Application.db");

        public FilePathService()
        {
            if (!Directory.Exists(ResolveFullPath($"{Environment.CurrentDirectory}/Logs")))
                Directory.CreateDirectory(ResolveFullPath($"{Environment.CurrentDirectory}/Logs"));

            if (!Directory.Exists(ResolveFullPath($"{Environment.CurrentDirectory}/Data")))
                Directory.CreateDirectory(ResolveFullPath($"{Environment.CurrentDirectory}/Data"));
        }

        private static string ResolveFullPath(string filePath)
        {
            return Path.GetFullPath(ResolveFilePath(filePath));
        }

        private static string ResolveFilePath(string filePath)
        {
            return filePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}