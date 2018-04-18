using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;

namespace TheDialgaTeam.DiscordBot.Services.SQLite.Table
{
    public interface IDiscordAppTableService
    {
        Task<IDiscordAppModel> GetAsync(ulong clientId);

        Task<IDiscordAppModel[]> GetAllAsync();

        Task<IDiscordAppModel> SetAsync(ulong clientId, string clientSecret, string botToken);

        Task<int> RemoveAsync(ulong clientId);
    }

    internal sealed class DiscordAppTableService : SQLiteDatabaseTableService<DiscordAppModel>, IDiscordAppTableService
    {
        public DiscordAppTableService(ISQLiteService sqliteService) : base(sqliteService)
        {
        }

        public async Task<IDiscordAppModel> GetAsync(ulong clientId)
        {
            var stringClientId = clientId.ToString();

            return await DataTable.FirstOrDefaultAsync(a => a.ClientId == stringClientId);
        }

        public async Task<IDiscordAppModel[]> GetAllAsync()
        {
            return await DataTable.ToArrayAsync();
        }

        public async Task<IDiscordAppModel> SetAsync(ulong clientId, string clientSecret, string botToken)
        {
            var stringClientId = clientId.ToString();

            var discordAppModel = await DataTable.FirstOrDefaultAsync(a => a.ClientId == stringClientId);

            if (discordAppModel != null)
            {
                discordAppModel.ClientSecret = clientSecret;
                discordAppModel.BotToken = botToken;

                await UpdateAsync(discordAppModel);

                return discordAppModel;
            }

            discordAppModel = new DiscordAppModel { ClientId = stringClientId, ClientSecret = clientSecret, BotToken = botToken };
            await InsertAsync(discordAppModel);

            return discordAppModel;
        }

        public async Task<int> RemoveAsync(ulong clientId)
        {
            var stringClientId = clientId.ToString();

            return await DataTable.DeleteAsync(a => a.ClientId == stringClientId);
        }
    }
}