using System.Reflection;
using System.Threading.Tasks;
using SQLite;
using TheDialgaTeam.DependencyInjection.ProgramLoop;
using TheDialgaTeam.Discord.Bot.Old.Model.SQLite;
using TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Services.Console;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Old.Service.SQLite
{
    public sealed class SQLiteService : IInitializableAsync
    {
        public SQLiteAsyncConnection SQLiteAsyncConnection { get; private set; }

        private FilePathService FilePathService { get; }

        private LoggerService LoggerService { get; }

        public SQLiteService(FilePathService filePathService, LoggerService loggerService)
        {
            FilePathService = filePathService;
            LoggerService = loggerService;
        }

        public async Task InitializeAsync()
        {
            SQLiteAsyncConnection = new SQLiteAsyncConnection(FilePathService.SQLiteDatabaseFilePath);

            var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var assemblyType in assemblyTypes)
            {
                if (!assemblyType.IsClass || assemblyType.IsAbstract || !typeof(IDatabaseTable).IsAssignableFrom(assemblyType))
                    continue;

                await SQLiteAsyncConnection.CreateTableAsync(assemblyType).ConfigureAwait(false);
            }

            await SQLiteAsyncConnection.ExecuteAsync("VACUUM;").ConfigureAwait(false);
            LoggerService.LogMessage($"Database created at: {FilePathService.SQLiteDatabaseFilePath}\n");
        }

        public async Task<long?> GetDiscordAppIdAsync(ulong clientId)
        {
            var clientIdString = clientId.ToString();
            var discordApp = await SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);
            return discordApp?.Id;
        }

        public async Task<long?> GetDiscordGuildIdAsync(ulong clientId, ulong guildId)
        {
            var discordGuild = await GetOrCreateDiscordGuildTableAsync(clientId, guildId).ConfigureAwait(false);
            return discordGuild.Id;
        }

        public async Task<long?> GetDiscordChannelIdAsync(ulong clientId, ulong guildId, ulong channelId)
        {
            var discordChannel = await GetOrCreateDiscordChannelTableAsync(clientId, guildId, channelId).ConfigureAwait(false);
            return discordChannel.Id;
        }

        public async Task<DiscordGuildTable> GetOrCreateDiscordGuildTableAsync(ulong clientId, ulong guildId)
        {
            var discordAppId = await GetDiscordAppIdAsync(clientId).ConfigureAwait(false);
            var guildIdString = guildId.ToString();
            var discordGuild = await SQLiteAsyncConnection.Table<DiscordGuildTable>().Where(a => a.DiscordAppId == discordAppId && a.GuildId == guildIdString).FirstOrDefaultAsync().ConfigureAwait(false);

            if (discordGuild == null)
                await SQLiteAsyncConnection.InsertAsync(new DiscordGuildTable { GuildId = guildIdString, DiscordAppId = discordAppId.Value }).ConfigureAwait(false);

            return await SQLiteAsyncConnection.Table<DiscordGuildTable>().Where(a => a.DiscordAppId == discordAppId && a.GuildId == guildIdString).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<DiscordChannelTable> GetOrCreateDiscordChannelTableAsync(ulong clientId, ulong guildId, ulong channelId)
        {
            var discordGuildId = await GetDiscordGuildIdAsync(clientId, guildId).ConfigureAwait(false);
            var channelIdString = channelId.ToString();
            var discordChannel = await SQLiteAsyncConnection.Table<DiscordChannelTable>().Where(a => a.DiscordGuildId == discordGuildId && a.ChannelId == channelIdString).FirstOrDefaultAsync().ConfigureAwait(false);

            if (discordChannel == null)
                await SQLiteAsyncConnection.InsertAsync(new DiscordChannelTable { ChannelId = channelIdString, DiscordGuildId = discordGuildId.Value }).ConfigureAwait(false);

            return await SQLiteAsyncConnection.Table<DiscordChannelTable>().Where(a => a.DiscordGuildId == discordGuildId && a.ChannelId == channelIdString).FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}