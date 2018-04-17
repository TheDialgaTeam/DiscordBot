using SQLite;
using System;
using TheDialgaTeam.DiscordBot.Model.SQLite.Interface;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table.Modules
{
    public interface IPollModel : IBaseTable, IClientId, IGuildId, IChannelId, IMessageId
    {
        DateTimeOffset StartDateTime { get; set; }

        TimeSpan Duration { get; set; }
    }

    [Table("PollModuleModels")]
    internal sealed class PollModel : BaseTable, IPollModel
    {
        public string ClientId { get; set; }

        public string GuildId { get; set; }

        public string ChannelId { get; set; }

        public string MessageId { get; set; }

        public DateTimeOffset StartDateTime { get; set; }

        public TimeSpan Duration { get; set; }
    }
}