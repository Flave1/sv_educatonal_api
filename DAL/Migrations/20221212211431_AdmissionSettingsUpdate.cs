using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class AdmissionSettingsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScreeningEmailContent",
                table: "AdmissionSettings",
                newName: "ScreeningEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScreeningEmail",
                table: "AdmissionSettings",
                newName: "ScreeningEmailContent");
        }
    }
}
