using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class comment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeacherClassNoteComment",
                columns: table => new
                {
                    TeacherClassNoteCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepliedToId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherClassNoteComment", x => x.TeacherClassNoteCommentId);
                    table.ForeignKey(
                        name: "FK_TeacherClassNoteComment_ClassNote_ClassNoteId",
                        column: x => x.ClassNoteId,
                        principalTable: "ClassNote",
                        principalColumn: "ClassNoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherClassNoteComment_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherClassNoteComment_TeacherClassNoteComment_RepliedToId",
                        column: x => x.RepliedToId,
                        principalTable: "TeacherClassNoteComment",
                        principalColumn: "TeacherClassNoteCommentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassNoteComment_ClassNoteId",
                table: "TeacherClassNoteComment",
                column: "ClassNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassNoteComment_RepliedToId",
                table: "TeacherClassNoteComment",
                column: "RepliedToId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassNoteComment_TeacherId",
                table: "TeacherClassNoteComment",
                column: "TeacherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeacherClassNoteComment");
        }
    }
}
