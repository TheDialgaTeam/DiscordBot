using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordChannel")]
    internal sealed class DiscordChannelTable : BaseTable, IDatabaseTable
    {
        public string ChannelId { get; set; }

        [ForeignKey(typeof(DiscordGuildTable))]
        public long DiscordGuildId { get; set; }

        [ForeignKey(typeof(FreeGameNotificationTable))]
        public long FreeGameNotificationId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordGuildTable DiscordGuild { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<DiscordChannelModeratorRoleTable> DiscordChannelModeratorRole { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<DiscordChannelModeratorUserTable> DiscordChannelModeratorUser { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public FreeGameNotificationTable FreeGameNotification { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PollTable> Poll { get; set; }
    }
}