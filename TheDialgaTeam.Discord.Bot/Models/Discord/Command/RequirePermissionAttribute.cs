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
                    var guildUser = await context.Guild.GetUserAsync(context.Message.Author.Id).ConfigureAwait(false);

                    var currentUser = (await databaseContext.DiscordAppTable.Where(a => a.ClientId == context.Client.CurrentUser.Id)
                                                            .Select(a => new
                                                            {
                                                                discordGuild = a.DiscordGuilds.Where(b => b.GuildId == context.Guild.Id)
                                                                                .Select(b => new
                                                                                {
                                                                                    discordChannel = b.DiscordChannels.Where(c => c.ChannelId == context.Channel.Id)
                                                                                                      .Select(c => new
                                                                                                      {
                                                                                                          discordChannelModerator = c.DiscordChannelModerators.Count(d => d.Type == DiscordChannelModeratorType.Role && guildUser.RoleIds.Contains(d.Value) ||
                                                                                                                                                                          d.Type == DiscordChannelModeratorType.User && d.Value == context.User.Id)
                                                                                                      }),
                                                                                    discordGuildModerator = b.DiscordGuildModerators.Count(c => c.Type == DiscordGuildModeratorType.Role && guildUser.RoleIds.Contains(c.Value) ||
                                                                                                                                                c.Type == DiscordGuildModeratorType.User && c.Value == context.User.Id)
                                                                                })
                                                            }).ToListAsync().ConfigureAwait(false))
                                      .Select(a => new
                                      {
                                          isAChannelModerator = a.discordGuild.FirstOrDefault()?.discordChannel.FirstOrDefault()?.discordChannelModerator > 0,
                                          isAGuildModerator = a.discordGuild.FirstOrDefault()?.discordGuildModerator > 0
                                      }).FirstOrDefault();

                    // Channel Moderator
                    if (currentUser?.isAChannelModerator ?? false)
                        currentUserPermission = RequiredPermission.ChannelModerator;

                    // Guild Moderator
                    if (currentUser?.isAGuildModerator ?? false)
                        currentUserPermission = RequiredPermission.GuildModerator;

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
                    var isAnOwner = (await databaseContext.DiscordAppTable.Where(a => a.ClientId == context.Client.CurrentUser.Id)
                                                          .Select(a => new
                                                          {
                                                              discordAppOwner = a.DiscordAppOwners.Count(b => b.UserId == context.User.Id)
                                                          }).ToListAsync().ConfigureAwait(false))
                                    .Select(a => a.discordAppOwner > 0).FirstOrDefault();

                    if (isAnOwner)
                        currentUserPermission = RequiredPermission.DiscordAppOwner;
                }

                // Global Discord App Owner
                var isAGlobalOwner = await databaseContext.DiscordAppOwnerTable.Where(a => a.DiscordAppId == null && a.UserId == context.User.Id).CountAsync().ConfigureAwait(false) > 0;

                if (isAGlobalOwner)
                    currentUserPermission = RequiredPermission.GlobalDiscordAppOwner;
            }

            return currentUserPermission >= RequiredPermission ? PreconditionResult.FromSuccess() : PreconditionResult.FromError($"This command require {RequiredPermission.ToString()} permission and above.");
        }
    }
}