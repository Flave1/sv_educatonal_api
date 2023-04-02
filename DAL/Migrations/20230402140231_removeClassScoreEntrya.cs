using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class removeClassScoreEntrya : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreEntry_Subject_SubjectId",
                table: "ScoreEntry");

            migrationBuilder.DropIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "ScoreEntry",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry",
                column: "SubjectId",
                unique: true,
                filter: "[SubjectId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreEntry_Subject_SubjectId",
                table: "ScoreEntry",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreEntry_Subject_SubjectId",
                table: "ScoreEntry");

            migrationBuilder.DropIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "ScoreEntry",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntry_SubjectId",
                table: "ScoreEntry",
                column: "SubjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreEntry_Subject_SubjectId",
                table: "ScoreEntry",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
