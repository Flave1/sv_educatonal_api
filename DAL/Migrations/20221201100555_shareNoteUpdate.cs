using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class shareNoteUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionTermId",
                table: "TeacherClassNote",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassNote_SessionTermId",
                table: "TeacherClassNote",
                column: "SessionTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassNote_SessionTerm_SessionTermId",
                table: "TeacherClassNote",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassNote_SessionTerm_SessionTermId",
                table: "TeacherClassNote");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassNote_SessionTermId",
                table: "TeacherClassNote");

            migrationBuilder.DropColumn(
                name: "SessionTermId",
                table: "TeacherClassNote");
        }
    }
}
