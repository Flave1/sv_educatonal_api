using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class hmassak : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassAssessment_SessionTerm_SessionTermId",
                table: "ClassAssessment");

            migrationBuilder.DropIndex(
                name: "IX_ClassAssessment_SessionTermId",
                table: "ClassAssessment");

            migrationBuilder.DropColumn(
                name: "SessionTermId",
                table: "ClassAssessment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionTermId",
                table: "ClassAssessment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessment_SessionTermId",
                table: "ClassAssessment",
                column: "SessionTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAssessment_SessionTerm_SessionTermId",
                table: "ClassAssessment",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
