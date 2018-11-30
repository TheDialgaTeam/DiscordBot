using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TheDialgaTeam.Discord.Bot.Migrations
{
    public partial class _100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordAppGlobalOwnerTable",
                columns: table => new
                {
                    DiscordAppGlobalOwnerId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordAppGlobalOwnerTable", x => x.DiscordAppGlobalOwnerId);
                });

            migrationBuilder.CreateTable(
                name: "DiscordAppTable",
                columns: table => new
                {
                    DiscordAppId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<ulong>(nullable: false),
                    ClientSecret = table.Column<string>(maxLength: 32, nullable: false),
                    AppName = table.Column<string>(maxLength: 32, nullable: true),
                    AppDescription = table.Column<string>(maxLength: 400, nullable: true),
                    BotToken = table.Column<string>(maxLength: 59, nullable: false),
                    LastUpdateCheck = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordAppTable", x => x.DiscordAppId);
                });

            migrationBuilder.CreateTable(
                name: "DiscordGuildTable",
                columns: table => new
                {
                    DiscordGuildId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordGuildTable", x => x.DiscordGuildId);
                });

            migrationBuilder.CreateTable(
                name: "DiscordModuleTable",
                columns: table => new
                {
                    DiscordModuleId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Module = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordModuleTable", x => x.DiscordModuleId);
                });

            migrationBuilder.CreateTable(
                name: "DiscordAppOwnerTable",
                columns: table => new
                {
                    DiscordAppOwnerId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(nullable: false),
                    DiscordAppId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordAppOwnerTable", x => x.DiscordAppOwnerId);
                    table.ForeignKey(
                        name: "FK_DiscordAppOwnerTable_DiscordAppTable_DiscordAppId",
                        column: x => x.DiscordAppId,
                        principalTable: "DiscordAppTable",
                        principalColumn: "DiscordAppId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordAppGuildTable",
                columns: table => new
                {
                    DiscordAppGuildId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prefix = table.Column<string>(maxLength: 255, nullable: true),
                    DeleteCommandAfterUse = table.Column<bool>(nullable: false),
                    DiscordAppId = table.Column<int>(nullable: false),
                    DiscordGuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordAppGuildTable", x => x.DiscordAppGuildId);
                    table.ForeignKey(
                        name: "FK_DiscordAppGuildTable_DiscordAppTable_DiscordAppId",
                        column: x => x.DiscordAppId,
                        principalTable: "DiscordAppTable",
                        principalColumn: "DiscordAppId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscordAppGuildTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "DiscordGuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordChannelTable",
                columns: table => new
                {
                    DiscordChannelId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelId = table.Column<ulong>(nullable: false),
                    DiscordGuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordChannelTable", x => x.DiscordChannelId);
                    table.ForeignKey(
                        name: "FK_DiscordChannelTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "DiscordGuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordGuildModeratorTable",
                columns: table => new
                {
                    DiscordGuildModeratorId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<ulong>(nullable: false),
                    DiscordGuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordGuildModeratorTable", x => x.DiscordGuildModeratorId);
                    table.ForeignKey(
                        name: "FK_DiscordGuildModeratorTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "DiscordGuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordModuleRequirementTable",
                columns: table => new
                {
                    DiscordModuleRequirementId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordModuleId = table.Column<int>(nullable: false),
                    DiscordGuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordModuleRequirementTable", x => x.DiscordModuleRequirementId);
                    table.ForeignKey(
                        name: "FK_DiscordModuleRequirementTable_DiscordGuildTable_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "DiscordGuildTable",
                        principalColumn: "DiscordGuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscordModuleRequirementTable_DiscordModuleTable_DiscordModuleId",
                        column: x => x.DiscordModuleId,
                        principalTable: "DiscordModuleTable",
                        principalColumn: "DiscordModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordAppGuildModuleTable",
                columns: table => new
                {
                    DiscordAppGuildModuleId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Active = table.Column<bool>(nullable: false),
                    DiscordAppGuildId = table.Column<int>(nullable: false),
                    DiscordModuleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordAppGuildModuleTable", x => x.DiscordAppGuildModuleId);
                    table.ForeignKey(
                        name: "FK_DiscordAppGuildModuleTable_DiscordAppGuildTable_DiscordAppGuildId",
                        column: x => x.DiscordAppGuildId,
                        principalTable: "DiscordAppGuildTable",
                        principalColumn: "DiscordAppGuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscordAppGuildModuleTable_DiscordModuleTable_DiscordModuleId",
                        column: x => x.DiscordModuleId,
                        principalTable: "DiscordModuleTable",
                        principalColumn: "DiscordModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordAppChannelTable",
                columns: table => new
                {
                    DiscordAppChannelId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordAppGuildId = table.Column<int>(nullable: false),
                    DiscordChannelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordAppChannelTable", x => x.DiscordAppChannelId);
                    table.ForeignKey(
                        name: "FK_DiscordAppChannelTable_DiscordAppGuildTable_DiscordAppGuildId",
                        column: x => x.DiscordAppGuildId,
                        principalTable: "DiscordAppGuildTable",
                        principalColumn: "DiscordAppGuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscordAppChannelTable_DiscordChannelTable_DiscordChannelId",
                        column: x => x.DiscordChannelId,
                        principalTable: "DiscordChannelTable",
                        principalColumn: "DiscordChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordChannelModeratorTable",
                columns: table => new
                {
                    DiscordChannelModeratorId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<ulong>(nullable: false),
                    DiscordChannelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordChannelModeratorTable", x => x.DiscordChannelModeratorId);
                    table.ForeignKey(
                        name: "FK_DiscordChannelModeratorTable_DiscordChannelTable_DiscordChannelId",
                        column: x => x.DiscordChannelId,
                        principalTable: "DiscordChannelTable",
                        principalColumn: "DiscordChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FreeGameNotificationTable",
                columns: table => new
                {
                    FreeGameNotificationId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<ulong>(nullable: false),
                    DiscordChannelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreeGameNotificationTable", x => x.FreeGameNotificationId);
                    table.ForeignKey(
                        name: "FK_FreeGameNotificationTable_DiscordChannelTable_DiscordChannelId",
                        column: x => x.DiscordChannelId,
                        principalTable: "DiscordChannelTable",
                        principalColumn: "DiscordChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscordAppChannelTable_DiscordAppGuildId",
                table: "DiscordAppChannelTable",
                column: "DiscordAppGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordAppChannelTable_DiscordChannelId",
                table: "DiscordAppChannelTable",
                column: "DiscordChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordAppGuildModuleTable_DiscordAppGuildId",
                table: "DiscordAppGuildModuleTable",
                column: "DiscordAppGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordAppGuildModuleTable_DiscordModuleId",
                table: "DiscordAppGuildModuleTable",
                column: "DiscordModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordAppGuildTable_DiscordAppId",
                table: "DiscordAppGuildTable",
                column: "DiscordAppId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordAppGuildTable_DiscordGuildId",
                table: "DiscordAppGuildTable",
                column: "DiscordGuildId");

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
                name: "IX_DiscordModuleRequirementTable_DiscordGuildId",
                table: "DiscordModuleRequirementTable",
                column: "DiscordGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordModuleRequirementTable_DiscordModuleId",
                table: "DiscordModuleRequirementTable",
                column: "DiscordModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_FreeGameNotificationTable_DiscordChannelId",
                table: "FreeGameNotificationTable",
                column: "DiscordChannelId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordAppChannelTable");

            migrationBuilder.DropTable(
                name: "DiscordAppGlobalOwnerTable");

            migrationBuilder.DropTable(
                name: "DiscordAppGuildModuleTable");

            migrationBuilder.DropTable(
                name: "DiscordAppOwnerTable");

            migrationBuilder.DropTable(
                name: "DiscordChannelModeratorTable");

            migrationBuilder.DropTable(
                name: "DiscordGuildModeratorTable");

            migrationBuilder.DropTable(
                name: "DiscordModuleRequirementTable");

            migrationBuilder.DropTable(
                name: "FreeGameNotificationTable");

            migrationBuilder.DropTable(
                name: "DiscordAppGuildTable");

            migrationBuilder.DropTable(
                name: "DiscordModuleTable");

            migrationBuilder.DropTable(
                name: "DiscordChannelTable");

            migrationBuilder.DropTable(
                name: "DiscordAppTable");

            migrationBuilder.DropTable(
                name: "DiscordGuildTable");
        }
    }
}
