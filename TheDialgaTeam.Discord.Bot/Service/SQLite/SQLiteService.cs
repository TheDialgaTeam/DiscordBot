using System;
using System.Reflection;
using System.Threading.Tasks;
using SQLite;
using TheDialgaTeam.Discord.Bot.Model.SQLite;
using TheDialgaTeam.Discord.Bot.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Service.IO;
using TheDialgaTeam.Discord.Bot.Service.Logger;

namespace TheDialgaTeam.Discord.Bot.Service.SQLite
{
    public sealed class SQLiteService
    {
        public SQLiteAsyncConnection SQLiteAsyncConnection { get; }

        private FilePathService FilePathService { get; }

        private LoggerService LoggerService { get; }

        public SQLiteService(FilePathService filePathService, LoggerService loggerService)
        {
            FilePathService = filePathService;
            LoggerService = loggerService;
            SQLiteAsyncConnection = new SQLiteAsyncConnection(filePathService.SQLiteDatabaseFilePath);
        }

        public async Task<long?> GetDiscordAppIdAsync(ulong clientId)
        {
            var clientIdString = clientId.ToString();
            var discordApp = await SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);
            return discordApp?.Id;
        }

        public async Task<long?> GetDiscordGuildIdAsync(ulong clientId, ulong guildId)
        {
            var discordAppId = await GetDiscordAppIdAsync(clientId).ConfigureAwait(false);
            var guildIdString = guildId.ToString();
            var discordGuild = await SQLiteAsyncConnection.Table<DiscordGuildTable>().Where(a => a.DiscordAppId == discordAppId && a.GuildId == guildIdString).FirstOrDefaultAsync().ConfigureAwait(false);
            return discordGuild?.Id;
        }

        public async Task<long?> GetDiscordChannelIdAsync(ulong clientId, ulong guildId, ulong channelId)
        {
            var discordGuildId = await GetDiscordGuildIdAsync(clientId, guildId).ConfigureAwait(false);
            var channelIdString = channelId.ToString();
            var discordChannel = await SQLiteAsyncConnection.Table<DiscordChannelTable>().Where(a => a.DiscordGuildId == discordGuildId && a.ChannelId == channelIdString).FirstOrDefaultAsync().ConfigureAwait(false);
            return discordChannel?.Id;
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

                foreach (var assemblyType in assemblyTypes)
                {
                    if (!assemblyType.IsClass || !typeof(IDatabaseTable).IsAssignableFrom(assemblyType))
                        continue;

                    await SQLiteAsyncConnection.CreateTableAsync(assemblyType).ConfigureAwait(false);
                }

                await SQLiteAsyncConnection.ExecuteAsync("VACUUM;").ConfigureAwait(false);
                await LoggerService.LogMessageAsync($"Database created at: {FilePathService.SQLiteDatabaseFilePath}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await LoggerService.LogErrorMessageAsync(ex).ConfigureAwait(false);
            }
        }
    }
}