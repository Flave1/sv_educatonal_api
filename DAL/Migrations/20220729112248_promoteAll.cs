using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class promoteAll : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowPositionOnResult",
                table: "ResultSetting");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowPositionOnResult",
                table: "ResultSetting",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
