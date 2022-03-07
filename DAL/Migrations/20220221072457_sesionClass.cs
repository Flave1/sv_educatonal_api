using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class sesionClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "StudentContact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SessionClassId",
                table: "StudentContact",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Teacher",
                columns: table => new
                {
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher", x => x.TeacherId);
                    table.ForeignKey(
                        name: "FK_Teacher_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionClass",
                columns: table => new
                {
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormTeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InSession = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionClass", x => x.SessionClassId);
                    table.ForeignKey(
                        name: "FK_SessionClass_ClassLookUp_ClassId",
                        column: x => x.ClassId,
                        principalTable: "ClassLookUp",
                        principalColumn: "ClassLookupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionClass_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Session",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionClass_Teacher_FormTeacherId",
                        column: x => x.FormTeacherId,
                        principalTable: "Teacher",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentClassProgressions",
                columns: table => new
                {
                    StudentClassProgressionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClassProgressions", x => x.StudentClassProgressionId);
                    table.ForeignKey(
                        name: "FK_StudentClassProgressions_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClassProgressions_StudentContact_StudentId",
                        column: x => x.StudentId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentContact_SessionClassId",
                table: "StudentContact",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_ClassId",
                table: "SessionClass",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_FormTeacherId",
                table: "SessionClass",
                column: "FormTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_SessionId",
                table: "SessionClass",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassProgressions_SessionClassId",
                table: "StudentClassProgressions",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassProgressions_StudentId",
                table: "StudentClassProgressions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_UserId",
                table: "Teacher",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact");

            migrationBuilder.DropTable(
                name: "StudentClassProgressions");

            migrationBuilder.DropTable(
                name: "SessionClass");

            migrationBuilder.DropTable(
                name: "Teacher");

            migrationBuilder.DropIndex(
                name: "IX_StudentContact_SessionClassId",
                table: "StudentContact");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "StudentContact");

            migrationBuilder.DropColumn(
                name: "SessionClassId",
                table: "StudentContact");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
