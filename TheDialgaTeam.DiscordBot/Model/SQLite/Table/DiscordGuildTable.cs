using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    [Table("DiscordGuild")]
    internal sealed class DiscordGuildTable : BaseTable, IDatabaseTable
    {
        public string GuildId { get; set; }

        public string Prefix { get; set; }

        [ForeignKey(typeof(DiscordAppDetailTable))]
        public long DiscordAppDetailId { get; set; }

        [ForeignKey(typeof(ServerHoundTable))]
        public long ServerHoundId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public DiscordAppDetailTable DiscordAppDetail { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<DiscordGuildModeratorRoleTable> DiscordGuildModeratorRole { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<DiscordGuildModeratorUserTable> DiscordGuildModeratorUser { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<DiscordGuildModuleTable> DiscordGuildModule { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<DiscordChannelTable> DiscordChannel { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ServerHoundTable ServerHound { get; set; }
    }
}