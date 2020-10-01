using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccess.Data.Migrations
{
    public partial class AddedMoreTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Kill",
                table: "Kill");

            migrationBuilder.RenameTable(
                name: "Kill",
                newName: "Kills");

            migrationBuilder.RenameIndex(
                name: "IX_Kill_VictimId",
                table: "Kills",
                newName: "IX_Kills_VictimId");

            migrationBuilder.RenameIndex(
                name: "IX_Kill_KillerId",
                table: "Kills",
                newName: "IX_Kills_KillerId");

            migrationBuilder.RenameIndex(
                name: "IX_Kill_GameId",
                table: "Kills",
                newName: "IX_Kills_GameId");

            migrationBuilder.AddColumn<bool>(
                name: "IsRobot",
                table: "Teams",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kills",
                table: "Kills",
                columns: new[] { "RoundId", "VictimId", "KillerId" });

            migrationBuilder.CreateTable(
                name: "FutureGames",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RunOnce = table.Column<bool>(nullable: false, defaultValue: false),
                    RunGamesTill = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guesses",
                columns: table => new
                {
                    GuessId = table.Column<Guid>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<Guid>(nullable: false),
                    RoundId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(maxLength: 20, nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guesses", x => x.GuessId);
                    table.ForeignKey(
                        name: "FK__Guess__Game",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Guess__Round",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Guess__Team",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    RoundId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(maxLength: 20, nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<Guid>(nullable: false),
                    IsAlive = table.Column<bool>(nullable: false, defaultValue: true),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => new { x.RoundId, x.TeamId });
                    table.ForeignKey(
                        name: "FK__Part__Game",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Part__Round",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Part__Team",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<Guid>(nullable: true),
                    RoundId = table.Column<Guid>(nullable: true),
                    TeamId = table.Column<string>(nullable: true),
                    RequestMethod = table.Column<string>(nullable: false),
                    RequestApi = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK__Req__Game",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Req__Round",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Req__Team",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<Guid>(nullable: false),
                    RoundId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(nullable: false),
                    GuessId = table.Column<Guid>(nullable: false),
                    PointsScored = table.Column<long>(nullable: false, defaultValue: 0L),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Score__Game",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Score__Guess",
                        column: x => x.GuessId,
                        principalTable: "Guesses",
                        principalColumn: "GuessId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Score__Round",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Score__Team",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guesses_GameId",
                table: "Guesses",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Guesses_RoundId",
                table: "Guesses",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Guesses_TeamId",
                table: "Guesses",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_GameId",
                table: "Participants",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_TeamId",
                table: "Participants",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_GameId",
                table: "Requests",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RoundId",
                table: "Requests",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_TeamId",
                table: "Requests",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_GameId",
                table: "Scores",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_GuessId",
                table: "Scores",
                column: "GuessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scores_RoundId",
                table: "Scores",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_TeamId",
                table: "Scores",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutureGames");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Guesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kills",
                table: "Kills");

            migrationBuilder.DropColumn(
                name: "IsRobot",
                table: "Teams");

            migrationBuilder.RenameTable(
                name: "Kills",
                newName: "Kill");

            migrationBuilder.RenameIndex(
                name: "IX_Kills_VictimId",
                table: "Kill",
                newName: "IX_Kill_VictimId");

            migrationBuilder.RenameIndex(
                name: "IX_Kills_KillerId",
                table: "Kill",
                newName: "IX_Kill_KillerId");

            migrationBuilder.RenameIndex(
                name: "IX_Kills_GameId",
                table: "Kill",
                newName: "IX_Kill_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kill",
                table: "Kill",
                columns: new[] { "RoundId", "VictimId", "KillerId" });
        }
    }
}
