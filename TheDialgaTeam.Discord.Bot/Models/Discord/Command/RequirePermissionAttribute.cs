using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Discord.Bot.Models.EntityFramework;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Models.Discord.Command
{
    public enum RequiredPermission
    {
        GlobalDiscordAppOwner = 5,

        DiscordAppOwner = 4,

        GuildAdministrator = 3,

        GuildModerator = 2,

        ChannelModerator = 1,

        GuildMember = 0
    }

    public sealed class RequirePermissionAttribute : PreconditionAttribute
    {
        public RequiredPermission RequiredPermission { get; }

        public RequirePermissionAttribute(RequiredPermission requiredPermission)
        {
            RequiredPermission = requiredPermission;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var sqliteDatabaseService = services.GetRequiredService<SqliteDatabaseService>();

            var currentUserPermission = RequiredPermission.GuildMember;

            using (var databaseContext = sqliteDatabaseService.GetContext(true))
            {
                if (context.Message.Channel is SocketGuildChannel)
                {
                    //var discordChannelTable = await databaseContext.GetDiscordChannelTableAsync(context.Client.CurrentUser.Id, context.Guild.Id, context.Channel.Id, DiscordChannelTableIncludedEntities.DiscordGuildModeratorTable | DiscordChannelTableIncludedEntities.DiscordChannelModeratorTable).ConfigureAwait(false);

                    //var guildUser = await context.Guild.GetUserAsync(context.Message.Author.Id).ConfigureAwait(false);

                    //if (discordChannelTable != null)
                    //{
                    //    // Channel Moderator
                    //    if (discordChannelTable.DiscordChannelModerators.Any(a => a.Type == DiscordChannelModeratorType.Role && guildUser.RoleIds.Contains(a.Value) ||
                    //                                                              a.Type == DiscordChannelModeratorType.User && a.Value == context.User.Id))
                    //        currentUserPermission = RequiredPermission.ChannelModerator;

                    //    // Guild Moderator
                    //    if (discordChannelTable.DiscordGuild.DiscordGuildModerators.Any(a => a.Type == DiscordGuildModeratorType.Role && guildUser.RoleIds.Contains(a.Value) ||
                    //                                                                         a.Type == DiscordGuildModeratorType.User && a.Value == context.User.Id))
                    //        currentUserPermission = RequiredPermission.GuildModerator;

                    //    // Guild Administrator
                    //    if (guildUser.GuildPermissions.Administrator)
                    //        currentUserPermission = RequiredPermission.GuildAdministrator;
                    //}
                }

                // Discord App Owner
                var botOwner = (await context.Client.GetApplicationInfoAsync()).Owner;

                if (context.Message.Author.Id == botOwner.Id)
                    currentUserPermission = RequiredPermission.DiscordAppOwner;
                else
                {
                    //var isAnOwner = Enumerable.FirstOrDefault((await databaseContext.DiscordAppTable.Where(a => a.ClientId == context.Client.CurrentUser.Id)
                    //        .Select(a => new
                    //        {
                    //            discordAppOwner = a.DiscordAppOwners.Any(b => b.UserId == context.User.Id)
                    //        }).ToListAsync().ConfigureAwait(false))
                    //    .Select(a => a.discordAppOwner));

                    //if (isAnOwner)
                    //    currentUserPermission = RequiredPermission.DiscordAppOwner;
                }

                // Global Discord App Owner
                var isAGlobalOwner = await databaseContext.DiscordAppOwnerTable.Where(a => a.DiscordAppId == null && a.UserId == context.User.Id).AnyAsync().ConfigureAwait(false);

                if (isAGlobalOwner)
                    currentUserPermission = RequiredPermission.GlobalDiscordAppOwner;
            }

            return currentUserPermission >= RequiredPermission ? PreconditionResult.FromSuccess() : PreconditionResult.FromError($"This command require {RequiredPermission.ToString()} permission and above.");
        }
    }
}