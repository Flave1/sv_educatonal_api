using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class classGrU : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionClassSubjectId",
                table: "ClassAssessment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "AssessmentScoreRecord",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessment_SessionClassSubjectId",
                table: "ClassAssessment",
                column: "SessionClassSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAssessment_SessionClassSubject_SessionClassSubjectId",
                table: "ClassAssessment",
                column: "SessionClassSubjectId",
                principalTable: "SessionClassSubject",
                principalColumn: "SessionClassSubjectId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassAssessment_SessionClassSubject_SessionClassSubjectId",
                table: "ClassAssessment");

            migrationBuilder.DropIndex(
                name: "IX_ClassAssessment_SessionClassSubjectId",
                table: "ClassAssessment");

            migrationBuilder.DropColumn(
                name: "SessionClassSubjectId",
                table: "ClassAssessment");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "AssessmentScoreRecord");
        }
    }
}
