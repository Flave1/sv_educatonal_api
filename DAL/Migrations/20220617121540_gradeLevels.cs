using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class gradeLevels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_GradeGroup_GradeGroupId",
                table: "SessionClass");

            migrationBuilder.DropIndex(
                name: "IX_SessionClass_GradeGroupId",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "GradeGroupId",
                table: "SessionClass");

            migrationBuilder.AddColumn<Guid>(
                name: "GradeGroupId",
                table: "ClassLookUp",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("64C399E2-D8BE-406A-E391-08DA4AB60955"));

            migrationBuilder.CreateIndex(
                name: "IX_ClassLookUp_GradeGroupId",
                table: "ClassLookUp",
                column: "GradeGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLookUp_GradeGroup_GradeGroupId",
                table: "ClassLookUp",
                column: "GradeGroupId",
                principalTable: "GradeGroup",
                principalColumn: "GradeGroupId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLookUp_GradeGroup_GradeGroupId",
                table: "ClassLookUp");

            migrationBuilder.DropIndex(
                name: "IX_ClassLookUp_GradeGroupId",
                table: "ClassLookUp");

            migrationBuilder.DropColumn(
                name: "GradeGroupId",
                table: "ClassLookUp");

            migrationBuilder.AddColumn<Guid>(
                name: "GradeGroupId",
                table: "SessionClass",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_GradeGroupId",
                table: "SessionClass",
                column: "GradeGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClass_GradeGroup_GradeGroupId",
                table: "SessionClass",
                column: "GradeGroupId",
                principalTable: "GradeGroup",
                principalColumn: "GradeGroupId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
