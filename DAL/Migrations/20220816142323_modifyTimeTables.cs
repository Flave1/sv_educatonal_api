using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class modifyTimeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassTimeTableTime_ClassTimeTableDay_ClassTimeTableDayId",
                table: "ClassTimeTableTime");

            migrationBuilder.DropIndex(
                name: "IX_ClassTimeTableTime_ClassTimeTableDayId",
                table: "ClassTimeTableTime");

            migrationBuilder.DropColumn(
                name: "ClassTimeTableDayId",
                table: "ClassTimeTableTime");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassTimeTableDayId",
                table: "ClassTimeTableTimeActivity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTableTimeActivity_ClassTimeTableDayId",
                table: "ClassTimeTableTimeActivity",
                column: "ClassTimeTableDayId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTimeTableTimeActivity_ClassTimeTableDay_ClassTimeTableDayId",
                table: "ClassTimeTableTimeActivity",
                column: "ClassTimeTableDayId",
                principalTable: "ClassTimeTableDay",
                principalColumn: "ClassTimeTableDayId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassTimeTableTimeActivity_ClassTimeTableDay_ClassTimeTableDayId",
                table: "ClassTimeTableTimeActivity");

            migrationBuilder.DropIndex(
                name: "IX_ClassTimeTableTimeActivity_ClassTimeTableDayId",
                table: "ClassTimeTableTimeActivity");

            migrationBuilder.DropColumn(
                name: "ClassTimeTableDayId",
                table: "ClassTimeTableTimeActivity");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassTimeTableDayId",
                table: "ClassTimeTableTime",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTableTime_ClassTimeTableDayId",
                table: "ClassTimeTableTime",
                column: "ClassTimeTableDayId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTimeTableTime_ClassTimeTableDay_ClassTimeTableDayId",
                table: "ClassTimeTableTime",
                column: "ClassTimeTableDayId",
                principalTable: "ClassTimeTableDay",
                principalColumn: "ClassTimeTableDayId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
