﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using TheDialgaTeam.DiscordBot.Extension.System.IO;
using TheDialgaTeam.DiscordBot.Model.Discord;

namespace TheDialgaTeam.DiscordBot.Services.Logger
{
    public interface ILoggerService
    {
        Task<string> ReadLineAsync();

        Task LogMessageAsync(string message);

        Task LogMessageAsync(IDiscordShardedClientHelper discordShardedClientHelper, LogMessage logMessage);

        Task LogErrorMessageAsync(string message);

        Task LogErrorMessageAsync(Exception exception);
    }

    internal sealed class LoggerService : ILoggerService
    {
        private string ConsoleLogBasePath { get; } = $"{Environment.CurrentDirectory}/Logs/{DateTime.UtcNow:yyyy-MM-dd}.txt".ResolveFullPath();

        private StreamWriter StreamWriter { get; }

        private SemaphoreSlim StreamLock { get; } = new SemaphoreSlim(1, 1);

        public LoggerService()
        {
            var directory = $"{Environment.CurrentDirectory}/Logs".ResolveFullPath();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            StreamWriter = new StreamWriter(new FileStream(ConsoleLogBasePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
        }

        public async Task<string> ReadLineAsync()
        {
            return await Console.In.ReadLineAsync().ConfigureAwait(false);
        }

        public async Task LogMessageAsync(string message)
        {
            await InternalMessageAsync(Console.Out, ConsoleColor.White, $"{message}").ConfigureAwait(false);
        }

        public async Task LogMessageAsync(IDiscordShardedClientHelper discordShardedClientHelper, LogMessage logMessage)
        {
            var botId = discordShardedClientHelper?.DiscordShardedClient?.CurrentUser?.Id;
            var botName = discordShardedClientHelper?.DiscordShardedClient?.CurrentUser?.ToString();

            var message = discordShardedClientHelper?.DiscordShardedClient?.CurrentUser == null ? $"[Bot] {logMessage.ToString()}" : $"[Bot {botId}] {botName}:\n{logMessage.ToString()}";

            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    await InternalMessageAsync(Console.Error, ConsoleColor.Red, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Error:
                    await InternalMessageAsync(Console.Error, ConsoleColor.Red, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Warning:
                    await InternalMessageAsync(Console.Out, ConsoleColor.Yellow, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Info:
                    await InternalMessageAsync(Console.Out, ConsoleColor.White, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Verbose:
                    await InternalMessageAsync(Console.Out, ConsoleColor.White, message).ConfigureAwait(false);

                    break;

                case LogSeverity.Debug:
                    await InternalMessageAsync(Console.Out, ConsoleColor.White, message).ConfigureAwait(false);

                    break;

                default:

                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task LogErrorMessageAsync(string message)
        {
            await InternalMessageAsync(Console.Error, ConsoleColor.Red, $"{message}").ConfigureAwait(false);
        }

        public async Task LogErrorMessageAsync(Exception exception)
        {
            await InternalMessageAsync(Console.Error, ConsoleColor.Red, $"{exception}").ConfigureAwait(false);
        }

        private async Task InternalMessageAsync(TextWriter writer, ConsoleColor consoleColor, string message)
        {
            await StreamLock.WaitAsync().ConfigureAwait(false);

            try
            {
                Console.ForegroundColor = consoleColor;

                await writer.WriteLineAsync(message).ConfigureAwait(false);
                await StreamWriter.WriteLineAsync($"{DateTime.UtcNow:HH:mm:ss} {message}").ConfigureAwait(false);
                await StreamWriter.FlushAsync().ConfigureAwait(false);

                Console.ResetColor();
            }
            finally
            {
                StreamLock.Release();
            }
        }
    }
}