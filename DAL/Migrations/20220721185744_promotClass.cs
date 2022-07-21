using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class promotClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassId",
                table: "PromotedSessionClass",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_PromotedSessionClass_SessionClassId",
                table: "PromotedSessionClass",
                column: "SessionClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotedSessionClass_SessionClass_SessionClassId",
                table: "PromotedSessionClass",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotedSessionClass_SessionClass_SessionClassId",
                table: "PromotedSessionClass");

            migrationBuilder.DropIndex(
                name: "IX_PromotedSessionClass_SessionClassId",
                table: "PromotedSessionClass");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionClassId",
                table: "PromotedSessionClass",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
