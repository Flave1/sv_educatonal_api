using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class admissionSettingIdUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_AdmissionSettings_AdmissionSettingsId",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "AdmissionSettingsId",
                table: "Admissions",
                newName: "AdmissionSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_Admissions_AdmissionSettingsId",
                table: "Admissions",
                newName: "IX_Admissions_AdmissionSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_AdmissionSettings_AdmissionSettingId",
                table: "Admissions",
                column: "AdmissionSettingId",
                principalTable: "AdmissionSettings",
                principalColumn: "AdmissionSettingId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_AdmissionSettings_AdmissionSettingId",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "AdmissionSettingId",
                table: "Admissions",
                newName: "AdmissionSettingsId");

            migrationBuilder.RenameIndex(
                name: "IX_Admissions_AdmissionSettingId",
                table: "Admissions",
                newName: "IX_Admissions_AdmissionSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_AdmissionSettings_AdmissionSettingsId",
                table: "Admissions",
                column: "AdmissionSettingsId",
                principalTable: "AdmissionSettings",
                principalColumn: "AdmissionSettingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
