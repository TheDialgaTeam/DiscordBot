using Nancy;
using System;
using System.Collections.Generic;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Nancy
{
    public sealed class HomeModule : NancyModule
    {
        public HomeModule(ISQLiteService sqliteService)
        {
            Get("/discordAppModel", async args =>
            {
                var where = Where(Request, "id", "clientId", "clientSecret", "appName", "botToken", "verified");

                var discordAppModels = await sqliteService.SQLiteAsyncConnection.QueryAsync<DiscordAppModel>($"select * from DiscordApps {where.Item1} order by Id", where.Item2);

                return Response.AsJson(discordAppModels);
            });

            Get("/discordAppModel", async args =>
            {
                var where = Where(Request, "id", "clientId", "clientSecret", "appName", "botToken", "verified");

                var discordAppModels = await sqliteService.SQLiteAsyncConnection.QueryAsync<DiscordAppModel>($"select * from DiscordApps {where.Item1} order by Id", where.Item2);

                return Response.AsJson(discordAppModels);
            });

            Get("discordAppOwnerModel", async args =>
            {
                var where = Where(Request, "id", "clientId", "userId");

                var discordAppOwnerModels = await sqliteService.SQLiteAsyncConnection.QueryAsync<DiscordAppOwnerModel>($"select * from DiscordAppOwners {where.Item1} order by Id", where.Item2);

                return Response.AsJson(discordAppOwnerModels);
            });

            Get("/discordChannelModeratorModel", async args =>
            {
                var where = Where(Request, "id", "clientId", "channelId", "roleId", "userId");

                var discordChannelModeratorModels = await sqliteService.SQLiteAsyncConnection.QueryAsync<DiscordChannelModeratorModel>($"select * from DiscordChannelModerators {where.Item1} order by Id", where.Item2);

                return Response.AsJson(discordChannelModeratorModels);
            });

            Get("/discordGuildModel", async args =>
            {
                var where = Where(Request, "id", "clientId", "guildId", "charPrefix");

                var discordGuildModels = await sqliteService.SQLiteAsyncConnection.QueryAsync<DiscordGuildModel>($"select * from DiscordGuilds {where.Item1} order by Id", where.Item2);

                return Response.AsJson(discordGuildModels);
            });

            Get("/discordGuildModeratorModel", async args =>
            {
                var where = Where(Request, "id", "clientId", "guildId", "roldId", "userId");

                var discordGuildModeratorModels = await sqliteService.SQLiteAsyncConnection.QueryAsync<DiscordGuildModeratorModel>($"select * from DiscordGuildModerators {where.Item1} order by Id", where.Item2);

                return Response.AsJson(discordGuildModeratorModels);
            });

            Get("/discordGuildModuleModel", async args =>
            {
                var where = Where(Request, "id", "clientId", "guildId", "module", "active");

                var discordGuildModuleModels = await sqliteService.SQLiteAsyncConnection.QueryAsync<DiscordGuildModuleModel>($"select * from DiscordGuildModules {where.Item1} order by Id", where.Item2);

                return Response.AsJson(discordGuildModuleModels);
            });

            Get("/freeGameNotificationModel", async args =>
            {
                var where = Where(Request, "id", "clientId", "guildId", "roleId", "channelId");

                var freeGameNotificationModels = await sqliteService.SQLiteAsyncConnection.QueryAsync<FreeGameNotificationModel>($"select * from FreeGameNotification {where.Item1} order by Id", where.Item2);

                return Response.AsJson(freeGameNotificationModels);
            });

            Get("/serverHoundModel", async args =>
            {
                var where = Where(Request, "id", "clientId", "guildId", "dBans");

                var serverHoundModels = await sqliteService.SQLiteAsyncConnection.QueryAsync<ServerHoundModel>($"select * from ServerHound {where.Item1} order by Id", where.Item2);

                return Response.AsJson(serverHoundModels);
            });
        }

        private static Tuple<string, object[]> Where(Request request, params string[] columns)
        {
            var whereQuery = new List<string>();
            var whereValue = new List<object>();

            foreach (var column in columns)
            {
                if (request.Query[column] == null)
                    continue;

                whereQuery.Add($"{column} = ?");
                whereValue.Add(request.Query[column].ToString());
            }

            return new Tuple<string, object[]>($"{(whereValue.Count == 0 ? "" : $"where {string.Join(" and ", whereQuery)}")}", whereValue.ToArray());
        }
    }
}