using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class settingUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "StudentContact");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrincipalStample",
                table: "ResultSetting",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "PrincipalStample",
                table: "ResultSetting");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "StudentContact",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
