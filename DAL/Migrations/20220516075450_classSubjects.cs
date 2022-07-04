using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class classSubjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionClassSubject",
                columns: table => new
                {
                    ClassSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectTeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionClassSubject", x => x.ClassSubjectId);
                    table.ForeignKey(
                        name: "FK_SessionClassSubject_SessionClass_ClassId",
                        column: x => x.ClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionClassSubject_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionClassSubject_Teacher_SubjectTeacherId",
                        column: x => x.SubjectTeacherId,
                        principalTable: "Teacher",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionClassSubject_ClassId",
                table: "SessionClassSubject",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClassSubject_SubjectId",
                table: "SessionClassSubject",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClassSubject_SubjectTeacherId",
                table: "SessionClassSubject",
                column: "SubjectTeacherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionClassSubject");
        }
    }
}
