using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class rev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassGrade");

            migrationBuilder.DropTable(
                name: "StudentClassProgressions");

            migrationBuilder.AddColumn<Guid>(
                name: "ScoreEntryId",
                table: "Subject",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssessmentScore",
                table: "SessionClassSubject",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExamScore",
                table: "SessionClassSubject",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PublishStatusId",
                table: "SessionClass",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GradeGroupId",
                table: "ClassLookUp",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
                name: "PromotedSessionClass",
                columns: table => new
                {
                    PromotedClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPromoted = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotedSessionClass", x => x.PromotedClassId);
                });

            migrationBuilder.CreateTable(
                name: "PublishStatus",
                columns: table => new
                {
                    PublishStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    SessionTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishStatus", x => x.PublishStatusId);
                    table.ForeignKey(
                        name: "FK_PublishStatus_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishStatus_SessionTerm_SessionTermId",
                        column: x => x.SessionTermId,
                        principalTable: "SessionTerm",
                        principalColumn: "SessionTermId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreEntry",
                columns: table => new
                {
                    ScoreEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentScore = table.Column<int>(type: "int", nullable: false),
                    ExamScore = table.Column<int>(type: "int", nullable: false),
                    IsOffered = table.Column<bool>(type: "bit", nullable: false),
                    IsSaved = table.Column<bool>(type: "bit", nullable: false),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassScoreEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                        name: "FK_ScoreEntry_SessionTerm_SessionTermId",
                        column: x => x.SessionTermId,
                        principalTable: "SessionTerm",
                        principalColumn: "SessionTermId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScoreEntry_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subject_ScoreEntryId",
                table: "Subject",
                column: "ScoreEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_PublishStatusId",
                table: "SessionClass",
                column: "PublishStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLookUp_GradeGroupId",
                table: "ClassLookUp",
                column: "GradeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreEntry_SessionClassId",
                table: "ClassScoreEntry",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreEntry_SubjectId",
                table: "ClassScoreEntry",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishStatus_SessionClassId",
                table: "PublishStatus",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishStatus_SessionTermId",
                table: "PublishStatus",
                column: "SessionTermId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_ClassScoreEntryId",
                table: "ScoreEntry",
                column: "ClassScoreEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_SessionTermId",
                table: "ScoreEntry",
                column: "SessionTermId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_StudentContactId",
                table: "ScoreEntry",
                column: "StudentContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLookUp_GradeGroup_GradeGroupId",
                table: "ClassLookUp",
                column: "GradeGroupId",
                principalTable: "GradeGroup",
                principalColumn: "GradeGroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClass_PublishStatus_PublishStatusId",
                table: "SessionClass",
                column: "PublishStatusId",
                principalTable: "PublishStatus",
                principalColumn: "PublishStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_ScoreEntry_ScoreEntryId",
                table: "Subject",
                column: "ScoreEntryId",
                principalTable: "ScoreEntry",
                principalColumn: "ScoreEntryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLookUp_GradeGroup_GradeGroupId",
                table: "ClassLookUp");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_PublishStatus_PublishStatusId",
                table: "SessionClass");

            migrationBuilder.DropForeignKey(
                name: "FK_Subject_ScoreEntry_ScoreEntryId",
                table: "Subject");

            migrationBuilder.DropTable(
                name: "PromotedSessionClass");

            migrationBuilder.DropTable(
                name: "PublishStatus");

            migrationBuilder.DropTable(
                name: "ScoreEntry");

            migrationBuilder.DropTable(
                name: "ClassScoreEntry");

            migrationBuilder.DropIndex(
                name: "IX_Subject_ScoreEntryId",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_SessionClass_PublishStatusId",
                table: "SessionClass");

            migrationBuilder.DropIndex(
                name: "IX_ClassLookUp_GradeGroupId",
                table: "ClassLookUp");

            migrationBuilder.DropColumn(
                name: "ScoreEntryId",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "AssessmentScore",
                table: "SessionClassSubject");

            migrationBuilder.DropColumn(
                name: "ExamScore",
                table: "SessionClassSubject");

            migrationBuilder.DropColumn(
                name: "PublishStatusId",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "GradeGroupId",
                table: "ClassLookUp");

            migrationBuilder.CreateTable(
                name: "ClassGrade",
                columns: table => new
                {
                    ClassGradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    GradeGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassGrade", x => x.ClassGradeId);
                    table.ForeignKey(
                        name: "FK_ClassGrade_GradeGroup_GradeGroupId",
                        column: x => x.GradeGroupId,
                        principalTable: "GradeGroup",
                        principalColumn: "GradeGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassGrade_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentClassProgressions",
                columns: table => new
                {
                    StudentClassProgressionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClassProgressions", x => x.StudentClassProgressionId);
                    table.ForeignKey(
                        name: "FK_StudentClassProgressions_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentClassProgressions_StudentContact_StudentId",
                        column: x => x.StudentId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassGrade_GradeGroupId",
                table: "ClassGrade",
                column: "GradeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGrade_SessionClassId",
                table: "ClassGrade",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassProgressions_SessionClassId",
                table: "StudentClassProgressions",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassProgressions_StudentId",
                table: "StudentClassProgressions",
                column: "StudentId");
        }
    }
}
