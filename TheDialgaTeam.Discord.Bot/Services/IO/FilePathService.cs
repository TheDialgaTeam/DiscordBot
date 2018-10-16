using System;
using System.IO;
using TheDialgaTeam.DependencyInjection.ProgramLoop;

namespace TheDialgaTeam.Discord.Bot.Services.IO
{
    public sealed class FilePathService : IInitializable
    {
        public string ConsoleLogFilePath { get; } = ResolveFullPath($"{Environment.CurrentDirectory}/Logs/{DateTime.UtcNow:yyyy-MM-dd}.txt");

        public string SQLiteDatabaseFilePath { get; } = ResolveFullPath($"{Environment.CurrentDirectory}/Data/Application.db");

        private static string ResolveFullPath(string filePath)
        {
            return Path.GetFullPath(ResolveFilePath(filePath));
        }

        private static string ResolveFilePath(string filePath)
        {
            return filePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }

        public void Initialize()
        {
            if (!Directory.Exists(ResolveFullPath($"{Environment.CurrentDirectory}/Logs")))
                Directory.CreateDirectory(ResolveFullPath($"{Environment.CurrentDirectory}/Logs"));

            if (!Directory.Exists(ResolveFullPath($"{Environment.CurrentDirectory}/Data")))
                Directory.CreateDirectory(ResolveFullPath($"{Environment.CurrentDirectory}/Data"));
        }
    }
}