using System;
using System.Threading.Tasks;
using SQLite;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("DiscordApp")]
    internal sealed class DiscordAppTable : IDatabaseTable
    {
        [PrimaryKey]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AppName { get; set; }

        public string AppDescription { get; set; }

        public string BotToken { get; set; }

        public DateTimeOffset LastUpdateCheck { get; set; }

        public static async Task<DiscordAppTable[]> GetAllRowsAsync(ISQLiteService sqliteService)
        {
            return await sqliteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);
        }

        public static async Task<DiscordAppTable> GetRowAsync(ISQLiteService sqliteService, ulong clientId)
        {
            var clientIdString = clientId.ToString();

            return await sqliteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientIdString).FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}