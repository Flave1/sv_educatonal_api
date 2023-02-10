using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class RegNoSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegNoPosition",
                table: "SchoolSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegNoSeperator",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentRegNoFormat",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherRegNoFormat",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegNoPosition",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RegNoSeperator",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "StudentRegNoFormat",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "TeacherRegNoFormat",
                table: "SchoolSettings");
        }
    }
}
