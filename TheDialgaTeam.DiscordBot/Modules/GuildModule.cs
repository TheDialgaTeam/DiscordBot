using Discord.Commands;
using System;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.Command;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Modules
{
    [Name("Guild")]
    [RequireContext(ContextType.Guild)]
    public sealed class GuildModule : ModuleHelper
    {
        private CommandService CommandService { get; }

        private ISQLiteService SQLiteService { get; }

        public GuildModule(IProgram program, ISQLiteService sqliteService)
        {
            CommandService = program.CommandService;
            SQLiteService = sqliteService;
        }

        [Command("ShowCommandPrefix")]
        [Summary("Show the current command prefix for this guild.")]
        public async Task ShowCommandPrefixAsync()
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var discordGuildModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (discordGuildModel == null)
                await ReplyAsync(":negative_squared_cross_mark: There are no command prefix set for this guild!");
            else
                await ReplyAsync($"The current command prefix is `{discordGuildModel.CharPrefix}`.");
        }

        [Command("SetCommandPrefix")]
        [Summary("Set the current command prefix for this guild.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        public async Task SetCommandPrefixAsync([Remainder] [Summary("Custom prefix to use when using this bot commands.")] string prefix)
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            var discordGuildModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId).FirstOrDefaultAsync();

            if (discordGuildModel == null)
                await SQLiteService.SQLiteAsyncConnection.InsertAsync(new DiscordGuildModel { ClientId = clientId, GuildId = guildId, CharPrefix = prefix });
            else
            {
                discordGuildModel.CharPrefix = prefix;
                await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordGuildModel);
            }

            await ReplyAsync($":white_check_mark: Successfully changed the prefix to `{prefix}`.");
        }

        [Command("SubscribeModule")]
        [Summary("Subscribe the bot specific modules.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        public async Task SubscribeModuleAsync([Remainder] [Summary("Module to subscribe.")] string module)
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            foreach (var commandServiceModule in CommandService.Modules)
            {
                if (commandServiceModule.Preconditions.Count == 0)
                    continue;

                foreach (var preconditionAttribute in commandServiceModule.Preconditions)
                {
                    if (!(preconditionAttribute is RequireActiveModuleAttribute))
                        continue;

                    if (commandServiceModule.Name.Equals(module, StringComparison.OrdinalIgnoreCase))
                    {
                        var discordGuildModuleModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId && a.Module == commandServiceModule.Name).FirstOrDefaultAsync();

                        if (discordGuildModuleModel == null)
                        {
                            await SQLiteService.SQLiteAsyncConnection.InsertAsync(new DiscordGuildModuleModel { ClientId = clientId, GuildId = guildId, Module = commandServiceModule.Name, Active = true });
                            await ReplyAsync($":white_check_mark: Successfully subscribe to {commandServiceModule.Name} Module.");
                            return;
                        }

                        discordGuildModuleModel.Active = true;
                        await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordGuildModuleModel);
                        await ReplyAsync($":white_check_mark: Successfully subscribe to {commandServiceModule.Name} Module.");
                        return;
                    }
                }
            }

            await ReplyAsync($":negative_squared_cross_mark: Unable to find `{module}` to subscribe.");
        }

        [Command("UnsubscribeModule")]
        [Summary("Unsubscribe the bot specific modules.")]
        [RequirePermission(RequiredPermissions.GuildAdministrator)]
        public async Task UnsubscribeModuleAsync([Remainder] [Summary("Module to unsubscribe.")] string module)
        {
            var clientId = Context.Client.CurrentUser.Id.ToString();
            var guildId = Context.Guild.Id.ToString();

            foreach (var commandServiceModule in CommandService.Modules)
            {
                if (commandServiceModule.Preconditions.Count == 0)
                    continue;

                foreach (var preconditionAttribute in commandServiceModule.Preconditions)
                {
                    if (!(preconditionAttribute is RequireActiveModuleAttribute))
                        continue;

                    if (commandServiceModule.Name.Equals(module, StringComparison.OrdinalIgnoreCase))
                    {
                        var discordGuildModuleModel = await SQLiteService.SQLiteAsyncConnection.Table<DiscordGuildModuleModel>().Where(a => a.ClientId == clientId && a.GuildId == guildId && a.Module == commandServiceModule.Name).FirstOrDefaultAsync();

                        if (discordGuildModuleModel == null)
                        {
                            await SQLiteService.SQLiteAsyncConnection.InsertAsync(new DiscordGuildModuleModel { ClientId = clientId, GuildId = guildId, Module = commandServiceModule.Name, Active = false });
                            await ReplyAsync($":white_check_mark: Successfully unsubscribe to {commandServiceModule.Name} Module.");
                            return;
                        }

                        discordGuildModuleModel.Active = false;
                        await SQLiteService.SQLiteAsyncConnection.UpdateAsync(discordGuildModuleModel);
                        await ReplyAsync($":white_check_mark: Successfully unsubscribe to {commandServiceModule.Name} Module.");
                        return;
                    }
                }
            }

            await ReplyAsync($":negative_squared_cross_mark: Unable to find `{module}` to unsubscribe.");
        }
    }
}