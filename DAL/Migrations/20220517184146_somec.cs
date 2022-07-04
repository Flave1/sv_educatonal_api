using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class somec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HeadTeacherId",
                table: "Session",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Session_HeadTeacherId",
                table: "Session",
                column: "HeadTeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_Teacher_HeadTeacherId",
                table: "Session",
                column: "HeadTeacherId",
                principalTable: "Teacher",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_Teacher_HeadTeacherId",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_HeadTeacherId",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "HeadTeacherId",
                table: "Session");
        }
    }
}
