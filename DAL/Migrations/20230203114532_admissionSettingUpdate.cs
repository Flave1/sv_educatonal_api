using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class admissionSettingUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdmissionSettingName",
                table: "AdmissionSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdmissionSettingsId",
                table: "Admissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_AdmissionSettingsId",
                table: "Admissions",
                column: "AdmissionSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_AdmissionSettings_AdmissionSettingsId",
                table: "Admissions",
                column: "AdmissionSettingsId",
                principalTable: "AdmissionSettings",
                principalColumn: "AdmissionSettingId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_AdmissionSettings_AdmissionSettingsId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_AdmissionSettingsId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "AdmissionSettingName",
                table: "AdmissionSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionSettingsId",
                table: "Admissions");
        }
    }
}
