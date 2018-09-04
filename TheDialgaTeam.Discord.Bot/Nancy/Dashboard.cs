using System.Threading.Tasks;
using Nancy;
using TheDialgaTeam.Discord.Bot.Model.Nancy;
using TheDialgaTeam.Discord.Bot.Model.Nancy.Dashboard;
using TheDialgaTeam.Discord.Bot.Service.Discord;
using TheDialgaTeam.Discord.Bot.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Nancy
{
    public sealed class Dashboard : NancyModule
    {
        public Dashboard(DiscordAppService discordAppService, SQLiteService sqliteService)
        {
            Get("/getBotNickname/clientId/{clientId}/guildId/{guildId}", async args =>
            {
                string clientIdString = args["clientId"];
                string guildIdString = args["guildId"];

                if (!ulong.TryParse(clientIdString, out var clientId))
                    return Response.AsJson(new NancyResult { IsSuccess = false });

                if (!ulong.TryParse(guildIdString, out var guildId))
                    return Response.AsJson(new NancyResult { IsSuccess = false });

                var isSuccess = false;
                BotNickname result = null;

                await discordAppService.RequestDiscordAppInstanceAsync(discordAppInstances =>
                {
                    foreach (var discordAppInstance in discordAppInstances)
                    {
                        if (discordAppInstance.ClientId != clientId)
                            continue;

                        var guild = discordAppInstance.DiscordShardedClient.GetGuild(guildId);

                        if (guild == null)
                            break;

                        result = new BotNickname { Nickname = guild.CurrentUser.Nickname };
                        isSuccess = true;
                        break;
                    }

                    return Task.CompletedTask;
                }).ConfigureAwait(false);

                return !isSuccess ? Response.AsJson(new NancyResult { IsSuccess = false }) : Response.AsJson(result);
            });
        }
    }
}