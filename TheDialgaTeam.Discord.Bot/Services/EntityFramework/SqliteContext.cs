using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.Discord.Bot.Models.EntityFramework;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Services.EntityFramework
{
    public sealed class SqliteContext : DbContext
    {
        public DbSet<DiscordAppTable> DiscordAppTable { get; set; }

        public DbSet<DiscordAppOwnerTable> DiscordAppOwnerTable { get; set; }

        public DbSet<DiscordGuildTable> DiscordGuildTable { get; set; }

        public DbSet<DiscordGuildModeratorTable> DiscordGuildModeratorTable { get; set; }

        public DbSet<DiscordGuildModuleTable> DiscordGuildModuleTable { get; set; }

        public DbSet<DiscordChannelTable> DiscordChannelTable { get; set; }

        public DbSet<DiscordChannelModeratorTable> DiscordChannelModeratorTable { get; set; }

        private FilePathService FilePathService { get; }

        public SqliteContext()
        {
        }

        public SqliteContext(FilePathService filePathService)
        {
            FilePathService = filePathService;
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(FilePathService == null ? "Data Source=Application.db" : $"Data Source={FilePathService.SQLiteDatabaseFilePath}");
        }
    }
}