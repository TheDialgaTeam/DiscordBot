using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Discord.Bot.Old.Model.SQLite.Table;
using TheDialgaTeam.Discord.Bot.Old.Service.SQLite;

namespace TheDialgaTeam.Discord.Bot.Old.Model.Discord.Command
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
            var sqliteService = services.GetRequiredService<SQLiteService>();

            var currentUserPermission = RequiredPermission.GuildMember;

            if (context.Message.Channel is SocketGuildChannel)
            {
                var discordGuildId = await sqliteService.GetDiscordGuildIdAsync(context.Client.CurrentUser.Id, context.Guild.Id).ConfigureAwait(false);
                var discordChannelId = await sqliteService.GetDiscordChannelIdAsync(context.Client.CurrentUser.Id, context.Guild.Id, context.Channel.Id).ConfigureAwait(false);
                var guildUser = await context.Guild.GetUserAsync(context.Message.Author.Id).ConfigureAwait(false);

                // Channel Moderator
                var discordChannelModeratorRole = await sqliteService.SQLiteAsyncConnection.Table<DiscordChannelModeratorRoleTable>().Where(a => a.DiscordChannelId == discordChannelId).ToArrayAsync().ConfigureAwait(false);
                var discordChannelmoderatorUser = await sqliteService.SQLiteAsyncConnection.Table<DiscordChannelModeratorUserTable>().Where(a => a.DiscordChannelId == discordChannelId).ToArrayAsync().ConfigureAwait(false);

                foreach (var discordChannelModeratorRoleTable in discordChannelModeratorRole)
                {
                    foreach (var guildUserRoleId in guildUser.RoleIds)
                    {
                        if (guildUserRoleId.ToString() != discordChannelModeratorRoleTable.RoleId)
                            continue;

                        currentUserPermission = RequiredPermission.ChannelModerator;
                        break;
                    }

                    if (currentUserPermission == RequiredPermission.ChannelModerator)
                        break;
                }

                if (currentUserPermission != RequiredPermission.ChannelModerator)
                {
                    foreach (var discordChannelModeratorUserTable in discordChannelmoderatorUser)
                    {
                        if (context.Message.Author.Id.ToString() != discordChannelModeratorUserTable.UserId)
                            continue;

                        currentUserPermission = RequiredPermission.ChannelModerator;
                        break;
                    }
                }

                // Guild Moderator
                var discordGuildModeratorRole = await sqliteService.SQLiteAsyncConnection.Table<DiscordGuildModeratorRoleTable>().Where(a => a.DiscordGuildId == discordGuildId).ToArrayAsync().ConfigureAwait(false);
                var discordGuildModeratorUser = await sqliteService.SQLiteAsyncConnection.Table<DiscordGuildModeratorUserTable>().Where(a => a.DiscordGuildId == discordGuildId).ToArrayAsync().ConfigureAwait(false);

                foreach (var discordGuildModeratorRoleTable in discordGuildModeratorRole)
                {
                    foreach (var guildUserRoleId in guildUser.RoleIds)
                    {
                        if (guildUserRoleId.ToString() != discordGuildModeratorRoleTable.RoleId)
                            continue;

                        currentUserPermission = RequiredPermission.GuildModerator;
                        break;
                    }

                    if (currentUserPermission == RequiredPermission.GuildModerator)
                        break;
                }

                if (currentUserPermission != RequiredPermission.GuildModerator)
                {
                    foreach (var discordGuildModeratorUserTable in discordGuildModeratorUser)
                    {
                        if (context.Message.Author.Id.ToString() != discordGuildModeratorUserTable.UserId)
                            continue;

                        currentUserPermission = RequiredPermission.GuildModerator;
                        break;
                    }
                }

                // Guild Administrator
                if (guildUser.GuildPermissions.Administrator)
                    currentUserPermission = RequiredPermission.GuildAdministrator;
            }

            // Discord App Owner
            var botOwner = (await context.Client.GetApplicationInfoAsync()).Owner;

            if (context.Message.Author.Id == botOwner.Id)
                currentUserPermission = RequiredPermission.DiscordAppOwner;
            else
            {
                var discordAppId = await sqliteService.GetDiscordAppIdAsync(context.Client.CurrentUser.Id).ConfigureAwait(false);
                var discordAppOwner = await sqliteService.SQLiteAsyncConnection.Table<DiscordAppOwnerTable>().Where(a => a.DiscordAppId == discordAppId).ToArrayAsync().ConfigureAwait(false);

                foreach (var discordAppOwnerTable in discordAppOwner)
                {
                    if (discordAppOwnerTable.UserId != context.Message.Author.Id.ToString())
                        continue;

                    currentUserPermission = RequiredPermission.DiscordAppOwner;
                    break;
                }
            }

            // Global Discord App Owner
            var discordAppOwner2 = await sqliteService.SQLiteAsyncConnection.Table<DiscordAppOwnerTable>().Where(a => a.DiscordAppId == null).ToArrayAsync().ConfigureAwait(false);

            foreach (var discordAppOwnerTable in discordAppOwner2)
            {
                if (discordAppOwnerTable.UserId != context.Message.Author.Id.ToString())
                    continue;

                currentUserPermission = RequiredPermission.GlobalDiscordAppOwner;
                break;
            }

            return currentUserPermission >= RequiredPermission ? PreconditionResult.FromSuccess() : PreconditionResult.FromError($"This command require {RequiredPermission.ToString()} permission and above.");
        }
    }
}