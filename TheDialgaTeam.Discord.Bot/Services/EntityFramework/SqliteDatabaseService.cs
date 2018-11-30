using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.DependencyInjection;
using TheDialgaTeam.Discord.Bot.Models.Discord;
using TheDialgaTeam.Discord.Bot.Models.EntityFramework;
using TheDialgaTeam.Discord.Bot.Services.Console;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Services.EntityFramework
{
    public sealed class SqliteDatabaseService : IInitializableAsync
    {
        private FilePathService FilePathService { get; }

        private LoggerService LoggerService { get; }

        public SqliteDatabaseService(FilePathService filePathService, LoggerService loggerService)
        {
            FilePathService = filePathService;
            LoggerService = loggerService;
        }

        public async Task InitializeAsync()
        {
            using (var context = GetContext(true))
                await context.Database.MigrateAsync().ConfigureAwait(false);
        }

        public SqliteContext GetContext(bool readOnly = false)
        {
            return new SqliteContext(FilePathService, readOnly);
        }

        public async Task RebuildGuildChannelTableAsync(DiscordAppInstance discordAppInstance, SocketGuild socketGuild)
        {
            try
            {
                using (var context = GetContext())
                {
                    /*
                     * Sync Database with the discord servers.
                     *
                     * DiscordGuildTable - Global Guild Settings
                     * DiscordChannelTable - Global Channel Settings
                     * DiscordAppGuildTable - Local Guild Settings
                     * DiscordAppChannelTable - Local Channel Settings
                     */

                    // Sync Global Settings
                    var discordGuild = await DiscordGuild.SyncTableAsync(context, socketGuild).ConfigureAwait(false);
                    var discordChannels = await DiscordChannel.SyncTableAsync(context, socketGuild, discordGuild).ConfigureAwait(false);

                    await LoggerService.LogMessageAsync(discordAppInstance.DiscordShardedClient, new LogMessage(LogSeverity.Info, "", $"{socketGuild} have been synced into the database.")).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await LoggerService.LogErrorMessageAsync(ex).ConfigureAwait(false);
            }
        }

        public async Task RemoveGuildChannelAsync(DiscordAppInstance discordAppInstance, SocketGuild socketGuild)
        {
            using (var context = GetContext())
            {
                //var discordGuild = await context.GetDiscordGuildTableAsync(discordAppInstance.ClientId, socketGuild.Id).ConfigureAwait(false);

                //if (discordGuild != null)
                //    context.Remove(discordGuild);

                //await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}