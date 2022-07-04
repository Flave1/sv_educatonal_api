using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class currentTerm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionTermId",
                table: "ScoreEntry",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_SessionTermId",
                table: "ScoreEntry",
                column: "SessionTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreEntry_SessionTerm_SessionTermId",
                table: "ScoreEntry",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreEntry_SessionTerm_SessionTermId",
                table: "ScoreEntry");

            migrationBuilder.DropIndex(
                name: "IX_ScoreEntry_SessionTermId",
                table: "ScoreEntry");

            migrationBuilder.DropColumn(
                name: "SessionTermId",
                table: "ScoreEntry");
        }
    }
}
