using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using TheDialgaTeam.DiscordBot.Model.Discord.User.Enum;
using TheDialgaTeam.DiscordBot.Model.SQLite.Table;
using TheDialgaTeam.DiscordBot.Services;

namespace TheDialgaTeam.DiscordBot.Model.Discord
{
    public interface IDiscordUser
    {
        UserPermissions UserPermission { get; }

        Task InitializeUser();
    }

    internal sealed class DiscordUser : IDiscordUser
    {
        public UserPermissions UserPermission { get; private set; } = UserPermissions.GuildMember;

        private ICommandContext CommandContext { get; }

        private IDiscordSocketClientModel DiscordSocketClientModel { get; }

        private ISQLiteService SQLiteService { get; }

        public DiscordUser(ICommandContext commandContext, IDiscordSocketClientModel discordSocketClientModel, ISQLiteService sqliteService)
        {
            CommandContext = commandContext;
            DiscordSocketClientModel = discordSocketClientModel;
            SQLiteService = sqliteService;
        }

        public async Task InitializeUser()
        {
            if (CommandContext.Message.Channel.GetType() != typeof(SocketDMChannel))
            {
                var user = await CommandContext.Guild.GetUserAsync(CommandContext.Message.Author.Id);

                // Channel Moderator

                // Guild Moderator

                // Guild Administrator
                if (user.GuildPermissions.Administrator)
                    UserPermission = UserPermissions.GuildAdministrator;
            }

            // Discord App Owner
            var botOwner = (await CommandContext.Client.GetApplicationInfoAsync()).Owner;

            if (CommandContext.Message.Author.Id == botOwner.Id)
                UserPermission = UserPermissions.DiscordAppOwner;
            else
            {
                foreach (var discordAppOwnerModel in DiscordSocketClientModel.DiscordTableModel.DiscordAppOwnerModels)
                {
                    if (CommandContext.Message.Author.Id.ToString() != discordAppOwnerModel.UserId)
                        continue;

                    UserPermission = UserPermissions.DiscordAppOwner;
                    break;
                }
            }

            // Global Discord App Owner
            var discordAppOwnerModels = await SQLiteService.SQLiteAsyncConnection.Table<DiscordAppOwnerModel>().Where(a => a.ClientId == "").ToArrayAsync();

            foreach (var discordAppOwnerModel in discordAppOwnerModels)
            {
                if (CommandContext.Message.Author.Id.ToString() != discordAppOwnerModel.UserId)
                    continue;

                UserPermission = UserPermissions.GlobalDiscordAppOwner;
                break;
            }
        }
    }
}