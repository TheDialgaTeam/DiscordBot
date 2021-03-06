﻿//using System;
//using System.Threading.Tasks;
//using Discord.Commands;
//using Discord.WebSocket;
//using Microsoft.Extensions.DependencyInjection;
//using TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table;
//using TheDialgaTeam.Discord.Bot.Old.Service.SQLite;

//namespace TheDialgaTeam.Discord.Bot.Old.Model.Discord.Command
//{
//    public sealed class RequireActiveModuleAttribute : PreconditionAttribute
//    {
//        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
//        {
//            if (context.Message.Channel is SocketDMChannel || context.Message.Channel is SocketGroupChannel)
//                return PreconditionResult.FromError("This command requires to be in Guild context.");

//            var moduleName = string.Empty;

//            foreach (var attribute in command.Module.Attributes)
//            {
//                if (!(attribute is SubscribableModuleAttribute moduleAttribute))
//                    continue;

//                moduleName = moduleAttribute.ModuleName;
//                break;
//            }

//            var sqliteService = services.GetRequiredService<SQLiteService>();

//            var discordGuildId = await sqliteService.GetDiscordGuildIdAsync(context.Client.CurrentUser.DiscordAppId, context.Guild.DiscordAppId).ConfigureAwait(false);
//            var discordGuildModule = await sqliteService.SQLiteAsyncConnection.Table<DiscordGuildModuleTable>().Where(a => a.DiscordGuildId == discordGuildId && a.Module == moduleName).FirstOrDefaultAsync().ConfigureAwait(false);

//            if (discordGuildModule == null)
//                return PreconditionResult.FromError("Missing Discord Guild Module record from the database.");

//            return discordGuildModule.Active ?? false ? PreconditionResult.FromSuccess() : PreconditionResult.FromError($"This command require {moduleName} to be active in this guild.");
//        }
//    }
//}

