using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TheDialgaTeam.Discord.Bot.Migrations
{
    public partial class _100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordAppTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<ulong>(nullable: true),
                    ClientSecret = table.Column<string>(nullable: true),
                    AppName = table.Column<string>(nullable: true),
                    AppDescription = table.Column<string>(nullable: true),
                    BotToken = table.Column<string>(nullable: true),
                    LastUpdateCheck = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordAppTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscordAppOwnerTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(nullable: true),
                    DiscordAppId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordAppOwnerTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordAppOwnerTable_DiscordAppTable_DiscordAppId",
                        column: x => x.DiscordAppId,
                        principalTable: "DiscordAppTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscordGuildTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    DiscordAppId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordGuildTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordGuildTable_DiscordAppTable_DiscordAppId",
                        column: x => x.DiscordAppId,
                        principalTable: "DiscordAppTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscordChannelTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelId = table.Column<ulong>(nullable: true),
                    DiscordGuildId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordChannelTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordChannelTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscordGuildModeratorTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<ulong>(nullable: true),
                    DiscordGuildId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordGuildModeratorTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordGuildModeratorTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscordGuildModuleTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Module = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: true),
                    DiscordGuildId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordGuildModuleTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordGuildModuleTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscordChannelModeratorTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<ulong>(nullable: true),
                    DiscordChannelId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordChannelModeratorTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordChannelModeratorTable_DiscordChannelTable_DiscordChannelId",
                        column: x => x.DiscordChannelId,
                        principalTable: "DiscordChannelTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscordAppOwnerTable_DiscordAppId",
                table: "DiscordAppOwnerTable",
                column: "DiscordAppId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordChannelModeratorTable_DiscordChannelId",
                table: "DiscordChannelModeratorTable",
                column: "DiscordChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordChannelTable_DiscordGuildId",
                table: "DiscordChannelTable",
                column: "DiscordGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordGuildModeratorTable_DiscordGuildId",
                table: "DiscordGuildModeratorTable",
                column: "DiscordGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordGuildModuleTable_DiscordGuildId",
                table: "DiscordGuildModuleTable",
                column: "DiscordGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordGuildTable_DiscordAppId",
                table: "DiscordGuildTable",
                column: "DiscordAppId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordAppOwnerTable");

            migrationBuilder.DropTable(
                name: "DiscordChannelModeratorTable");

            migrationBuilder.DropTable(
                name: "DiscordGuildModeratorTable");

            migrationBuilder.DropTable(
                name: "DiscordGuildModuleTable");

            migrationBuilder.DropTable(
                name: "DiscordChannelTable");

            migrationBuilder.DropTable(
                name: "DiscordGuildTable");

            migrationBuilder.DropTable(
                name: "DiscordAppTable");
        }
    }
}
