using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class teacgerAct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Teacher",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hobbies",
                table: "Teacher",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortBiography",
                table: "Teacher",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "Hobbies",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "ShortBiography",
                table: "Teacher");
        }
    }
}
