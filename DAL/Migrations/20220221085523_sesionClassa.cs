using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class sesionClassa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_Teacher_FormTeacherId",
                table: "SessionClass");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClassProgressions_SessionClass_SessionClassId",
                table: "StudentClassProgressions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClassProgressions_StudentContact_StudentId",
                table: "StudentClassProgressions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassId",
                table: "StudentContact",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "StudentClassProgressions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassId",
                table: "StudentClassProgressions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FormTeacherId",
                table: "SessionClass",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassCaptainId",
                table: "SessionClass",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_ClassCaptainId",
                table: "SessionClass",
                column: "ClassCaptainId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClass_StudentContact_ClassCaptainId",
                table: "SessionClass",
                column: "ClassCaptainId",
                principalTable: "StudentContact",
                principalColumn: "StudentContactId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClass_Teacher_FormTeacherId",
                table: "SessionClass",
                column: "FormTeacherId",
                principalTable: "Teacher",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClassProgressions_SessionClass_SessionClassId",
                table: "StudentClassProgressions",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClassProgressions_StudentContact_StudentId",
                table: "StudentClassProgressions",
                column: "StudentId",
                principalTable: "StudentContact",
                principalColumn: "StudentContactId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_StudentContact_ClassCaptainId",
                table: "SessionClass");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_Teacher_FormTeacherId",
                table: "SessionClass");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClassProgressions_SessionClass_SessionClassId",
                table: "StudentClassProgressions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClassProgressions_StudentContact_StudentId",
                table: "StudentClassProgressions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact");

            migrationBuilder.DropIndex(
                name: "IX_SessionClass_ClassCaptainId",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "ClassCaptainId",
                table: "SessionClass");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassId",
                table: "StudentContact",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "StudentClassProgressions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassId",
                table: "StudentClassProgressions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FormTeacherId",
                table: "SessionClass",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClass_Teacher_FormTeacherId",
                table: "SessionClass",
                column: "FormTeacherId",
                principalTable: "Teacher",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClassProgressions_SessionClass_SessionClassId",
                table: "StudentClassProgressions",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClassProgressions_StudentContact_StudentId",
                table: "StudentClassProgressions",
                column: "StudentId",
                principalTable: "StudentContact",
                principalColumn: "StudentContactId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
