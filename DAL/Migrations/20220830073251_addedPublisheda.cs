using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class addedPublisheda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_PublishStatus_PublishStatusId",
                table: "SessionClass");

            migrationBuilder.DropIndex(
                name: "IX_SessionClass_PublishStatusId",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "PublishStatusId",
                table: "SessionClass");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublishStatusId",
                table: "SessionClass",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_PublishStatusId",
                table: "SessionClass",
                column: "PublishStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClass_PublishStatus_PublishStatusId",
                table: "SessionClass",
                column: "PublishStatusId",
                principalTable: "PublishStatus",
                principalColumn: "PublishStatusId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
