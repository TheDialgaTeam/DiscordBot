using Nancy;
using TheDialgaTeam.Discord.Bot.Model.Nancy;
using TheDialgaTeam.Discord.Bot.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Nancy
{
    public sealed class BaseModule : NancyModule
    {
        public BaseModule(SQLiteService sqliteService)
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

            Get("/getDiscordAppOwnerTable", async args =>
            {
                var discordAppOwnerTables = await sqliteService.SQLiteAsyncConnection.Table<DiscordAppOwnerTable>().Where(a => a.DiscordAppId == null).ToArrayAsync().ConfigureAwait(false);
                return Response.AsJson(discordAppOwnerTables);
            });

            Get("/getDiscordAppOwnerTable/clientId/{clientId}", async args =>
            {
                string clientIdString = args["clientId"];

                if (!ulong.TryParse(clientIdString, out var clientId))
                    return Response.AsJson(new NancyResult { IsSuccess = false });

                var discordAppId = await sqliteService.GetDiscordAppIdAsync(clientId).ConfigureAwait(false);
                var discordAppOwnerTables = await sqliteService.SQLiteAsyncConnection.Table<DiscordAppOwnerTable>().Where(a => a.DiscordAppId == discordAppId).ToArrayAsync().ConfigureAwait(false);
                return Response.AsJson(discordAppOwnerTables);
            });

            Get("/getDiscordGuildTable/clientId/{clientId}/guildId/{guildId}", async args =>
            {
                string clientIdString = args["clientId"];
                string guildIdString = args["guildId"];

                if (!ulong.TryParse(clientIdString, out var clientId))
                    return Response.AsJson(new NancyResult { IsSuccess = false });

                if (!ulong.TryParse(guildIdString, out var guildId))
                    return Response.AsJson(new NancyResult { IsSuccess = false });

                var discordGuildTable = await sqliteService.GetOrCreateDiscordGuildTableAsync(clientId, guildId).ConfigureAwait(false);
                return Response.AsJson(discordGuildTable);
            });

            Get("/getDiscordGuildModeratorRole/clientId/{clientId}/guildId/{guildId}", async args =>
            {
                string clientIdString = args["clientId"];
                string guildIdString = args["guildId"];

                if (!ulong.TryParse(clientIdString, out var clientId))
                    return Response.AsJson(new NancyResult { IsSuccess = false });

                if (!ulong.TryParse(guildIdString, out var guildId))
                    return Response.AsJson(new NancyResult { IsSuccess = false });

                var discordGuildId = await sqliteService.GetDiscordGuildIdAsync(clientId, guildId).ConfigureAwait(false);
                var discordGuildModeratorRole = await sqliteService.SQLiteAsyncConnection.Table<DiscordGuildModeratorRoleTable>().Where(a => a.DiscordGuildId == discordGuildId).ToArrayAsync().ConfigureAwait(false);

                return Response.AsJson(discordGuildModeratorRole);
            });
        }
    }
}