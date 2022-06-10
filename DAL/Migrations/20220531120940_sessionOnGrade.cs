using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class sessionOnGrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "GradeGroup",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_GradeGroup_SessionId",
                table: "GradeGroup",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GradeGroup_Session_SessionId",
                table: "GradeGroup",
                column: "SessionId",
                principalTable: "Session",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GradeGroup_Session_SessionId",
                table: "GradeGroup");

            migrationBuilder.DropIndex(
                name: "IX_GradeGroup_SessionId",
                table: "GradeGroup");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "GradeGroup");
        }
    }
}
