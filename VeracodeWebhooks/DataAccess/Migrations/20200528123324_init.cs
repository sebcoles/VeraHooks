using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WebHookName = table.Column<string>(nullable: true),
                    WebHookSendAddress = table.Column<string>(nullable: true),
                    TimeFired = table.Column<DateTime>(nullable: false),
                    StatusReturned = table.Column<int>(nullable: false),
                    MessageSent = table.Column<string>(nullable: true),
                    Response = table.Column<string>(nullable: true),
                    AppName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebHooks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    SendAddress = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastFired = table.Column<DateTime>(nullable: false),
                    TimesFired = table.Column<int>(nullable: false),
                    SecondsBetweenCheck = table.Column<int>(nullable: false),
                    Demand = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHooks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Apps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MitigationWebhookId = table.Column<int>(nullable: false),
                    AppId = table.Column<int>(nullable: false),
                    AppName = table.Column<string>(nullable: true),
                    LastBuild = table.Column<string>(nullable: true),
                    FlawString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apps_WebHooks_MitigationWebhookId",
                        column: x => x.MitigationWebhookId,
                        principalTable: "WebHooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conditons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MitigationWebhookId = table.Column<int>(nullable: false),
                    Field = table.Column<string>(nullable: true),
                    ExpectedValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conditons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conditons_WebHooks_MitigationWebhookId",
                        column: x => x.MitigationWebhookId,
                        principalTable: "WebHooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apps_MitigationWebhookId",
                table: "Apps",
                column: "MitigationWebhookId");

            migrationBuilder.CreateIndex(
                name: "IX_Conditons_MitigationWebhookId",
                table: "Conditons",
                column: "MitigationWebhookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apps");

            migrationBuilder.DropTable(
                name: "Conditons");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "WebHooks");
        }
    }
}
