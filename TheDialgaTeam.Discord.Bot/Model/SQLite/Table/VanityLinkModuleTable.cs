using SQLite;

namespace TheDialgaTeam.Discord.Bot.Model.SQLite.Table
{
    [Table("VanityLinkModule")]
    public sealed class VanityLinkModuleTable : IDatabaseTable
    {
        public long? Id { get; set; }

        public bool? Active { get; set; }

        public string VanityLink { get; set; }

        public string Password { get; set; }

        public bool? CheckDBans { get; set; }

        public bool? RejectInviteLinkUser { get; set; }

        public long? NewUserRateLimit { get; set; }

        public long? RejoinUserRateLimit { get; set; }

        public long? NewIPRateLimit { get; set; }

        public long? RejoinIPRateLimit { get; set; }

        public long? DiscordGuildId { get; set; }
    }
}