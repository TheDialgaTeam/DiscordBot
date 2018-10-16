using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TheDialgaTeam.DependencyInjection.ProgramLoop;
using TheDialgaTeam.Discord.Bot.Old.Model.Discord;
using TheDialgaTeam.Discord.Bot.Old.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Old.Service.SQLite;
using TheDialgaTeam.Discord.Bot.Services.Console;

namespace TheDialgaTeam.Discord.Bot.Old.Service.Discord
{
    public sealed class DiscordAppService : IInitializableAsync, ITickableAsync, IDisposableAsync
    {
        private LoggerService LoggerService { get; }

        private SQLiteService SQLiteService { get; }

        private Program Program { get; }

        private SynchronizedCollection<DiscordAppInstance> DiscordAppInstances { get; } = new SynchronizedCollection<DiscordAppInstance>();

        public DiscordAppService(LoggerService loggerService, SQLiteService sqliteService, Program program)
        {
            LoggerService = loggerService;
            SQLiteService = sqliteService;
            Program = program;
        }

        public async Task InitializeAsync()
        {
            var discordApps = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);

            foreach (var discordAppTable in discordApps)
                await StartDiscordAppAsync(Convert.ToUInt64(discordAppTable.ClientId)).ConfigureAwait(false);
        }

        public async Task TickAsync()
        {
            for (var i = DiscordAppInstances.Count - 1; i >= 0; i--)
            {
                var discordAppInstance = DiscordAppInstances[i];

                // check if discord app is verified.
                if (!discordAppInstance.IsVerified)
                {
                    // Wait for current user to be downloaded from discord server.
                    if (discordAppInstance.DiscordShardedClient.CurrentUser == null)
                        continue;

                    var clientIdString = discordAppInstance.ClientId.ToString();

                    if (discordAppInstance.ClientId != discordAppInstance.DiscordShardedClient.CurrentUser.Id)
                    {
                        LoggerService.LogMessage(discordAppInstance.DiscordShardedClient, new LogMessage(LogSeverity.Error, "", "Discord App client id mismatch found! Forced application to stop!"));
                        await StopDiscordAppAsync(discordAppInstance.ClientId).ConfigureAwait(false);
                        await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().DeleteAsync(a => a.ClientId == clientIdString).ConfigureAwait(false);
                        continue;
                    }

                    var discordAppTable = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);
                    var discordAppInstanceInfo = await discordAppInstance.DiscordShardedClient.GetApplicationInfoAsync().ConfigureAwait(false);

                    discordAppTable.AppName = discordAppInstanceInfo.Name;
                    discordAppTable.AppDescription = discordAppInstanceInfo.Description;
                    discordAppTable.LastUpdateCheck = DateTimeOffset.Now;

                    await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordAppTable).ConfigureAwait(false);

                    discordAppInstance.IsVerified = true;
                }

                if (discordAppInstance.NextCheck == null)
                    discordAppInstance.NextCheck = DateTimeOffset.Now.AddMinutes(15);

                if (DateTimeOffset.Now < discordAppInstance.NextCheck)
                    continue;

                // Check if discord app is logged in.
                try
                {
                    if (discordAppInstance.DiscordShardedClient.LoginState == LoginState.LoggingOut || discordAppInstance.DiscordShardedClient.LoginState == LoginState.LoggedOut)
                        await discordAppInstance.DiscordAppLoginAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LoggerService.LogErrorMessage(ex);
                }

                // Check if discord app is connected.
                foreach (var discordSocketClient in discordAppInstance.DiscordShardedClient.Shards)
                {
                    try
                    {
                        if (discordSocketClient.ConnectionState == ConnectionState.Disconnected || discordSocketClient.ConnectionState == ConnectionState.Disconnecting)
                            await discordSocketClient.StartAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        LoggerService.LogErrorMessage(ex);
                    }
                }

                discordAppInstance.NextCheck = DateTimeOffset.Now.AddMinutes(15);
            }
        }

        public async Task<CommandExecuteResult> StartDiscordAppAsync(ulong clientId)
        {
            await StopDiscordAppAsync(clientId).ConfigureAwait(false);

            var clientIdString = clientId.ToString();
            var discordAppTable = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(discordAppTable?.BotToken))
                return CommandExecuteResult.FromError("Discord App is not registered in the database.");

            var discordApp = new DiscordAppInstance(Convert.ToUInt64(discordAppTable.ClientId), discordAppTable.BotToken);

            AddListener(discordApp);

            try
            {
                await discordApp.DiscordAppLoginAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await discordApp.DiscordAppLogoutAsync().ConfigureAwait(false);
                LoggerService.LogErrorMessage(ex);
            }

            if (!discordApp.IsLoggedIn)
                return CommandExecuteResult.FromError("Discord App encountered an error while trying to authenticate.");

            try
            {
                await discordApp.DiscordAppStartAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggerService.LogErrorMessage(ex);
            }

            if (!discordApp.IsStarted)
            {
                RemoveListener(discordApp);
                return CommandExecuteResult.FromError("Discord App encountered an error while trying to connect to the discord gateway server.");
            }

            DiscordAppInstances.Add(discordApp);

            return CommandExecuteResult.FromSuccess("Discord App is now starting!");
        }

        public async Task<CommandExecuteResult> StopDiscordAppAsync(ulong clientId)
        {
            for (var i = DiscordAppInstances.Count - 1; i >= 0; i--)
            {
                var discordAppInstance = DiscordAppInstances[i];

                if (discordAppInstance.ClientId != clientId)
                    continue;

                var instanceName = discordAppInstance.DiscordShardedClient?.CurrentUser?.ToString();

                try
                {
                    await discordAppInstance.DiscordAppStopAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LoggerService.LogErrorMessage(ex);
                }

                try
                {
                    await discordAppInstance.DiscordAppLogoutAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LoggerService.LogErrorMessage(ex);
                }

                RemoveListener(discordAppInstance);

                discordAppInstance.Dispose();
                DiscordAppInstances.Remove(discordAppInstance);

                return CommandExecuteResult.FromSuccess($"Discord App {instanceName} has stopped.");
            }

            return CommandExecuteResult.FromError("Discord App is not running!");
        }

        private void AddListener(DiscordAppInstance discordAppInstance)
        {
            discordAppInstance.Log += DiscordAppInstanceOnLog;
            discordAppInstance.ShardReady += DiscordAppInstanceOnShardReady;
            discordAppInstance.MessageReceived += DiscordAppInstanceOnMessageReceived;
        }

        private void RemoveListener(DiscordAppInstance discordAppInstance)
        {
            discordAppInstance.MessageReceived -= DiscordAppInstanceOnMessageReceived;
            discordAppInstance.ShardReady -= DiscordAppInstanceOnShardReady;
            discordAppInstance.Log -= DiscordAppInstanceOnLog;
        }

        private Task DiscordAppInstanceOnLog(DiscordAppInstance discordAppInstance, LogMessage logMessage)
        {
            Task.Run(() => LoggerService.LogMessage(discordAppInstance.DiscordShardedClient, logMessage)).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private Task DiscordAppInstanceOnShardReady(DiscordAppInstance discordAppInstance, DiscordSocketClient discordSocketClient)
        {
            Task.Run(async () =>
            {
                await discordSocketClient.SetGameAsync($"@{discordAppInstance.DiscordShardedClient.CurrentUser.Username} help").ConfigureAwait(false);
                LoggerService.LogMessage($"{discordAppInstance.DiscordShardedClient.CurrentUser}: Shard {discordSocketClient.ShardId + 1}/{discordAppInstance.DiscordShardedClient.Shards.Count} is ready!", ConsoleColor.Green);
            }).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private Task DiscordAppInstanceOnMessageReceived(DiscordAppInstance discordAppInstance, SocketMessage socketMessage)
        {
            Task.Run(async () =>
            {
                if (!(socketMessage is SocketUserMessage socketUserMessage))
                    return;

                ICommandContext context = new ShardedCommandContext(discordAppInstance.DiscordShardedClient, socketUserMessage);
                var argPos = 0;

                if (socketUserMessage.Channel is SocketDMChannel)
                    socketUserMessage.HasMentionPrefix(discordAppInstance.DiscordShardedClient.CurrentUser, ref argPos);
                else
                {
                    var discordAppId = await SQLiteService.GetDiscordAppIdAsync(context.Client.CurrentUser.Id).ConfigureAwait(false);
                    var guildId = context.Guild.Id.ToString();
                    var discordGuild = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildTable>().Where(a => a.DiscordAppId == discordAppId && a.GuildId == guildId).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (discordGuild == null && !socketUserMessage.HasMentionPrefix(discordAppInstance.DiscordShardedClient.CurrentUser, ref argPos))
                        return;

                    if (!socketUserMessage.HasMentionPrefix(discordAppInstance.DiscordShardedClient.CurrentUser, ref argPos) &&
                        !socketUserMessage.HasStringPrefix(discordGuild?.Prefix ?? "", ref argPos, StringComparison.OrdinalIgnoreCase))
                        return;
                }

                await Program.CommandService.ExecuteAsync(context, argPos, Program.ServiceProvider).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            for (var i = DiscordAppInstances.Count - 1; i >= 0; i--)
            {
                var discordAppInstance = DiscordAppInstances[i];
                await StopDiscordAppAsync(discordAppInstance.ClientId).ConfigureAwait(false);
            }
        }
    }
}