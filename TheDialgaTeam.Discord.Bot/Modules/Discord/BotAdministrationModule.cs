//using System.Linq;
//using System.Threading.Tasks;
//using Discord;
//using Discord.Commands;
//using Microsoft.EntityFrameworkCore;
//using TheDialgaTeam.Discord.Bot.Models.Discord.Command;
//using TheDialgaTeam.Discord.Bot.Models.EntityFramework;
//using TheDialgaTeam.Discord.Bot.Services.Discord;
//using TheDialgaTeam.Discord.Bot.Services.EntityFramework;

//namespace TheDialgaTeam.Discord.Bot.Modules.Discord
//{
//    [Name("Bot Administration")]
//    [RequirePermission(RequiredPermission.GlobalDiscordAppOwner)]
//    public sealed class BotAdministrationModule : ModuleHelper
//    {
//        private DiscordAppService DiscordAppService { get; }

//        public BotAdministrationModule(DiscordAppService discordAppService, SqliteDatabaseService sqliteDatabaseService) : base(sqliteDatabaseService)
//        {
//            DiscordAppService = discordAppService;
//        }

//        [Command("AddDiscordApp")]
//        [Summary("Add a new bot instance.")]
//        public async Task AddDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId, [Summary("Discord bot client secret.")]
//            string clientSecret, [Summary("Bot secret token.")] string botToken)
//        {
//            using (var context = SqliteDatabaseService.GetContext())
//            {
//                var discordApp = await context.GetDiscordAppTableAsync(clientId).ConfigureAwait(false);

//                if (discordApp == null)
//                {
//                    discordApp = new DiscordApp { ClientId = clientId, ClientSecret = clientSecret, BotToken = botToken };

//                    context.DiscordAppTable.Add(discordApp);
//                }
//                else
//                {
//                    discordApp.ClientSecret = clientSecret;
//                    discordApp.BotToken = botToken;

//                    context.DiscordAppTable.Update(discordApp);
//                }

//                await context.SaveChangesAsync().ConfigureAwait(false);

//                await ReplyAsync(CommandExecuteResult.FromSuccess("Discord App have been added.").BuildDiscordTextResponse()).ConfigureAwait(false);
//            }
//        }

//        [Command("RemoveDiscordApp")]
//        [Summary("Remove a bot instance.")]
//        public async Task RemoveDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId)
//        {
//            if (Context.Client.CurrentUser.Id == clientId)
//            {
//                await ReplyAsync(CommandExecuteResult.FromError("You are not allowed to delete this bot!").BuildDiscordTextResponse()).ConfigureAwait(false);
//                return;
//            }

//            using (var context = SqliteDatabaseService.GetContext())
//            {
//                var discordApp = await context.GetDiscordAppTableAsync(clientId).ConfigureAwait(false);

//                if (discordApp == null)
//                {
//                    await ReplyAsync(CommandExecuteResult.FromError("Could not find any Discord App with this ID!").BuildDiscordTextResponse()).ConfigureAwait(false);
//                    return;
//                }

//                await DiscordAppService.StopDiscordAppAsync(clientId).ConfigureAwait(false);

//                context.DiscordAppTable.Remove(discordApp);
//                await context.SaveChangesAsync().ConfigureAwait(false);

//                await ReplyAsync(CommandExecuteResult.FromSuccess("Discord App have been deleted.").BuildDiscordTextResponse()).ConfigureAwait(false);
//            }
//        }

//        [Command("StartDiscordApp")]
//        [Summary("Start a new bot instance.")]
//        public async Task StartDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId)
//        {
//            var result = await DiscordAppService.StartDiscordAppAsync(clientId).ConfigureAwait(false);
//            await ReplyAsync(result.BuildDiscordTextResponse()).ConfigureAwait(false);
//        }

//        [Command("StopDiscordApp")]
//        [Summary("Stop a bot instance.")]
//        public async Task StopDiscordAppAsync([Summary("Discord bot client id.")] ulong clientId)
//        {
//            var result = await DiscordAppService.StopDiscordAppAsync(clientId).ConfigureAwait(false);
//            await ReplyAsync(result.BuildDiscordTextResponse()).ConfigureAwait(false);
//        }

//        [Command("AddGlobalAdmin")]
//        [Summary("Add a global administrator.")]
//        public async Task AddGlobalAdminAsync([Summary("User to add.")] IUser user)
//        {
//            if (user == null)
//            {
//                await ReplyAsync(CommandExecuteResult.FromError("Invalid user.").BuildDiscordTextResponse()).ConfigureAwait(false);
//                return;
//            }

//            using (var context = SqliteDatabaseService.GetContext())
//            {
//                var discordAppOwner = await context.DiscordAppOwnerTable.Where(a => a.DiscordAppId == null && a.UserId == user.Id).FirstOrDefaultAsync().ConfigureAwait(false);

//                if (discordAppOwner == null)
//                {
//                    discordAppOwner = new DiscordAppOwner { UserId = user.Id };

//                    context.DiscordAppOwnerTable.Add(discordAppOwner);
//                    await context.SaveChangesAsync().ConfigureAwait(false);

//                    await ReplyAsync(CommandExecuteResult.FromSuccess($"Successfully added {user} as a global admin.").BuildDiscordTextResponse()).ConfigureAwait(false);
//                }
//                else
//                    await ReplyAsync(CommandExecuteResult.FromError($"{user} is already a global admin.").BuildDiscordTextResponse()).ConfigureAwait(false);
//            }
//        }

//        [Command("RemoveGlobalAdmin")]
//        [Summary("Remove a global administrator.")]
//        public async Task RemoveGlobalAdminAsync([Summary("User to remove.")] IUser user)
//        {
//            if (user == null)
//            {
//                await ReplyAsync(CommandExecuteResult.FromError("Invalid user.").BuildDiscordTextResponse()).ConfigureAwait(false);
//                return;
//            }

//            using (var context = SqliteDatabaseService.GetContext())
//            {
//                var discordAppOwner = await context.DiscordAppOwnerTable.Where(a => a.DiscordAppId == null && a.UserId == user.Id).FirstOrDefaultAsync().ConfigureAwait(false);

//                if (discordAppOwner == null)
//                    await ReplyAsync(CommandExecuteResult.FromError($"{user} is not a global admin.").BuildDiscordTextResponse()).ConfigureAwait(false);
//                else
//                {
//                    context.DiscordAppOwnerTable.Remove(discordAppOwner);
//                    await context.SaveChangesAsync().ConfigureAwait(false);

//                    await ReplyAsync(CommandExecuteResult.FromSuccess($"Successfully removed {user} as a global admin.").BuildDiscordTextResponse()).ConfigureAwait(false);
//                }
//            }
//        }
//    }
//}