using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TheDialgaTeam.Discord.Bot.Service.IO;

namespace TheDialgaTeam.Discord.Bot.Service.Logger
{
    public sealed class LoggerService : IDisposable
    {
        private StreamWriter StreamWriter { get; }

        private SemaphoreSlim StreamWriterLock { get; } = new SemaphoreSlim(1, 1);

        public LoggerService(FilePathService filePathService)
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

                await StreamWriter.WriteLineAsync($"{DateTime.UtcNow:u} {message}").ConfigureAwait(false);
                await StreamWriter.FlushAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                await Console.Error.WriteLineAsync(ex.ToString()).ConfigureAwait(false);
            }
            finally
            {
                StreamWriterLock.Release();
                Console.ResetColor();
            }
        }

        public void Dispose()
        {
            StreamWriter?.Dispose();
            StreamWriterLock?.Dispose();
        }
    }
}