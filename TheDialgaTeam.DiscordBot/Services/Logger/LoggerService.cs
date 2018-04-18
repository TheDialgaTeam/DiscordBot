using Discord;
using System;
using System.IO;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Extension.System.IO;
using TheDialgaTeam.DiscordBot.Model.Discord;

namespace TheDialgaTeam.DiscordBot.Services.Logger
{
    public interface ILoggerService
    {
        Task<string> ReadLineAsync();

        Task LogMessageAsync(string message);

        Task LogMessageAsync(IDiscordShardedClientModel discordShardedClientModel, LogMessage logMessage);

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
            await InternalMessageAsync(Console.Out, ConsoleColor.White, $"{message}");
        }

        public async Task LogMessageAsync(IDiscordShardedClientModel discordShardedClientModel, LogMessage logMessage)
        {
            var message = $"[Bot {discordShardedClientModel.DiscordAppModel.ClientId}] {(string.IsNullOrEmpty(discordShardedClientModel.DiscordShardedClient?.CurrentUser?.Username) ? logMessage.ToString() : discordShardedClientModel.DiscordShardedClient?.CurrentUser?.Username + ":\n" + logMessage.ToString())}";

            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    await InternalMessageAsync(Console.Error, ConsoleColor.Red, message);
                    break;

                case LogSeverity.Error:
                    await InternalMessageAsync(Console.Error, ConsoleColor.Red, message);
                    break;

                case LogSeverity.Warning:
                    await InternalMessageAsync(Console.Out, ConsoleColor.Yellow, message);
                    break;

                case LogSeverity.Info:
                    await InternalMessageAsync(Console.Out, ConsoleColor.White, message);
                    break;

                case LogSeverity.Verbose:
                    await InternalMessageAsync(Console.Out, ConsoleColor.White, message);
                    break;

                case LogSeverity.Debug:
                    await InternalMessageAsync(Console.Out, ConsoleColor.White, message);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task LogErrorMessageAsync(string message)
        {
            await InternalMessageAsync(Console.Error, ConsoleColor.Red, $"{message}");
        }

        public async Task LogErrorMessageAsync(Exception exception)
        {
            await InternalMessageAsync(Console.Error, ConsoleColor.Red, $"{exception}");
        }

        private async Task InternalMessageAsync(TextWriter writer, ConsoleColor consoleColor, string message)
        {
            Console.ForegroundColor = consoleColor;

            await writer.WriteLineAsync(message);
            await StreamWriter.WriteLineAsync($"{DateTime.UtcNow:HH:mm:ss} {message}");
            await StreamWriter.FlushAsync();

            Console.ResetColor();
        }
    }
}