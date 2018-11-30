using Microsoft.EntityFrameworkCore.Migrations;

namespace TheDialgaTeam.Discord.Bot.Migrations
{
    public partial class _101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DiscordGuildTable_GuildId",
                table: "DiscordGuildTable",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscordChannelTable_ChannelId",
                table: "DiscordChannelTable",
                column: "ChannelId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiscordGuildTable_GuildId",
                table: "DiscordGuildTable");

            migrationBuilder.DropIndex(
                name: "IX_DiscordChannelTable_ChannelId",
                table: "DiscordChannelTable");
        }
    }
}
