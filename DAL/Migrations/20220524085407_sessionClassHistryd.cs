using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class sessionClassHistryd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_ClassId",
                table: "StudentContact");

            migrationBuilder.DropIndex(
                name: "IX_StudentContact_ClassId",
                table: "StudentContact");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "StudentContact");

            migrationBuilder.CreateIndex(
                name: "IX_StudentContact_SessionClassId",
                table: "StudentContact",
                column: "SessionClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact");

            migrationBuilder.DropIndex(
                name: "IX_StudentContact_SessionClassId",
                table: "StudentContact");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "StudentContact",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentContact_ClassId",
                table: "StudentContact",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_ClassId",
                table: "StudentContact",
                column: "ClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
