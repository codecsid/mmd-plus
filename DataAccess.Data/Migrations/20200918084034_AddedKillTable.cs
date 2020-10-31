using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccess.Data.Migrations
{
    public partial class AddedKillTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultLives",
                table: "RoundConfigs");

            migrationBuilder.AddColumn<int>(
                name: "LifeLines",
                table: "RoundConfigs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Kill",
                columns: table => new
                {
                    RoundId = table.Column<Guid>(nullable: false),
                    VictimId = table.Column<string>(maxLength: 20, nullable: false),
                    KillerId = table.Column<string>(maxLength: 20, nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<Guid>(nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kill", x => new { x.RoundId, x.VictimId, x.KillerId });
                    table.ForeignKey(
                        name: "FK__Kill__Game",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Kill__Killer",
                        column: x => x.KillerId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Kill__Round",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Kill__Victim",
                        column: x => x.VictimId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kill_GameId",
                table: "Kill",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Kill_KillerId",
                table: "Kill",
                column: "KillerId");

            migrationBuilder.CreateIndex(
                name: "IX_Kill_VictimId",
                table: "Kill",
                column: "VictimId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kill");

            migrationBuilder.DropColumn(
                name: "LifeLines",
                table: "RoundConfigs");

            migrationBuilder.AddColumn<int>(
                name: "DefaultLives",
                table: "RoundConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
