namespace TheDialgaTeam.Discord.Bot.Models.EntityFramework
{
    public enum DiscordChannelModeratorType
    {
        Role = 0,

        User = 1
    }

    public sealed class DiscordChannelModeratorTable
    {
        public ulong? Id { get; set; }

        public DiscordChannelModeratorType Type { get; set; }

        public ulong? Value { get; set; }

        public ulong? DiscordChannelId { get; set; }

        public DiscordChannelTable DiscordChannel { get; set; }
    }
}