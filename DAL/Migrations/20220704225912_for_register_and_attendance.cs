using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class for_register_and_attendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassRegister",
                columns: table => new
                {
                    ClassRegisterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassRegister", x => x.ClassRegisterId);
                    table.ForeignKey(
                        name: "FK_ClassRegister_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAttendance",
                columns: table => new
                {
                    ClassAttendanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassRegisterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttendance", x => x.ClassAttendanceId);
                    table.ForeignKey(
                        name: "FK_StudentAttendance_ClassRegister_ClassRegisterId",
                        column: x => x.ClassRegisterId,
                        principalTable: "ClassRegister",
                        principalColumn: "ClassRegisterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAttendance_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassRegister_SessionClassId",
                table: "ClassRegister",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendance_ClassRegisterId",
                table: "StudentAttendance",
                column: "ClassRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendance_StudentContactId",
                table: "StudentAttendance",
                column: "StudentContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAttendance");

            migrationBuilder.DropTable(
                name: "ClassRegister");
        }
    }
}