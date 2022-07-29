using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class promoteAlls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PromoteByPassmark",
                table: "ResultSetting",
                newName: "ShowPositionOnResult");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShowPositionOnResult",
                table: "ResultSetting",
                newName: "PromoteByPassmark");
        }
    }
}
