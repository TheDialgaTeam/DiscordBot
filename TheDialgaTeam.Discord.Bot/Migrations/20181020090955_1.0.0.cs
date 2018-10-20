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
                    ClientId = table.Column<ulong>(nullable: false),
                    ClientSecret = table.Column<string>(nullable: false),
                    AppName = table.Column<string>(nullable: true),
                    AppDescription = table.Column<string>(nullable: true),
                    BotToken = table.Column<string>(nullable: false),
                    LastUpdateCheck = table.Column<DateTimeOffset>(nullable: true)
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
                    UserId = table.Column<ulong>(nullable: false),
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
                    GuildId = table.Column<ulong>(nullable: false),
                    Prefix = table.Column<string>(nullable: true),
                    DeleteCommandAfterUse = table.Column<bool>(nullable: false),
                    DiscordAppId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordGuildTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordGuildTable_DiscordAppTable_DiscordAppId",
                        column: x => x.DiscordAppId,
                        principalTable: "DiscordAppTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordChannelTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelId = table.Column<ulong>(nullable: false),
                    DiscordGuildId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordChannelTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordChannelTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordGuildModeratorTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<ulong>(nullable: false),
                    DiscordGuildId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordGuildModeratorTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordGuildModeratorTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordGuildModuleTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Module = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    DiscordGuildId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordGuildModuleTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordGuildModuleTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordChannelModeratorTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<ulong>(nullable: false),
                    DiscordChannelId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordChannelModeratorTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordChannelModeratorTable_DiscordChannelTable_DiscordChannelId",
                        column: x => x.DiscordChannelId,
                        principalTable: "DiscordChannelTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscordAppOwnerTable_DiscordAppId",
                table: "DiscordAppOwnerTable",
                column: "DiscordAppId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordAppTable_ClientId",
                table: "DiscordAppTable",
                column: "ClientId",
                unique: true);

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
