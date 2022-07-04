using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class markOnClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssessmentScore",
                table: "SessionClass",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExamScore",
                table: "SessionClass",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PassMark",
                table: "SessionClass",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessmentScore",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "ExamScore",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "PassMark",
                table: "SessionClass");
        }
    }
}
