using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class assessment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassAssessment",
                columns: table => new
                {
                    ClassAssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListOfStudentIds = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassAssessment", x => x.ClassAssessmentId);
                    table.ForeignKey(
                        name: "FK_ClassAssessment_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeAssessment",
                columns: table => new
                {
                    HomeAssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssessmentScore = table.Column<int>(type: "int", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeAssessment", x => x.HomeAssessmentId);
                    table.ForeignKey(
                        name: "FK_HomeAssessment_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeAssessment_SessionClassGroup_SessionClassGroupId",
                        column: x => x.SessionClassGroupId,
                        principalTable: "SessionClassGroup",
                        principalColumn: "SessionClassGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HomeAssessment_SessionClassSubject_SessionClassSubjectId",
                        column: x => x.SessionClassSubjectId,
                        principalTable: "SessionClassSubject",
                        principalColumn: "SessionClassSubjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HomeAssessment_SessionTerm_SessionTermId",
                        column: x => x.SessionTermId,
                        principalTable: "SessionTerm",
                        principalColumn: "SessionTermId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentScoreRecord",
                columns: table => new
                {
                    AssessmentScoreRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentType = table.Column<int>(type: "int", nullable: false),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeAssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClassAssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentScoreRecord", x => x.AssessmentScoreRecordId);
                    table.ForeignKey(
                        name: "FK_AssessmentScoreRecord_ClassAssessment_ClassAssessmentId",
                        column: x => x.ClassAssessmentId,
                        principalTable: "ClassAssessment",
                        principalColumn: "ClassAssessmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssessmentScoreRecord_HomeAssessment_HomeAssessmentId",
                        column: x => x.HomeAssessmentId,
                        principalTable: "HomeAssessment",
                        principalColumn: "HomeAssessmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssessmentScoreRecord_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeAssessmentFeedBack",
                columns: table => new
                {
                    HomeAssessmentFeedBackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeAssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeAssessmentFeedBack", x => x.HomeAssessmentFeedBackId);
                    table.ForeignKey(
                        name: "FK_HomeAssessmentFeedBack_HomeAssessment_HomeAssessmentId",
                        column: x => x.HomeAssessmentId,
                        principalTable: "HomeAssessment",
                        principalColumn: "HomeAssessmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeAssessmentFeedBack_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentScoreRecord_ClassAssessmentId",
                table: "AssessmentScoreRecord",
                column: "ClassAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentScoreRecord_HomeAssessmentId",
                table: "AssessmentScoreRecord",
                column: "HomeAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentScoreRecord_StudentContactId",
                table: "AssessmentScoreRecord",
                column: "StudentContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessment_SessionClassId",
                table: "ClassAssessment",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeAssessment_SessionClassGroupId",
                table: "HomeAssessment",
                column: "SessionClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeAssessment_SessionClassId",
                table: "HomeAssessment",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeAssessment_SessionClassSubjectId",
                table: "HomeAssessment",
                column: "SessionClassSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeAssessment_SessionTermId",
                table: "HomeAssessment",
                column: "SessionTermId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeAssessmentFeedBack_HomeAssessmentId",
                table: "HomeAssessmentFeedBack",
                column: "HomeAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeAssessmentFeedBack_StudentContactId",
                table: "HomeAssessmentFeedBack",
                column: "StudentContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentScoreRecord");

            migrationBuilder.DropTable(
                name: "HomeAssessmentFeedBack");

            migrationBuilder.DropTable(
                name: "ClassAssessment");

            migrationBuilder.DropTable(
                name: "HomeAssessment");
        }
    }
}
