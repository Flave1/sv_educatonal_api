using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class AdmissionParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_AdmissionNotifications_AdmissionNotificationId",
                table: "Admissions");

            migrationBuilder.DropTable(
                name: "AdmissionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_AdmissionNotificationId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "AdmissionNotificationId",
                table: "Admissions");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Admissions",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Admissions");

            migrationBuilder.AddColumn<Guid>(
                name: "AdmissionNotificationId",
                table: "Admissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AdmissionNotifications",
                columns: table => new
                {
                    AdmissionNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ParentEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionNotifications", x => x.AdmissionNotificationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_AdmissionNotificationId",
                table: "Admissions",
                column: "AdmissionNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_AdmissionNotifications_AdmissionNotificationId",
                table: "Admissions",
                column: "AdmissionNotificationId",
                principalTable: "AdmissionNotifications",
                principalColumn: "AdmissionNotificationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
