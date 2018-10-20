using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.DependencyInjection.ProgramLoop;
using TheDialgaTeam.Discord.Bot.Models.Discord;
using TheDialgaTeam.Discord.Bot.Models.Discord.Command;
using TheDialgaTeam.Discord.Bot.Services.Console;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Services.Discord
{
    public sealed class DiscordAppService : IInitializableAsync, ITickableAsync, IDisposableAsync
    {
        public SynchronizedCollection<DiscordAppInstance> DiscordAppInstances { get; } = new SynchronizedCollection<DiscordAppInstance>();

        private LoggerService LoggerService { get; }

        private SqliteDatabaseService SqliteDatabaseService { get; }

        private Program Program { get; }

        public DiscordAppService(LoggerService loggerService, SqliteDatabaseService sqliteDatabaseService, Program program)
        {
            LoggerService = loggerService;
            SqliteDatabaseService = sqliteDatabaseService;
            Program = program;
        }

        public async Task InitializeAsync()
        {
            using (var context = SqliteDatabaseService.GetContext(true))
            {
                await context.DiscordAppTable.ForEachAsync(async a =>
                {
                    var result = await StartDiscordAppAsync(a.ClientId).ConfigureAwait(false);
                    LoggerService.LogMessage(result.BuildDiscordTextResponse(), ConsoleColor.Green);
                }).ConfigureAwait(false);
            }

            LoggerService.LogMessage("All discord apps have been started.", ConsoleColor.Green);
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

                    using (var context = SqliteDatabaseService.GetContext())
                    {
                        var discordApp = await context.DiscordAppTable.Where(a => a.ClientId == discordAppInstance.ClientId).FirstOrDefaultAsync().ConfigureAwait(false);

                        if (discordAppInstance.ClientId != discordAppInstance.DiscordShardedClient.CurrentUser.Id)
                        {
                            LoggerService.LogMessage(discordAppInstance.DiscordShardedClient, new LogMessage(LogSeverity.Error, "", "Discord App client id mismatch found! Forced application to stop!"));
                            await StopDiscordAppAsync(discordAppInstance.ClientId).ConfigureAwait(false);

                            context.DiscordAppTable.Remove(discordApp);
                            await context.SaveChangesAsync().ConfigureAwait(false);

                            continue;
                        }

                        var discordAppInstanceInfo = await discordAppInstance.DiscordShardedClient.GetApplicationInfoAsync().ConfigureAwait(false);

                        discordApp.AppName = discordAppInstanceInfo.Name;
                        discordApp.AppDescription = discordAppInstanceInfo.Description;
                        discordApp.LastUpdateCheck = DateTimeOffset.Now;

                        context.DiscordAppTable.Update(discordApp);
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }

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

            using (var context = SqliteDatabaseService.GetContext(true))
            {
                var discordApp = await context.DiscordAppTable.Where(a => a.ClientId == clientId).FirstOrDefaultAsync();

                if (discordApp == null)
                    return CommandExecuteResult.FromError("Discord App is not registered in the database.");

                var discordAppInstance = new DiscordAppInstance(discordApp.ClientId, discordApp.BotToken);

                AddListener(discordAppInstance);

                try
                {
                    await discordAppInstance.DiscordAppLoginAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await discordAppInstance.DiscordAppLogoutAsync().ConfigureAwait(false);
                    LoggerService.LogErrorMessage(ex);
                }

                if (!discordAppInstance.IsLoggedIn)
                    return CommandExecuteResult.FromError("Discord App encountered an error while trying to authenticate.");

                try
                {
                    await discordAppInstance.DiscordAppStartAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LoggerService.LogErrorMessage(ex);
                }

                if (!discordAppInstance.IsStarted)
                {
                    RemoveListener(discordAppInstance);
                    return CommandExecuteResult.FromError("Discord App encountered an error while trying to connect to the discord gateway server.");
                }

                DiscordAppInstances.Add(discordAppInstance);

                return CommandExecuteResult.FromSuccess("Discord App is now starting!");
            }
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
            discordAppInstance.GuildAvailable += DiscordAppInstanceOnGuildAvailable;
            discordAppInstance.JoinedGuild += DiscordAppInstanceOnJoinedGuild;
            discordAppInstance.LeftGuild += DiscordAppInstanceOnLeftGuild;
            discordAppInstance.ChannelCreated += DiscordAppInstanceOnChannelCreated;
            discordAppInstance.ChannelDestroyed += DiscordAppInstanceOnChannelDestroyed;
        }

        private void RemoveListener(DiscordAppInstance discordAppInstance)
        {
            discordAppInstance.ChannelDestroyed -= DiscordAppInstanceOnChannelDestroyed;
            discordAppInstance.ChannelCreated -= DiscordAppInstanceOnChannelCreated;
            discordAppInstance.LeftGuild -= DiscordAppInstanceOnLeftGuild;
            discordAppInstance.JoinedGuild -= DiscordAppInstanceOnJoinedGuild;
            discordAppInstance.GuildAvailable -= DiscordAppInstanceOnGuildAvailable;
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
                    using (var databaseContext = SqliteDatabaseService.GetContext(true))
                    {
                        var discordGuild = (await databaseContext.DiscordAppTable.Where(a => a.ClientId == context.Client.CurrentUser.Id)
                                                                 .Select(a => new
                                                                 {
                                                                     discordGuild = a.DiscordGuilds.FirstOrDefault(b => b.GuildId == context.Guild.Id)
                                                                 }).ToListAsync().ConfigureAwait(false))
                                           .Select(a => a.discordGuild).FirstOrDefault();

                        if (discordGuild == null && !socketUserMessage.HasMentionPrefix(discordAppInstance.DiscordShardedClient.CurrentUser, ref argPos))
                            return;

                        if (!socketUserMessage.HasMentionPrefix(discordAppInstance.DiscordShardedClient.CurrentUser, ref argPos) &&
                            !socketUserMessage.HasStringPrefix(discordGuild?.Prefix ?? "", ref argPos, StringComparison.OrdinalIgnoreCase))
                            return;
                    }
                }

                await Program.CommandService.ExecuteAsync(context, argPos, Program.ServiceProvider).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private Task DiscordAppInstanceOnGuildAvailable(DiscordAppInstance discordAppInstance, SocketGuild socketGuild)
        {
            Task.Run(async () => await SqliteDatabaseService.RebuildGuildChannelTableAsync(discordAppInstance, socketGuild).ConfigureAwait(false)).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private Task DiscordAppInstanceOnJoinedGuild(DiscordAppInstance discordAppInstance, SocketGuild socketGuild)
        {
            Task.Run(async () => await SqliteDatabaseService.RebuildGuildChannelTableAsync(discordAppInstance, socketGuild).ConfigureAwait(false)).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private Task DiscordAppInstanceOnLeftGuild(DiscordAppInstance discordAppInstance, SocketGuild socketGuild)
        {
            Task.Run(async () => await SqliteDatabaseService.RemoveGuildChannelAsync(discordAppInstance, socketGuild).ConfigureAwait(false)).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private Task DiscordAppInstanceOnChannelCreated(DiscordAppInstance discordAppInstance, SocketChannel socketChannel)
        {
            if (socketChannel is SocketTextChannel socketTextChannel)
                Task.Run(async () => await SqliteDatabaseService.RebuildGuildChannelTableAsync(discordAppInstance, socketTextChannel.Guild).ConfigureAwait(false)).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private Task DiscordAppInstanceOnChannelDestroyed(DiscordAppInstance discordAppInstance, SocketChannel socketChannel)
        {
            if (socketChannel is SocketTextChannel socketTextChannel)
                Task.Run(async () => await SqliteDatabaseService.RebuildGuildChannelTableAsync(discordAppInstance, socketTextChannel.Guild).ConfigureAwait(false)).ConfigureAwait(false);

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