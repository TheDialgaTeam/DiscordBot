using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public sealed class DiscordChannel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscordChannelId { get; set; }

        public ulong ChannelId { get; set; }

        public int DiscordGuildId { get; set; }
        public DiscordGuild DiscordGuild { get; set; }

        public List<DiscordAppChannel> DiscordAppChannels { get; set; }

        public List<DiscordChannelModerator> DiscordChannelModerators { get; set; }

        public FreeGameNotification FreeGameNotification { get; set; }

        public static async Task<List<DiscordChannel>> SyncTableAsync(SqliteContext sqliteContext, SocketGuild socketGuild, DiscordGuild discordGuild)
        {
            var discordChannels = await sqliteContext.DiscordChannelTable.Where(a => a.DiscordGuild.GuildId == socketGuild.Id).ToListAsync().ConfigureAwait(false);
            var discordChannelsToRemove = discordChannels.Where(a => !socketGuild.Channels.Where(b => b is SocketTextChannel).Select(b => b.Id).Contains(a.ChannelId)).AsEnumerable();

            sqliteContext.DiscordChannelTable.RemoveRange(discordChannelsToRemove);

            discordChannels.RemoveAll(a => !socketGuild.Channels.Where(b => b is SocketTextChannel).Select(b => b.Id).Contains(a.ChannelId));

            var discordChannelsToAdd = socketGuild.Channels.Where(a => a is SocketTextChannel).SkipWhile(a => discordChannels.Select(b => b.ChannelId).Contains(a.Id)).AsEnumerable();

            discordChannels.AddRange(discordChannelsToAdd.Select(socketGuildChannel => new DiscordChannel { ChannelId = socketGuildChannel.Id, DiscordGuildId = discordGuild.DiscordGuildId }));

            sqliteContext.DiscordChannelTable.UpdateRange(discordChannels);

            await sqliteContext.SaveChangesAsync().ConfigureAwait(false);

            return discordChannels;
        }
    }
}