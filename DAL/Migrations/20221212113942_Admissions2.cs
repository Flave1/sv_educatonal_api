using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class Admissions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdmissionNotifications",
                columns: table => new
                {
                    AdmissionNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionNotifications", x => x.AdmissionNotificationId);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionSettings",
                columns: table => new
                {
                    AdmissionSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Classes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdmissionStatus = table.Column<int>(type: "int", nullable: false),
                    PassedExamEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailedExamEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScreeningEmailContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationFee = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionSettings", x => x.AdmissionSettingId);
                });

            migrationBuilder.CreateTable(
                name: "Admissions",
                columns: table => new
                {
                    AdmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Middlename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LGAOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credentials = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentRelationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidateAdmissionStatus = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdmissionNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admissions", x => x.AdmissionId);
                    table.ForeignKey(
                        name: "FK_Admissions_AdmissionNotifications_AdmissionNotificationId",
                        column: x => x.AdmissionNotificationId,
                        principalTable: "AdmissionNotifications",
                        principalColumn: "AdmissionNotificationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Admissions_ClassLookUp_ClassId",
                        column: x => x.ClassId,
                        principalTable: "ClassLookUp",
                        principalColumn: "ClassLookupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_AdmissionNotificationId",
                table: "Admissions",
                column: "AdmissionNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_ClassId",
                table: "Admissions",
                column: "ClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admissions");

            migrationBuilder.DropTable(
                name: "AdmissionSettings");

            migrationBuilder.DropTable(
                name: "AdmissionNotifications");
        }
    }
}
