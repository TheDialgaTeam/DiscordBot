using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;

namespace TheDialgaTeam.DiscordBot.Services.SQLite.Table
{
    public interface IDiscordAppOwnerTableService
    {
        Task<IDiscordAppOwnerModel[]> GetAllAsync(ulong? clientId = null);

        Task<IDiscordAppOwnerModel> SetAsync(ulong userId, ulong? clientId = null);

        Task<int> RemoveAsync(ulong userId, ulong? clientId = null);
    }

    internal sealed class DiscordAppOwnerTableService : SQLiteDatabaseTableService<DiscordAppOwnerModel>, IDiscordAppOwnerTableService
    {
        public DiscordAppOwnerTableService(ISQLiteService sqliteService) : base(sqliteService)
        {
        }

        public async Task<IDiscordAppOwnerModel[]> GetAllAsync(ulong? clientId = null)
        {
            var stringClientId = clientId?.ToString() ?? string.Empty;

            return await DataTable.Where(a => a.ClientId == stringClientId).ToArrayAsync();
        }

        public async Task<IDiscordAppOwnerModel> SetAsync(ulong userId, ulong? clientId = null)
        {
            var stringClientId = clientId?.ToString() ?? string.Empty;
            var stringUserId = userId.ToString();

            var discordAppOwnerModel = await DataTable.FirstOrDefaultAsync(a => a.ClientId == stringClientId && a.UserId == stringUserId);

            if (discordAppOwnerModel != null)
                return discordAppOwnerModel;

            discordAppOwnerModel = new DiscordAppOwnerModel { ClientId = stringClientId, UserId = stringUserId };
            await InsertAsync(discordAppOwnerModel);

            return discordAppOwnerModel;
        }

        public async Task<int> RemoveAsync(ulong userId, ulong? clientId = null)
        {
            var stringClientId = clientId?.ToString() ?? string.Empty;
            var stringUserId = userId.ToString();

            return await DataTable.DeleteAsync(a => a.ClientId == stringClientId && a.UserId == stringUserId);
        }
    }
}