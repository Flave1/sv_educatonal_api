using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class hist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionTermId",
                table: "StudentSessionClassHistory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessionClassHistory_SessionTermId",
                table: "StudentSessionClassHistory",
                column: "SessionTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSessionClassHistory_SessionTerm_SessionTermId",
                table: "StudentSessionClassHistory",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSessionClassHistory_SessionTerm_SessionTermId",
                table: "StudentSessionClassHistory");

            migrationBuilder.DropIndex(
                name: "IX_StudentSessionClassHistory_SessionTermId",
                table: "StudentSessionClassHistory");

            migrationBuilder.DropColumn(
                name: "SessionTermId",
                table: "StudentSessionClassHistory");
        }
    }
}
