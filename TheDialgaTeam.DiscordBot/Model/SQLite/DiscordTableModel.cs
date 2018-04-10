using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Model.SQLite
{
    public interface IDiscordTableModel
    {
        IDiscordAppModel DiscordAppModel { get; }

        IDiscordAppOwnerModel[] DiscordAppOwnerModels { get; }

        IDiscordChannelModeratorModel[] DiscordChannelModeratorModels { get; }

        IDiscordGuildModel[] DiscordGuildModels { get; }

        IDiscordGuildModeratorModel[] DiscordGuildModeratorModels { get; }

        IDiscordGuildModuleModel[] DiscordGuildModuleModels { get; }
    }

    internal sealed class DiscordTableModel : IDiscordTableModel
    {
        public IDiscordAppModel DiscordAppModel { get; private set; }

        public IDiscordAppOwnerModel[] DiscordAppOwnerModels { get; private set; }

        public IDiscordChannelModeratorModel[] DiscordChannelModeratorModels { get; private set; }

        public IDiscordGuildModel[] DiscordGuildModels { get; private set; }

        public IDiscordGuildModeratorModel[] DiscordGuildModeratorModels { get; private set; }

        public IDiscordGuildModuleModel[] DiscordGuildModuleModels { get; private set; }

        private ISQLiteService SQLiteService { get; }

        private string ClientId { get; }

        public DiscordTableModel(ISQLiteService sqliteService, string clientId)
        {
            SQLiteService = sqliteService;
            ClientId = clientId;
        }

        public async Task InitializeDiscordTable()
        {
            DiscordAppModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppModel>().Where(a => a.ClientId == ClientId).FirstOrDefaultAsync();
            DiscordAppOwnerModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppOwnerModel>().Where(a => a.ClientId == ClientId).ToArrayAsync();
            DiscordChannelModeratorModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordChannelModeratorModel>().Where(a => a.ClientId == ClientId).ToArrayAsync();
            DiscordGuildModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().Where(a => a.ClientId == ClientId).ToArrayAsync();
            DiscordGuildModeratorModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModeratorModel>().Where(a => a.ClientId == ClientId).ToArrayAsync();
            DiscordGuildModuleModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().Where(a => a.ClientId == ClientId).ToArrayAsync();
        }
    }
}