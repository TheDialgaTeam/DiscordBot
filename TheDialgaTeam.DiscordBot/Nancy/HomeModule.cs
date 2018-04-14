using Nancy;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Nancy
{
    public sealed class HomeModule : NancyModule
    {
        public HomeModule(ISQLiteService sqliteService)
        {
            Get("/discordAppModel", async args =>
            {
                var discordAppModels = await sqliteService.SQLiteAsyncConnection.Table<DiscordAppModel>().OrderBy(a => a.Id).ToArrayAsync();
                return Response.AsJson(discordAppModels);
            });
        }
    }
}