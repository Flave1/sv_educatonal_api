using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class parentas1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentOrGuardianEmail",
                table: "StudentContact");

            migrationBuilder.DropColumn(
                name: "ParentOrGuardianName",
                table: "StudentContact");

            migrationBuilder.DropColumn(
                name: "ParentOrGuardianPhone",
                table: "StudentContact");

            migrationBuilder.DropColumn(
                name: "ParentOrGuardianRelationship",
                table: "StudentContact");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "StudentContact",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentContact_ParentId",
                table: "StudentContact",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_Parents_ParentId",
                table: "StudentContact",
                column: "ParentId",
                principalTable: "Parents",
                principalColumn: "Parentid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_Parents_ParentId",
                table: "StudentContact");

            migrationBuilder.DropIndex(
                name: "IX_StudentContact_ParentId",
                table: "StudentContact");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "StudentContact");

            migrationBuilder.AddColumn<string>(
                name: "ParentOrGuardianEmail",
                table: "StudentContact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentOrGuardianName",
                table: "StudentContact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentOrGuardianPhone",
                table: "StudentContact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentOrGuardianRelationship",
                table: "StudentContact",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
