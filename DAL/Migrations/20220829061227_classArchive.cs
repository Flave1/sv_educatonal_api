using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class classArchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionClassArchive",
                columns: table => new
                {
                    SessionClassArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HasPrintedResult = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionClassArchive");
        }
    }
}
