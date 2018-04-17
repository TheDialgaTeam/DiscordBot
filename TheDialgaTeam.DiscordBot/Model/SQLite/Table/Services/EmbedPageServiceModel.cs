using SQLite;
using TheDialgaTeam.DiscordBot.Model.SQLite.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table.Services
{
    public interface IEmbedPageServiceModel : IBaseTable, IClientId, IGuildId, IChannelId, IMessageId
    {
    }

    [Table("EmbedPageServiceModels")]
    internal sealed class EmbedPageServiceModel : BaseTable, IEmbedPageServiceModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string ChannelId { get; set; }

        public string MessageId { get; set; }
    }
}