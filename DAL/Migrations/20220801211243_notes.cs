using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class notes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassNote",
                columns: table => new
                {
                    ClassNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoteTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoteContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AprrovalStatus = table.Column<int>(type: "int", nullable: false),
                    Classes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassNote", x => x.ClassNoteId);
                    table.ForeignKey(
                        name: "FK_ClassNote_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentNote",
                columns: table => new
                {
                    StudentNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoteTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoteContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AprrovalStatus = table.Column<int>(type: "int", nullable: false),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentNote", x => x.StudentNoteId);
                    table.ForeignKey(
                        name: "FK_StudentNote_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentNote_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentNote_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentNote_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherClassNote",
                columns: table => new
                {
                    TeacherClassNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherClassNote", x => x.TeacherClassNoteId);
                    table.ForeignKey(
                        name: "FK_TeacherClassNote_ClassNote_ClassNoteId",
                        column: x => x.ClassNoteId,
                        principalTable: "ClassNote",
                        principalColumn: "ClassNoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherClassNote_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassNote_SubjectId",
                table: "ClassNote",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNote_SessionClassId",
                table: "StudentNote",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNote_StudentContactId",
                table: "StudentNote",
                column: "StudentContactId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNote_SubjectId",
                table: "StudentNote",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNote_TeacherId",
                table: "StudentNote",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassNote_ClassNoteId",
                table: "TeacherClassNote",
                column: "ClassNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassNote_TeacherId",
                table: "TeacherClassNote",
                column: "TeacherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentNote");

            migrationBuilder.DropTable(
                name: "TeacherClassNote");

            migrationBuilder.DropTable(
                name: "ClassNote");
        }
    }
}
