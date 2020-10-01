using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Data.Migrations
{
    public partial class AddedSecretColumnToParticipants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Secret",
                table: "Participants",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Secret",
                table: "Participants");
        }
    }
}
