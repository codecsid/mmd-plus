using DataAccess.Model.SharedModels;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Data.Migrations
{
    public partial class AddedJsonBlobToGuess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<GuessRequestBody>(
                name: "GuessRequest",
                table: "Guesses",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<GuessResponseBody>(
                name: "GuessResponse",
                table: "Guesses",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuessRequest",
                table: "Guesses");

            migrationBuilder.DropColumn(
                name: "GuessResponse",
                table: "Guesses");
        }
    }
}
