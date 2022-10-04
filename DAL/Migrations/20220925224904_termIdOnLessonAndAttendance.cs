using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class termIdOnLessonAndAttendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionTermId",
                table: "ClassRegister",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SessionTermId",
                table: "ClassNote",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassRegister_SessionTermId",
                table: "ClassRegister",
                column: "SessionTermId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassNote_SessionTermId",
                table: "ClassNote",
                column: "SessionTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassNote_SessionTerm_SessionTermId",
                table: "ClassNote",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRegister_SessionTerm_SessionTermId",
                table: "ClassRegister",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassNote_SessionTerm_SessionTermId",
                table: "ClassNote");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassRegister_SessionTerm_SessionTermId",
                table: "ClassRegister");

            migrationBuilder.DropIndex(
                name: "IX_ClassRegister_SessionTermId",
                table: "ClassRegister");

            migrationBuilder.DropIndex(
                name: "IX_ClassNote_SessionTermId",
                table: "ClassNote");

            migrationBuilder.DropColumn(
                name: "SessionTermId",
                table: "ClassRegister");

            migrationBuilder.DropColumn(
                name: "SessionTermId",
                table: "ClassNote");
        }
    }
}
