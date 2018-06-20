using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TheDialgaTeam.Discord.Bot.Service.IO;

namespace TheDialgaTeam.Discord.Bot.Service.Logger
{
    public interface ILoggerService : IDisposable
    {
        Task<string> ReadLineAsync();

        Task LogMessageAsync(string message, ConsoleColor consoleColor = ConsoleColor.White);

        Task LogMessageAsync(DiscordShardedClient discordShardedClient, LogMessage logMessage);

        Task LogErrorMessageAsync(Exception exception);
    }

    internal sealed class LoggerService : ILoggerService
    {
        private StreamWriter StreamWriter { get; }

        private SemaphoreSlim StreamWriterLock { get; } = new SemaphoreSlim(1, 1);

        public LoggerService(IFilePathService filePathService)
        {
            StreamWriter = new StreamWriter(new FileStream(filePathService.ConsoleLogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
        }

        public async Task<string> ReadLineAsync()
        {
            return await Console.In.ReadLineAsync().ConfigureAwait(false);
        }

        public async Task LogMessageAsync(string message, ConsoleColor consoleColor = ConsoleColor.White)
        {
            await InternalLogMessageAsync(Console.Out, consoleColor, message).ConfigureAwait(false);
        }

        public async Task LogMessageAsync(DiscordShardedClient discordShardedClient, LogMessage logMessage)
        {
            var botId = discordShardedClient?.CurrentUser?.Id;
            var botName = discordShardedClient?.CurrentUser?.ToString();

            var message = discordShardedClient?.CurrentUser == null ? $"[Bot] {logMessage.ToString()}" : $"[Bot {botId}] {botName}:\n{logMessage.ToString()}";

            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    await InternalLogMessageAsync(Console.Error, ConsoleColor.Red, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Error:
                    await InternalLogMessageAsync(Console.Error, ConsoleColor.Red, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Warning:
                    await InternalLogMessageAsync(Console.Out, ConsoleColor.Yellow, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Info:
                    await InternalLogMessageAsync(Console.Out, ConsoleColor.White, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Verbose:
                    await InternalLogMessageAsync(Console.Out, ConsoleColor.White, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Debug:
                    await InternalLogMessageAsync(Console.Out, ConsoleColor.White, message).ConfigureAwait(false);

                    break;

                default:

                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task LogErrorMessageAsync(Exception exception)
        {
            await InternalLogMessageAsync(Console.Error, ConsoleColor.Red, exception.ToString()).ConfigureAwait(false);
        }

        private async Task InternalLogMessageAsync(TextWriter writer, ConsoleColor consoleColor, string message)
        {
            try
            {
                await StreamWriterLock.WaitAsync().ConfigureAwait(false);

                Console.ForegroundColor = consoleColor;

                await writer.WriteLineAsync(message).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);

                await StreamWriter.WriteLineAsync($"{DateTime.UtcNow:HH:mm:ss} {message}").ConfigureAwait(false);
                await StreamWriter.FlushAsync().ConfigureAwait(false);

                Console.ResetColor();
            }
            finally
            {
                StreamWriterLock.Release();
            }
        }

        public void Dispose()
        {
            StreamWriter?.Dispose();
            StreamWriterLock?.Dispose();
        }
    }
}