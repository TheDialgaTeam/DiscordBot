using SQLite;

namespace TheDialgaTeam.DiscordBot.Model.SQLite.Table
{
    public interface IBotInstanceModel
    {
        int Id { get; }

        string BotId { get; }

        string BotToken { get; set; }
    }

    [Table("BotInstances")]
    internal class BotInstanceModel : IBotInstanceModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string BotId { get; set; }

        [Unique]
        public string BotToken { get; set; }
    }
}