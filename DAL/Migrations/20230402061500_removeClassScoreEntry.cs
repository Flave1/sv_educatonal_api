using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class removeClassScoreEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreEntry_ClassScoreEntry_ClassScoreEntryId",
                table: "ScoreEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_Subject_ScoreEntry_ScoreEntryId",
                table: "Subject");

            migrationBuilder.DropTable(
                name: "ClassScoreEntry");

            migrationBuilder.DropTable(
                name: "SessionClassArchive");

            migrationBuilder.DropTable(
                name: "UsedPin");

            migrationBuilder.DropTable(
                name: "UploadedPin");

            migrationBuilder.DropIndex(
                name: "IX_Subject_ScoreEntryId",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_ScoreEntry_ClassScoreEntryId",
                table: "ScoreEntry");

            migrationBuilder.DropColumn(
                name: "ScoreEntryId",
                table: "Subject");

            migrationBuilder.RenameColumn(
                name: "ClassScoreEntryId",
                table: "ScoreEntry",
                newName: "SubjectId");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "StudentSessionClassHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPromoted",
                table: "SessionClass",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "SessionClass",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SessionTermId",
                table: "SessionClass",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SessionClassId",
                table: "ScoreEntry",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_SessionTermId",
                table: "SessionClass",
                column: "SessionTermId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_SessionClassId",
                table: "ScoreEntry",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry",
                column: "SubjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreEntry_SessionClass_SessionClassId",
                table: "ScoreEntry",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreEntry_Subject_SubjectId",
                table: "ScoreEntry",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClass_SessionTerm_SessionTermId",
                table: "SessionClass",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreEntry_SessionClass_SessionClassId",
                table: "ScoreEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_ScoreEntry_Subject_SubjectId",
                table: "ScoreEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_SessionTerm_SessionTermId",
                table: "SessionClass");

            migrationBuilder.DropIndex(
                name: "IX_SessionClass_SessionTermId",
                table: "SessionClass");

            migrationBuilder.DropIndex(
                name: "IX_ScoreEntry_SessionClassId",
                table: "ScoreEntry");

            migrationBuilder.DropIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "StudentSessionClassHistory");

            migrationBuilder.DropColumn(
                name: "IsPromoted",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "SessionTermId",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "SessionClassId",
                table: "ScoreEntry");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "ScoreEntry",
                newName: "ClassScoreEntryId");

            migrationBuilder.AddColumn<Guid>(
                name: "ScoreEntryId",
                table: "Subject",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassScoreEntry",
                columns: table => new
                {
                    ClassScoreEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                name: "SessionClassArchive",
                columns: table => new
                {
                    SessionClassArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    HasPrintedResult = table.Column<bool>(type: "bit", nullable: false),
                    IsPromoted = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionClassArchive", x => x.SessionClassArchiveId);
                    table.ForeignKey(
                        name: "FK_SessionClassArchive_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionClassArchive_SessionTerm_SessionTermId",
                        column: x => x.SessionTermId,
                        principalTable: "SessionTerm",
                        principalColumn: "SessionTermId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionClassArchive_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UploadedPin",
                columns: table => new
                {
                    UploadedPinId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Pin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Serial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedPin", x => x.UploadedPinId);
                });

            migrationBuilder.CreateTable(
                name: "UsedPin",
                columns: table => new
                {
                    UsedPinId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UploadedPinId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedPin", x => x.UsedPinId);
                    table.ForeignKey(
                        name: "FK_UsedPin_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsedPin_SessionTerm_SessionTermId",
                        column: x => x.SessionTermId,
                        principalTable: "SessionTerm",
                        principalColumn: "SessionTermId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsedPin_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsedPin_UploadedPin_UploadedPinId",
                        column: x => x.UploadedPinId,
                        principalTable: "UploadedPin",
                        principalColumn: "UploadedPinId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subject_ScoreEntryId",
                table: "Subject",
                column: "ScoreEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_ClassScoreEntryId",
                table: "ScoreEntry",
                column: "ClassScoreEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreEntry_SessionClassId",
                table: "ClassScoreEntry",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreEntry_SubjectId",
                table: "ClassScoreEntry",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClassArchive_SessionClassId",
                table: "SessionClassArchive",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClassArchive_SessionTermId",
                table: "SessionClassArchive",
                column: "SessionTermId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClassArchive_StudentContactId",
                table: "SessionClassArchive",
                column: "StudentContactId");

            migrationBuilder.CreateIndex(
                name: "IX_UsedPin_SessionClassId",
                table: "UsedPin",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_UsedPin_SessionTermId",
                table: "UsedPin",
                column: "SessionTermId");

            migrationBuilder.CreateIndex(
                name: "IX_UsedPin_StudentContactId",
                table: "UsedPin",
                column: "StudentContactId");

            migrationBuilder.CreateIndex(
                name: "IX_UsedPin_UploadedPinId",
                table: "UsedPin",
                column: "UploadedPinId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreEntry_ClassScoreEntry_ClassScoreEntryId",
                table: "ScoreEntry",
                column: "ClassScoreEntryId",
                principalTable: "ClassScoreEntry",
                principalColumn: "ClassScoreEntryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_ScoreEntry_ScoreEntryId",
                table: "Subject",
                column: "ScoreEntryId",
                principalTable: "ScoreEntry",
                principalColumn: "ScoreEntryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
