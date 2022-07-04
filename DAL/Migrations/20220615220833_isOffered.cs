using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class isOffered : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOffered",
                table: "ScoreEntry",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSaved",
                table: "ScoreEntry",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOffered",
                table: "ScoreEntry");

            migrationBuilder.DropColumn(
                name: "IsSaved",
                table: "ScoreEntry");
        }
    }
}
