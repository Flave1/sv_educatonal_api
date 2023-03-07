using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class ExamTimeTableTermId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamTimeTable_SessionTerm_SessionTermId",
                table: "ExamTimeTable");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionTermId",
                table: "ExamTimeTable",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamTimeTable_SessionTerm_SessionTermId",
                table: "ExamTimeTable",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamTimeTable_SessionTerm_SessionTermId",
                table: "ExamTimeTable");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionTermId",
                table: "ExamTimeTable",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamTimeTable_SessionTerm_SessionTermId",
                table: "ExamTimeTable",
                column: "SessionTermId",
                principalTable: "SessionTerm",
                principalColumn: "SessionTermId",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
