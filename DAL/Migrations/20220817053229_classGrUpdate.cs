using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class classGrUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClassGroup_SessionClass_SessionClassId",
                table: "SessionClassGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionClassGroup_SessionClassSubject_SessionClassSubjectId",
                table: "SessionClassGroup");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassSubjectId",
                table: "SessionClassGroup",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassId",
                table: "SessionClassGroup",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClassGroup_SessionClass_SessionClassId",
                table: "SessionClassGroup",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClassGroup_SessionClassSubject_SessionClassSubjectId",
                table: "SessionClassGroup",
                column: "SessionClassSubjectId",
                principalTable: "SessionClassSubject",
                principalColumn: "SessionClassSubjectId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClassGroup_SessionClass_SessionClassId",
                table: "SessionClassGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionClassGroup_SessionClassSubject_SessionClassSubjectId",
                table: "SessionClassGroup");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassSubjectId",
                table: "SessionClassGroup",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassId",
                table: "SessionClassGroup",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClassGroup_SessionClass_SessionClassId",
                table: "SessionClassGroup",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClassGroup_SessionClassSubject_SessionClassSubjectId",
                table: "SessionClassGroup",
                column: "SessionClassSubjectId",
                principalTable: "SessionClassSubject",
                principalColumn: "SessionClassSubjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
