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
        List<IDiscordShardedClientHelper> DiscordShardedClientHelpers { get; }

        Task<ICommandExecuteResult> StartDiscordAppAsync(ulong clientId);

        Task<ICommandExecuteResult> StartDiscordAppsAsync();

        Task<ICommandExecuteResult> StopDiscordAppAsync(ulong clientId);

        Task<ICommandExecuteResult> StopDiscordAppsAsync();
    }

    internal class DiscordAppService : IDiscordAppService
    {
        public List<IDiscordShardedClientHelper> DiscordShardedClientHelpers { get; } = new List<IDiscordShardedClientHelper>();

        private ILoggerService LoggerService { get; }

        private ISQLiteService SQLiteService { get; }

        private SemaphoreSlim DiscordShardedClientHelpersLock { get; } = new SemaphoreSlim(1, 1);

        public DiscordAppService(ILoggerService loggerService, ISQLiteService sqliteService)
        {
            LoggerService = loggerService;
            SQLiteService = sqliteService;
        }

        public async Task<ICommandExecuteResult> StartDiscordAppAsync(ulong clientId)
        {
            return await ExecuteCommand(async () =>
            {
                await StopDiscordAppAsync(clientId).ConfigureAwait(false);

                try
                {
                    var discordAppTable = await DiscordAppTable.GetRowAsync(SQLiteService, clientId).ConfigureAwait(false);

                    if (string.IsNullOrEmpty(discordAppTable?.BotToken))
                        return CommandExecuteResult.FromError($"Failed to start a discord app instance ({clientId}).");

                    var discordApp = await InternalStartDiscordApp(Convert.ToUInt64(discordAppTable.ClientId), discordAppTable.BotToken).ConfigureAwait(false);

                    if (discordApp.DiscordShardedClient.LoginState == LoginState.LoggedOut)
                        return CommandExecuteResult.FromError($"Failed to start a discord app instance ({clientId}).");

                    DiscordShardedClientHelpers.Add(discordApp);

                    return CommandExecuteResult.FromSuccess($"Successfully started a discord app instance ({clientId}).");
                }
                catch (Exception ex)
                {
                    await LoggerService.LogErrorMessageAsync(ex).ConfigureAwait(false);

                    return CommandExecuteResult.FromError($"Failed to start a discord app instance ({clientId}).");
                }
                finally
                {
                    DiscordShardedClientHelpersLock.Release();
                }
            }).ConfigureAwait(false);
        }

        public async Task<ICommandExecuteResult> StartDiscordAppsAsync()
        {
            return await ExecuteCommand(async () =>
            {
                if (DiscordShardedClientHelpers.Count > 0)
                    await StopDiscordAppsAsync().ConfigureAwait(false);

                try
                {
                    await DiscordShardedClientHelpersLock.WaitAsync().ConfigureAwait(false);

                    var discordAppTables = await DiscordAppTable.GetAllRowsAsync(SQLiteService).ConfigureAwait(false);

                    foreach (var discordAppTable in discordAppTables)
                    {
                        if (string.IsNullOrEmpty(discordAppTable.BotToken))
                            continue;

                        var discordApp = await InternalStartDiscordApp(Convert.ToUInt64(discordAppTable.ClientId), discordAppTable.BotToken).ConfigureAwait(false);

                        if (discordApp.DiscordShardedClient.LoginState == LoginState.LoggedOut)
                            return CommandExecuteResult.FromError($"Failed to start a discord app instance ({discordAppTable.ClientId}).");

                        DiscordShardedClientHelpers.Add(discordApp);
                    }

                    return CommandExecuteResult.FromSuccess("Successfully started all discord app instances.");
                }
                catch (Exception ex)
                {
                    await LoggerService.LogErrorMessageAsync(ex).ConfigureAwait(false);

                    return CommandExecuteResult.FromError("Failed to start all discord app instances.");
                }
                finally
                {
                    DiscordShardedClientHelpersLock.Release();
                }
            }).ConfigureAwait(false);
        }

        public async Task<ICommandExecuteResult> StopDiscordAppAsync(ulong clientId)
        {
            return await ExecuteCommand(async () =>
            {
                try
                {
                    await DiscordShardedClientHelpersLock.WaitAsync().ConfigureAwait(false);

                    for (var i = 0; i < DiscordShardedClientHelpers.Count; i++)
                    {
                        if (DiscordShardedClientHelpers[i].DiscordShardedClient.CurrentUser.Id != clientId)
                            continue;

                        await InternalStopDiscordApp(DiscordShardedClientHelpers[i]).ConfigureAwait(false);
                        DiscordShardedClientHelpers.Remove(DiscordShardedClientHelpers[i]);

                        break;
                    }

                    return CommandExecuteResult.FromSuccess($"Successfully stopped a discord app instance ({clientId}).");
                }
                catch (Exception ex)
                {
                    await LoggerService.LogErrorMessageAsync(ex).ConfigureAwait(false);

                    return CommandExecuteResult.FromError($"Failed to stop a discord app instance ({clientId}).");
                }
                finally
                {
                    DiscordShardedClientHelpersLock.Release();
                }
            }).ConfigureAwait(false);
        }

        public async Task<ICommandExecuteResult> StopDiscordAppsAsync()
        {
            return await ExecuteCommand(async () =>
            {
                try
                {
                    await DiscordShardedClientHelpersLock.WaitAsync().ConfigureAwait(false);

                    if (DiscordShardedClientHelpers.Count == 0)
                        return CommandExecuteResult.FromError("There are no discord app instances running.");

                    foreach (var discordShardedClientHelper in DiscordShardedClientHelpers)
                        await InternalStopDiscordApp(discordShardedClientHelper).ConfigureAwait(false);

                    DiscordShardedClientHelpers.Clear();

                    return CommandExecuteResult.FromSuccess("Successfully stopped all discord app instances.");
                }
                catch (Exception ex)
                {
                    await LoggerService.LogErrorMessageAsync(ex).ConfigureAwait(false);

                    return CommandExecuteResult.FromError("Failed to stop all discord app instances.");
                }
                finally
                {
                    DiscordShardedClientHelpersLock.Release();
                }
            }).ConfigureAwait(false);
        }

        private async Task<ICommandExecuteResult> ExecuteCommand(Func<Task<ICommandExecuteResult>> commandAction)
        {
            var result = await commandAction.Invoke().ConfigureAwait(false);

            await LoggerService.LogMessageAsync(result.Message, result.IsSuccess ? ConsoleColor.Green : ConsoleColor.Red).ConfigureAwait(false);

            return result;
        }

        private async Task<IDiscordShardedClientHelper> InternalStartDiscordApp(ulong clientId, string botToken)
        {
            var discordApp = new DiscordShardedClientHelper(clientId, botToken);
            AddListener(discordApp);

            await discordApp.StartListeningAsync().ConfigureAwait(false);

            if (discordApp.DiscordShardedClient.LoginState == LoginState.LoggedOut)
                await InternalStopDiscordApp(discordApp).ConfigureAwait(false);

            return discordApp;
        }

        private async Task InternalStopDiscordApp(IDiscordShardedClientHelper discordShardedClientHelper)
        {
            await discordShardedClientHelper.StopListeningAsync().ConfigureAwait(false);
            discordShardedClientHelper.Dispose();
            RemoveListener(discordShardedClientHelper);
        }

        private void AddListener(IDiscordShardedClientHelper discordShardedClientHelper)
        {
            discordShardedClientHelper.Log += DiscordShardedClientHelperOnLog;
        }

        private void RemoveListener(IDiscordShardedClientHelper discordShardedClientHelper)
        {
            discordShardedClientHelper.Log -= DiscordShardedClientHelperOnLog;
        }

        private Task DiscordShardedClientHelperOnLog(DiscordShardedClient discordShardedClient, LogMessage logMessage)
        {
            Task.Run(async () => await LoggerService.LogMessageAsync(discordShardedClient, logMessage).ConfigureAwait(false)).ConfigureAwait(false);

            return Task.CompletedTask;
        }
    }
}