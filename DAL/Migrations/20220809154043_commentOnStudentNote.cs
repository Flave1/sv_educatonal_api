using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class commentOnStudentNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentNoteComment",
                columns: table => new
                {
                    StudentNoteCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsParent = table.Column<bool>(type: "bit", nullable: false),
                    StudentNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepliedToId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentNoteComment", x => x.StudentNoteCommentId);
                    table.ForeignKey(
                        name: "FK_StudentNoteComment_StudentNote_StudentNoteId",
                        column: x => x.StudentNoteId,
                        principalTable: "StudentNote",
                        principalColumn: "StudentNoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentNoteComment_StudentNoteComment_RepliedToId",
                        column: x => x.RepliedToId,
                        principalTable: "StudentNoteComment",
                        principalColumn: "StudentNoteCommentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentNoteComment_RepliedToId",
                table: "StudentNoteComment",
                column: "RepliedToId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNoteComment_StudentNoteId",
                table: "StudentNoteComment",
                column: "StudentNoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentNoteComment");
        }
    }
}
