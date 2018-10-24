//using Nancy;
//using TheDialgaTeam.Discord.Bot.Old.Model.Nancy;
//using TheDialgaTeam.Discord.Bot.Old.Service.Discord;

//namespace TheDialgaTeam.Discord.Bot.Old.Nancy
//{
//    public sealed class DiscordAppAuthenticationHandler : NancyModule
//    {
//        public DiscordAppAuthenticationHandler(DiscordAppService discordAppService)
//        {
//            Get("/checkBotExist/clientId/{clientId}/guildId/{guildId}", async args =>
//            {
//                string clientId = args["clientId"];
//                string guildId = args["guildId"];

//                var exists = false;

//                //await discordAppService.RequestDiscordAppInstanceAsync(discordAppInstances =>
//                //{
//                //    foreach (var discordAppInstance in discordAppInstances)
//                //    {
//                //        if (discordAppInstance.ClientId.ToString() != clientId)
//                //            continue;

//                //        foreach (var socketGuild in discordAppInstance.DiscordShardedClient.Guilds)
//                //        {
//                //            if (socketGuild.Id.ToString() != guildId)
//                //                continue;

//                //            exists = true;
//                //            return Task.CompletedTask;
//                //        }
//                //    }

//                //    return Task.CompletedTask;
//                //});

//                return Response.AsJson(new NancyResult { IsSuccess = exists });
//            });
//        }
//    }
//}

