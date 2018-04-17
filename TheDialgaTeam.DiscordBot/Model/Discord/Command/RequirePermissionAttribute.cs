using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services.SQLite;

namespace TheDialgaTeam.DiscordBot.Model.Discord.Command
{
    internal sealed class RequirePermissionAttribute : PreconditionAttribute
    {
        public RequiredPermissions RequiredPermission { get; }

        public RequirePermissionAttribute(RequiredPermissions requiredPermission)
        {
            RequiredPermission = requiredPermission;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var sqliteService = services.GetRequiredService<ISQLiteService>();

            var currentUserPermission = RequiredPermissions.GuildMember;
            var clientId = context.Client.CurrentUser.Id.ToString();

            if (context.Message.Channel is SocketGuildChannel channel)
            {
                var channelId = context.Message.Channel.Id.ToString();
                var guildId = channel.Guild.Id.ToString();
                var guildUser = await context.Guild.GetUserAsync(context.Message.Author.Id);

                // Channel Moderator
                var discordChannelModeratorModels = await sqliteService.SQLiteAsyncConnection.Table<DiscordChannelModeratorModel>().Where(a => a.ClientId == clientId && a.ChannelId == channelId).ToArrayAsync();

                foreach (var discordChannelModeratorModel in discordChannelModeratorModels)
                {
                    if (!string.IsNullOrEmpty(discordChannelModeratorModel.RoleId))
                    {
                        foreach (var roleId in guildUser.RoleIds)
                        {
                            if (roleId.ToString() != discordChannelModeratorModel.RoleId)
                                continue;

                            currentUserPermission = RequiredPermissions.ChannelModerator;
                            break;
                        }

                        if (currentUserPermission == RequiredPermissions.ChannelModerator)
                            break;
                    }
                    else if (!string.IsNullOrEmpty(discordChannelModeratorModel.UserId))
                    {
                        if (context.Message.Author.Id.ToString() != discordChannelModeratorModel.UserId)
                            continue;

                        currentUserPermission = RequiredPermissions.ChannelModerator;
                        break;
                    }
                }

                // Guild Moderator
                var discordGuildModeratorModels = await sqliteService.SQLiteAsyncConnection.Table<DiscordGuildModeratorModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).ToArrayAsync();

                foreach (var discordGuildModeratorModel in discordGuildModeratorModels)
                {
                    if (!string.IsNullOrEmpty(discordGuildModeratorModel.RoleId))
                    {
                        foreach (var roleId in guildUser.RoleIds)
                        {
                            if (roleId.ToString() != discordGuildModeratorModel.RoleId)
                                continue;

                            currentUserPermission = RequiredPermissions.GuildModerator;
                            break;
                        }

                        if (currentUserPermission == RequiredPermissions.GuildModerator)
                            break;
                    }
                    else if (!string.IsNullOrEmpty(discordGuildModeratorModel.UserId))
                    {
                        if (context.Message.Author.Id.ToString() != discordGuildModeratorModel.UserId)
                            continue;

                        currentUserPermission = RequiredPermissions.GuildModerator;
                        break;
                    }
                }

                // Guild Administrator
                if (guildUser.GuildPermissions.Administrator)
                    currentUserPermission = RequiredPermissions.GuildAdministrator;
            }

            // Discord App Owner
            var botOwner = (await context.Client.GetApplicationInfoAsync()).Owner;

            if (context.Message.Author.Id == botOwner.Id)
                currentUserPermission = RequiredPermissions.DiscordAppOwner;
            else
            {
                var discordAppOwnerModels = await sqliteService.SQLiteAsyncConnection.Table<DiscordAppOwnerModel>().Where(a => a.ClientId == clientId).ToArrayAsync();

                foreach (var discordAppOwnerModel in discordAppOwnerModels)
                {
                    if (discordAppOwnerModel.UserId != context.Message.Author.Id.ToString())
                        continue;

                    currentUserPermission = RequiredPermissions.DiscordAppOwner;
                    break;
                }
            }

            // Global Discord App Owner
            var discordAppOwnerModels2 = await sqliteService.SQLiteAsyncConnection.Table<DiscordAppOwnerModel>().Where(a => a.ClientId == "").ToArrayAsync();

            foreach (var discordAppOwnerModel in discordAppOwnerModels2)
            {
                if (discordAppOwnerModel.UserId != context.Message.Author.Id.ToString())
                    continue;

                currentUserPermission = RequiredPermissions.GlobalDiscordAppOwner;
                break;
            }

            return currentUserPermission >= RequiredPermission ? PreconditionResult.FromSuccess() : PreconditionResult.FromError($"This command require {RequiredPermission.ToString()} permission and above.");
        }
    }

    public enum RequiredPermissions
    {
        GlobalDiscordAppOwner = 5,
        DiscordAppOwner = 4,
        GuildAdministrator = 3,
        GuildModerator = 2,
        ChannelModerator = 1,
        GuildMember = 0
    }
}