using System;
using System.IO;
using System.Threading.Tasks;
using TheDialgaTeam.Modules.System.IO;

namespace TheDialgaTeam.DiscordBot.Services
{
    public interface ILoggerService
    {
        Task<string> ReadLineAsync();

        Task LogMessageAsync(string message);

        Task LogErrorMessageAsync(string message);
    }

    internal sealed class LoggerService : ILoggerService
    {
        private string ConsoleLogBasePath { get; } = $"{Environment.CurrentDirectory}/Logs/{DateTime.UtcNow:yyyy-MM-dd}.txt".ResolveFullPath();

        private StreamWriter StreamWriter { get; }

        public LoggerService()
        {
            var directory = $"{Environment.CurrentDirectory}/Logs".ResolveFullPath();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            StreamWriter = new StreamWriter(new FileStream(ConsoleLogBasePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
        }

        public async Task<string> ReadLineAsync()
        {
            return await Console.In.ReadLineAsync();
        }

        public async Task LogMessageAsync(string message)
        {
            await InternalMessageAsync(Console.Out, message);
        }

        public async Task LogErrorMessageAsync(string message)
        {
            await InternalMessageAsync(Console.Error, message);
        }

        private async Task InternalMessageAsync(TextWriter writer, string message)
        {
            await writer.WriteLineAsync(message);
            await StreamWriter.WriteLineAsync($"{DateTime.UtcNow:HH:mm:ss} {message}");
            await StreamWriter.FlushAsync();
        }
    }
}