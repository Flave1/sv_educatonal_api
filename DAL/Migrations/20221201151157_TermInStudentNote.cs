using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class TermInStudentNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionTermId",
                table: "StudentNote",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentNote_SessionTermId",
                table: "StudentNote",
                column: "SessionTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNote_SessionTerm_SessionTermId",
                table: "StudentNote",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentNote_SessionTerm_SessionTermId",
                table: "StudentNote");

            migrationBuilder.DropIndex(
                name: "IX_StudentNote_SessionTermId",
                table: "StudentNote");

            migrationBuilder.DropColumn(
                name: "SessionTermId",
                table: "StudentNote");
        }
    }
}
