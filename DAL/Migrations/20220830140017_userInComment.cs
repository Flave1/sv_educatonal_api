using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class userInComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentNoteComment_StudentContact_StudentContactId",
                table: "StudentNoteComment");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentNoteComment_Teacher_TeacherId",
                table: "StudentNoteComment");

            migrationBuilder.DropIndex(
                name: "IX_StudentNoteComment_StudentContactId",
                table: "StudentNoteComment");

            migrationBuilder.DropIndex(
                name: "IX_StudentNoteComment_TeacherId",
                table: "StudentNoteComment");

            migrationBuilder.DropColumn(
                name: "StudentContactId",
                table: "StudentNoteComment");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "StudentNoteComment");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StudentNoteComment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentNoteComment_UserId",
                table: "StudentNoteComment",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNoteComment_AspNetUsers_UserId",
                table: "StudentNoteComment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentNoteComment_AspNetUsers_UserId",
                table: "StudentNoteComment");

            migrationBuilder.DropIndex(
                name: "IX_StudentNoteComment_UserId",
                table: "StudentNoteComment");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StudentNoteComment");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentContactId",
                table: "StudentNoteComment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "StudentNoteComment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentNoteComment_StudentContactId",
                table: "StudentNoteComment",
                column: "StudentContactId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNoteComment_TeacherId",
                table: "StudentNoteComment",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNoteComment_StudentContact_StudentContactId",
                table: "StudentNoteComment",
                column: "StudentContactId",
                principalTable: "StudentContact",
                principalColumn: "StudentContactId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNoteComment_Teacher_TeacherId",
                table: "StudentNoteComment",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
