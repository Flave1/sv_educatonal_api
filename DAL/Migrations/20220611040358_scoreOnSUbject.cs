using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class scoreOnSUbject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssessmentScore",
                table: "SessionClassSubject",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExamScore",
                table: "SessionClassSubject",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessmentScore",
                table: "SessionClassSubject");

            migrationBuilder.DropColumn(
                name: "ExamScore",
                table: "SessionClassSubject");
        }
    }
}
