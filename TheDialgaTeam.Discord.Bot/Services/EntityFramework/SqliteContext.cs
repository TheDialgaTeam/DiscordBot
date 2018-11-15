using Microsoft.EntityFrameworkCore;
using TheDialgaTeam.Discord.Bot.Models.EntityFramework;
using TheDialgaTeam.Discord.Bot.Services.IO;

namespace TheDialgaTeam.Discord.Bot.Services.EntityFramework
{
    public sealed class SqliteContext : DbContext
    {
        public DbSet<DiscordApp> DiscordAppTable { get; set; }

        public DbSet<DiscordAppOwner> DiscordAppOwnerTable { get; set; }

        public DbSet<DiscordAppModule> DiscordAppModuleTable { get; set; }

        public DbSet<DiscordGuild> DiscordGuildTable { get; set; }

        public DbSet<DiscordGuildModerator> DiscordGuildModeratorTable { get; set; }

        public DbSet<DiscordGuildModule> DiscordGuildModuleTable { get; set; }

        public DbSet<DiscordChannel> DiscordChannelTable { get; set; }

        public DbSet<DiscordChannelModerator> DiscordChannelModeratorTable { get; set; }

        public DbSet<DiscordModule> DiscordModuleTable { get; set; }

        public DbSet<DiscordModuleRequirement> DiscordModuleRequirementTable { get; set; }

        private FilePathService FilePathService { get; }

        private bool ReadOnly { get; }

        public SqliteContext()
        {
        }

        public SqliteContext(FilePathService filePathService, bool readOnly = false)
        {
            FilePathService = filePathService;
            ReadOnly = readOnly;

            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(FilePathService == null ? "Data Source=Application.db" : $"Data Source={FilePathService.SQLiteDatabaseFilePath}");

            if (ReadOnly)
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscordApp>().HasIndex(a => a.ClientId).IsUnique();
        }
    }
}