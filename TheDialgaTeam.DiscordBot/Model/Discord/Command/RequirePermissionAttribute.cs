using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.User.Enum;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Model.Discord.Command
{
    internal sealed class RequirePermissionAttribute : PreconditionAttribute
    {
        public UserPermissions UserPermissions { get; }

        public RequirePermissionAttribute(UserPermissions userPermissions)
        {
            UserPermissions = userPermissions;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var discordAppService = services.GetRequiredService<IDiscordAppService>();
            var sqliteService = services.GetRequiredService<ISQLiteService>();

            foreach (var discordSocketClientModel in discordAppService.DiscordSocketClientModels)
            {
                if (discordSocketClientModel.DiscordSocketClient != context.Client)
                    continue;

                var discordUser = new DiscordUser(context, discordSocketClientModel, sqliteService);
                await discordUser.InitializeUser();

                return discordUser.UserPermission >= UserPermissions ? PreconditionResult.FromSuccess() : PreconditionResult.FromError($"This command require {UserPermissions.ToString()} permission and above.");
            }

            return PreconditionResult.FromError($"This command require {UserPermissions.ToString()} permission and above.");
        }
    }
}