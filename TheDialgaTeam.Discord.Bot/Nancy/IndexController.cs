using System.Threading.Tasks;
using Nancy;
using TheDialgaTeam.Discord.Bot.Model.Nancy;
using TheDialgaTeam.Discord.Bot.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Service.Discord;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Nancy
{
    public sealed class IndexController : NancyModule
    {
        public IndexController(SQLiteService sqliteService, DiscordAppService discordAppService)
        {
            Get("/getDiscordAppTable", async args =>
            {
                var discordAppTables = await sqliteService.SQLiteAsyncConnection.Table<DiscordAppTable>().ToArrayAsync().ConfigureAwait(false);
                return Response.AsJson(discordAppTables);
            });

            Get("/getDiscordAppTable/clientId/{clientId}", async args =>
            {
                string clientId = args["clientId"];
                var discordAppTables = await sqliteService.SQLiteAsyncConnection.Table<DiscordAppTable>().Where(a => a.ClientId == clientId).ToArrayAsync().ConfigureAwait(false);
                return Response.AsJson(discordAppTables);
            });

            Get("/checkBotExist/clientId/{clientId}/guildId/{guildId}", async args =>
            {
                string clientId = args["clientId"];
                string guildId = args["guildId"];

                var exists = false;

                await discordAppService.RequestDiscordAppInstanceAsync(discordAppInstances =>
                {
                    foreach (var discordAppInstance in discordAppInstances)
                    {
                        if (discordAppInstance.ClientId.ToString() != clientId)
                            continue;

                        foreach (var socketGuild in discordAppInstance.DiscordShardedClient.Guilds)
                        {
                            if (socketGuild.Id.ToString() != guildId)
                                continue;

                            exists = true;
                            return Task.CompletedTask;
                        }
                    }

                    return Task.CompletedTask;
                });

                return Response.AsJson(new NancyResult { IsSuccess = exists });
            });
        }
    }
}