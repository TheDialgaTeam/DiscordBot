using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TheDialgaTeam.DependencyInjection.ProgramLoop;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Services.Console
{
    public sealed class LoggerService : IInitializable, ITickableAsync, IDisposableAsync, IErrorLogger
    {
        private FilePathService FilePathService { get; }

        private ConcurrentQueue<(TextWriter writer, ConsoleColor consoleColor, string message)> WriteToLogQueue { get; } = new ConcurrentQueue<(TextWriter writer, ConsoleColor consoleColor, string message)>();

        private StreamWriter StreamWriter { get; set; }

        public LoggerService(FilePathService filePathService)
        {
            FilePathService = filePathService;
        }

        public void Initialize()
        {
            StreamWriter = new StreamWriter(new FileStream(FilePathService.ConsoleLogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));

            LogMessage("==================================================");
            LogMessage("The Dialga Team Discord Bot (.NET Core)");
            LogMessage("==================================================");
            LogMessage("Initializing Application...\n");
        }

        public async Task TickAsync()
        {
            while (WriteToLogQueue.TryDequeue(out var item))
            {
                try
                {
                    System.Console.ForegroundColor = item.consoleColor;

                    await item.writer.WriteLineAsync(item.message).ConfigureAwait(false);
                    await item.writer.FlushAsync().ConfigureAwait(false);

                    await StreamWriter.WriteLineAsync($"{DateTime.UtcNow:u} {item.message}").ConfigureAwait(false);
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
                }
            }
        }

        public void LogMessage(string message, ConsoleColor consoleColor = ConsoleColor.White)
        {
            WriteToLogQueue.Enqueue((System.Console.Out, consoleColor, message));
        }

        public void LogMessage(DiscordShardedClient discordShardedClient, LogMessage logMessage)
        {
            var botId = discordShardedClient?.CurrentUser?.Id;
            var botName = discordShardedClient?.CurrentUser?.ToString();
            var message = discordShardedClient?.CurrentUser == null ? $"[Bot] {logMessage.ToString()}" : $"[Bot {botId}] {botName}:\n{logMessage.ToString()}";

            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    WriteToLogQueue.Enqueue((System.Console.Error, ConsoleColor.Red, message));
                    break;

                case LogSeverity.Error:
                    WriteToLogQueue.Enqueue((System.Console.Error, ConsoleColor.Red, message));
                    break;

                case LogSeverity.Warning:
                    WriteToLogQueue.Enqueue((System.Console.Out, ConsoleColor.Yellow, message));
                    break;

                case LogSeverity.Info:
                    WriteToLogQueue.Enqueue((System.Console.Out, ConsoleColor.White, message));
                    break;

                case LogSeverity.Verbose:
                    WriteToLogQueue.Enqueue((System.Console.Out, ConsoleColor.White, message));
                    break;

                case LogSeverity.Debug:
                    WriteToLogQueue.Enqueue((System.Console.Out, ConsoleColor.White, message));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void LogErrorMessage(Exception exception)
        {
            WriteToLogQueue.Enqueue((System.Console.Error, ConsoleColor.Red, exception.ToString()));
        }

        public async Task DisposeAsync()
        {
            await TickAsync().ConfigureAwait(false);
            StreamWriter?.Dispose();
        }
    }
}