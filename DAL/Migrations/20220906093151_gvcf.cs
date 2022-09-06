using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class gvcf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassNoteComment_Teacher_TeacherId",
                table: "TeacherClassNoteComment");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassNoteComment_TeacherId",
                table: "TeacherClassNoteComment");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "TeacherClassNoteComment");

            migrationBuilder.DropColumn(
                name: "DeadLine",
                table: "HomeAssessment");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TeacherClassNoteComment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateDeadLine",
                table: "HomeAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeDeadLine",
                table: "HomeAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOfferring",
                table: "AssessmentScoreRecord",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassNoteComment_UserId",
                table: "TeacherClassNoteComment",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassNoteComment_AspNetUsers_UserId",
                table: "TeacherClassNoteComment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassNoteComment_AspNetUsers_UserId",
                table: "TeacherClassNoteComment");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassNoteComment_UserId",
                table: "TeacherClassNoteComment");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TeacherClassNoteComment");

            migrationBuilder.DropColumn(
                name: "DateDeadLine",
                table: "HomeAssessment");

            migrationBuilder.DropColumn(
                name: "TimeDeadLine",
                table: "HomeAssessment");

            migrationBuilder.DropColumn(
                name: "IsOfferring",
                table: "AssessmentScoreRecord");

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "TeacherClassNoteComment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeadLine",
                table: "HomeAssessment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassNoteComment_TeacherId",
                table: "TeacherClassNoteComment",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassNoteComment_Teacher_TeacherId",
                table: "TeacherClassNoteComment",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
