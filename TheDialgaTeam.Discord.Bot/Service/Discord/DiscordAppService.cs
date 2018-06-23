using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TheDialgaTeam.Discord.Bot.Model.Discord;
using TheDialgaTeam.Discord.Bot.Model.Discord.Command;
using TheDialgaTeam.Discord.Bot.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Service.Logger;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Service.Discord
{
    public interface IDiscordAppService
    {
        List<IDiscordAppInstance> DiscordAppInstances { get; }

        Task<ICommandExecuteResult> StartDiscordAppAsync(ulong clientId);

        Task<ICommandExecuteResult> StartAllDiscordAppsAsync();

        Task<ICommandExecuteResult> StopDiscordAppAsync(ulong clientId);

        Task<ICommandExecuteResult> StopAllDiscordAppsAsync();
    }

    internal class DiscordAppService : IDiscordAppService
    {
        public List<IDiscordAppInstance> DiscordAppInstances { get; } = new List<IDiscordAppInstance>();

        private ILoggerService LoggerService { get; }

        private ISQLiteService SQLiteService { get; }

        private SemaphoreSlim DiscordAppInstancesLock { get; } = new SemaphoreSlim(1, 1);

        private Task BackgroundTask { get; }

        public DiscordAppService(ILoggerService loggerService, ISQLiteService sqliteService)
        {
            LoggerService = loggerService;
            SQLiteService = sqliteService;
            BackgroundTask = DiscordAppInstanceCheckTask();
        }

        public async Task<ICommandExecuteResult> StartDiscordAppAsync(ulong clientId)
        {
            await StopDiscordAppAsync(clientId).ConfigureAwait(false);

            try
            {
                await DiscordAppInstancesLock.WaitAsync().ConfigureAwait(false);

                var clientIdString = clientId.ToString();
                var discordAppTable = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);

                if (string.IsNullOrEmpty(discordAppTable?.BotToken))
                    return CommandExecuteResult.FromError("Failed to request a discord app instance to start.");

                var isSuccess = true;
                var discordApp = new DiscordAppInstance(Convert.ToUInt64(discordAppTable.ClientId), discordAppTable.BotToken);

                try
                {
                    await InternalStartDiscordAppAsync(discordApp).ConfigureAwait(false);
                    DiscordAppInstances.Add(discordApp);
                }
                catch (Exception)
                {
                    await InternalStopDiscordAppAsync(discordApp);
                    isSuccess = false;
                }

                return isSuccess ? CommandExecuteResult.FromSuccess("Successfully requested a discord app instance to start.") : CommandExecuteResult.FromError("Failed to request a discord app instance to start.");
            }
            finally
            {
                DiscordAppInstancesLock.Release();
            }
        }

        public async Task<ICommandExecuteResult> StartAllDiscordAppsAsync()
        {
            if (DiscordAppInstances.Count > 0)
                await StopAllDiscordAppsAsync().ConfigureAwait(false);

            try
            {
                await DiscordAppInstancesLock.WaitAsync().ConfigureAwait(false);

                var discordAppTables = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);
                var isSuccess = true;

                foreach (var discordAppTable in discordAppTables)
                {
                    if (string.IsNullOrEmpty(discordAppTable.ClientId) || string.IsNullOrEmpty(discordAppTable.BotToken))
                        continue;

                    var discordApp = new DiscordAppInstance(Convert.ToUInt64(discordAppTable.ClientId), discordAppTable.BotToken);

                    try
                    {
                        await InternalStartDiscordAppAsync(discordApp).ConfigureAwait(false);
                        DiscordAppInstances.Add(discordApp);
                    }
                    catch (Exception)
                    {
                        await InternalStopDiscordAppAsync(discordApp);
                        isSuccess = false;
                    }
                }

                return isSuccess ? CommandExecuteResult.FromSuccess("Successfully requested all discord app instances to start.") : CommandExecuteResult.FromError("Failed to request all discord app instances to start.");
            }
            finally
            {
                DiscordAppInstancesLock.Release();
            }
        }

        public async Task<ICommandExecuteResult> StopDiscordAppAsync(ulong clientId)
        {
            try
            {
                await DiscordAppInstancesLock.WaitAsync().ConfigureAwait(false);

                if (DiscordAppInstances.Count == 0)
                    return CommandExecuteResult.FromError("There are no discord app instances running.");

                for (var i = DiscordAppInstances.Count - 1; i >= 0; i--)
                {
                    if (DiscordAppInstances[i].ClientId != clientId)
                        continue;

                    await InternalStopDiscordAppAsync(DiscordAppInstances[i]).ConfigureAwait(false);
                    DiscordAppInstances.Remove(DiscordAppInstances[i]);

                    break;
                }

                return CommandExecuteResult.FromSuccess("Successfully requested a discord app instance to stop.");
            }
            finally
            {
                DiscordAppInstancesLock.Release();
            }
        }

        public async Task<ICommandExecuteResult> StopAllDiscordAppsAsync()
        {
            try
            {
                await DiscordAppInstancesLock.WaitAsync().ConfigureAwait(false);

                if (DiscordAppInstances.Count == 0)
                    return CommandExecuteResult.FromError("There are no discord app instances running.");

                foreach (var discordShardedClientHelper in DiscordAppInstances)
                    await InternalStopDiscordAppAsync(discordShardedClientHelper).ConfigureAwait(false);

                DiscordAppInstances.Clear();

                return CommandExecuteResult.FromSuccess("Successfully requested all discord app instances to stop.");
            }
            finally
            {
                DiscordAppInstancesLock.Release();
            }
        }

        private async Task InternalStartDiscordAppAsync(IDiscordAppInstance discordAppInstance)
        {
            AddListener(discordAppInstance);
            await discordAppInstance.StartDiscordAppAsync().ConfigureAwait(false);
        }

        private async Task InternalStopDiscordAppAsync(IDiscordAppInstance discordAppInstance)
        {
            await discordAppInstance.StopDiscordAppAsync().ConfigureAwait(false);
            discordAppInstance.Dispose();
            RemoveListener(discordAppInstance);
        }

        private Task DiscordAppInstanceCheckTask()
        {
            return Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await DiscordAppInstancesLock.WaitAsync().ConfigureAwait(false);

                    try
                    {
                        for (var i = DiscordAppInstances.Count - 1; i >= 0; i--)
                        {
                            var discordAppInstance = DiscordAppInstances[i];

                            // Check if instance is verified or not.
                            if (!discordAppInstance.IsVerified)
                            {
                                var shouldKeep = await VerifyDiscordAppInstance(discordAppInstance).ConfigureAwait(false);

                                if (!shouldKeep)
                                {
                                    DiscordAppInstances.Remove(discordAppInstance);

                                    continue;
                                }
                            }

                            // Check if instance is logged in.
                            try
                            {
                                if (discordAppInstance.DiscordShardedClient.LoginState == LoginState.LoggingOut || discordAppInstance.DiscordShardedClient.LoginState == LoginState.LoggedOut)
                                    await discordAppInstance.RequestLoginAsync().ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                await LoggerService.LogErrorMessageAsync(ex).ConfigureAwait(false);

                                continue;
                            }

                            // Check if instance is connected.
                            foreach (var discordSocketClient in discordAppInstance.DiscordShardedClient.Shards)
                            {
                                try
                                {
                                    if (discordSocketClient.ConnectionState == ConnectionState.Disconnected || discordSocketClient.ConnectionState == ConnectionState.Disconnecting)
                                        await discordSocketClient.StartAsync().ConfigureAwait(false);
                                }
                                catch (Exception ex)
                                {
                                    await LoggerService.LogErrorMessageAsync(ex).ConfigureAwait(false);
                                }
                            }
                        }
                    }
                    finally
                    {
                        DiscordAppInstancesLock.Release();
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }, TaskCreationOptions.LongRunning);
        }

        private async Task<bool> VerifyDiscordAppInstance(IDiscordAppInstance discordAppInstance)
        {
            if (discordAppInstance.IsVerified)
                return true;

            if (discordAppInstance.DiscordShardedClient.CurrentUser == null)
                return true;

            var clientIdString = discordAppInstance.ClientId.ToString();

            if (discordAppInstance.ClientId != discordAppInstance.DiscordShardedClient.CurrentUser.Id)
            {
                await LoggerService.LogMessageAsync(discordAppInstance.DiscordShardedClient, new LogMessage(LogSeverity.Error, "", "Discord App client id mismatch found! Forced application to stop!")).ConfigureAwait(false);
                await InternalStopDiscordAppAsync(discordAppInstance).ConfigureAwait(false);
                await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().DeleteAsync(a => a.ClientId == clientIdString).ConfigureAwait(false);

                return false;
            }

            var discordAppTable = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);
            var discordAppInstanceInfo = await discordAppInstance.DiscordShardedClient.GetApplicationInfoAsync().ConfigureAwait(false);

            discordAppTable.AppName = discordAppInstanceInfo.Name;
            discordAppTable.AppDescription = discordAppInstanceInfo.Description;
            discordAppTable.LastUpdateCheck = DateTimeOffset.Now;

            await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordAppTable).ConfigureAwait(false);

            discordAppInstance.IsVerified = true;

            return true;
        }

        private void AddListener(IDiscordAppInstance discordAppInstance)
        {
            discordAppInstance.Log += DiscordAppInstanceOnLog;
            discordAppInstance.ShardReady += DiscordAppInstanceOnShardReady;
        }

        private void RemoveListener(IDiscordAppInstance discordAppInstance)
        {
            discordAppInstance.ShardReady -= DiscordAppInstanceOnShardReady;
            discordAppInstance.Log -= DiscordAppInstanceOnLog;
        }

        private Task DiscordAppInstanceOnLog(IDiscordAppInstance discordAppInstance, LogMessage logMessage)
        {
            Task.Run(async () => await LoggerService.LogMessageAsync(discordAppInstance.DiscordShardedClient, logMessage).ConfigureAwait(false)).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private Task DiscordAppInstanceOnShardReady(IDiscordAppInstance arg1, DiscordSocketClient arg2)
        {
            throw new NotImplementedException();
        }
    }
}