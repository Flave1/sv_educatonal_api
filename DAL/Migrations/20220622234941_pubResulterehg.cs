using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class pubResulterehg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScoreEntryId",
                table: "Subject",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subject_ScoreEntryId",
                table: "Subject",
                column: "ScoreEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_ScoreEntry_ScoreEntryId",
                table: "Subject",
                column: "ScoreEntryId",
                principalTable: "ScoreEntry",
                principalColumn: "ScoreEntryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subject_ScoreEntry_ScoreEntryId",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_Subject_ScoreEntryId",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "ScoreEntryId",
                table: "Subject");
        }
    }
}
