using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class removeScoreOneTOOneFromSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry",
                column: "SubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry",
                column: "SubjectId",
                unique: true,
                filter: "[SubjectId] IS NOT NULL");
        }
    }
}
