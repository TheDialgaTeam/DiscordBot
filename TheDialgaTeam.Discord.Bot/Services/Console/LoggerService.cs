using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TheDialgaTeam.DependencyInjection;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Services.Console
{
    public sealed class LoggerService : IInitializableAsync, IErrorLogger, IDisposableAsync
    {
        private FilePathService FilePathService { get; }

        private StreamWriter StreamWriter { get; set; }

        private SemaphoreSlim StreamWriterLock { get; set; }

        public LoggerService(FilePathService filePathService)
        {
            FilePathService = filePathService;
        }

        public async Task InitializeAsync()
        {
            StreamWriter = new StreamWriter(new FileStream(FilePathService.ConsoleLogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            StreamWriterLock = new SemaphoreSlim(1, 1);

            await LogMessageAsync("==================================================").ConfigureAwait(false);
            await LogMessageAsync("The Dialga Team Discord Bot (.NET Core)").ConfigureAwait(false);
            await LogMessageAsync("==================================================").ConfigureAwait(false);
            await LogMessageAsync("Initializing Application...\n").ConfigureAwait(false);
        }

        public async Task LogErrorMessageAsync(Exception exception)
        {
            await LogMessageAsync(System.Console.Error, ConsoleColor.Red, exception.ToString()).ConfigureAwait(false);
        }

        public Task DisposeAsync()
        {
            StreamWriter?.Dispose();
            StreamWriterLock?.Dispose();

            return Task.CompletedTask;
        }

        public async Task LogMessageAsync(string message, ConsoleColor consoleColor = ConsoleColor.White)
        {
            await LogMessageAsync(System.Console.Out, consoleColor, message).ConfigureAwait(false);
        }

        public async Task LogMessageAsync(DiscordShardedClient discordShardedClient, LogMessage logMessage)
        {
            var botId = discordShardedClient?.CurrentUser?.Id;
            var botName = discordShardedClient?.CurrentUser?.ToString();
            var message = discordShardedClient?.CurrentUser == null ? $"[Bot] {logMessage.ToString()}" : $"[Bot {botId}] {botName}:\n{logMessage.ToString()}";

            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    await LogMessageAsync(System.Console.Error, ConsoleColor.Red, message).ConfigureAwait(false);
                    break;

                case LogSeverity.Error:
                    await LogMessageAsync(System.Console.Error, ConsoleColor.Red, message).ConfigureAwait(false);
                    break;

                case LogSeverity.Warning:
                    await LogMessageAsync(System.Console.Out, ConsoleColor.Yellow, message).ConfigureAwait(false);
                    break;

                case LogSeverity.Info:
                    await LogMessageAsync(System.Console.Out, ConsoleColor.White, message).ConfigureAwait(false);
                    break;

                case LogSeverity.Verbose:
                    await LogMessageAsync(System.Console.Out, ConsoleColor.White, message).ConfigureAwait(false);
                    break;

                case LogSeverity.Debug:
                    await LogMessageAsync(System.Console.Out, ConsoleColor.Cyan, message).ConfigureAwait(false);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task LogMessageAsync(TextWriter writer, ConsoleColor consoleColor, string message)
        {
            if (message == null)
                return;

            await StreamWriterLock.WaitAsync();

            try
            {
                System.Console.ForegroundColor = consoleColor;

                await writer.WriteLineAsync(message).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);

                await StreamWriter.WriteLineAsync($"{DateTime.UtcNow:u} {message}").ConfigureAwait(false);
                await StreamWriter.FlushAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                await System.Console.Error.WriteLineAsync(ex.ToString()).ConfigureAwait(false);
            }
            finally
            {
                System.Console.ResetColor();
                StreamWriterLock.Release();
            }
        }
    }
}