using Discord;
using System;
using System.IO;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Extension.System.IO;

namespace TheDialgaTeam.DiscordBot.Services
{
    public interface ILoggerService
    {
        Task<string> ReadLineAsync();

        Task LogMessageAsync(string message);

        Task LogMessageAsync(LogMessage logMessage);

        Task LogErrorMessageAsync(string message);

        Task LogErrorMessageAsync(Exception exception);
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

        public async Task LogMessageAsync(LogMessage logMessage)
        {
            if (logMessage.Exception != null)
                await InternalMessageAsync(Console.Error, logMessage.Exception.ToString());
            else
                await InternalMessageAsync(Console.Out, logMessage.Message);
        }

        public async Task LogErrorMessageAsync(string message)
        {
            await InternalMessageAsync(Console.Error, message);
        }

        public async Task LogErrorMessageAsync(Exception exception)
        {
            await InternalMessageAsync(Console.Error, exception.ToString());
        }

        private async Task InternalMessageAsync(TextWriter writer, string message)
        {
            await writer.WriteLineAsync(message);
            await StreamWriter.WriteLineAsync($"{DateTime.UtcNow:HH:mm:ss} {message}");
            await StreamWriter.FlushAsync();
        }
    }
}