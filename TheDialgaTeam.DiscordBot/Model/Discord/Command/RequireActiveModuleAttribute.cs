using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Model.Discord.Command
{
    internal sealed class RequireActiveModuleAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var sqliteService = services.GetRequiredService<ISQLiteService>();

            var clientId = context.Client.CurrentUser.Id.ToString();
            var guildId = context.Guild.Id.ToString();
            var moduleName = command.Module.Name;

            var discordGuildModuleModel = await sqliteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId && a.Module == moduleName).FirstOrDefaultAsync();

            return discordGuildModuleModel?.Active ?? false ? PreconditionResult.FromSuccess() : PreconditionResult.FromError($"This command require {command.Module.Name} to be active in this guild.");
        }
    }
}