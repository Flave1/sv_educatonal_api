using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class scoreEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassScoreEntry",
                columns: table => new
                {
                    ClassScoreEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassScoreEntry", x => x.ClassScoreEntryId);
                    table.ForeignKey(
                        name: "FK_ClassScoreEntry_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassScoreEntry_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreEntry",
                columns: table => new
                {
                    ScoreEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentScore = table.Column<int>(type: "int", nullable: false),
                    ExamScore = table.Column<int>(type: "int", nullable: false),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassScoreEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreEntry", x => x.ScoreEntryId);
                    table.ForeignKey(
                        name: "FK_ScoreEntry_ClassScoreEntry_ClassScoreEntryId",
                        column: x => x.ClassScoreEntryId,
                        principalTable: "ClassScoreEntry",
                        principalColumn: "ClassScoreEntryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoreEntry_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreEntry_SessionClassId",
                table: "ClassScoreEntry",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreEntry_SubjectId",
                table: "ClassScoreEntry",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_ClassScoreEntryId",
                table: "ScoreEntry",
                column: "ClassScoreEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_StudentContactId",
                table: "ScoreEntry",
                column: "StudentContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreEntry");

            migrationBuilder.DropTable(
                name: "ClassScoreEntry");
        }
    }
}
