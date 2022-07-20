using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class sessionClassOd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedPin_Session_SessionId",
                table: "UsedPin");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "UsedPin",
                newName: "SessionClassId");

            migrationBuilder.RenameIndex(
                name: "IX_UsedPin_SessionId",
                table: "UsedPin",
                newName: "IX_UsedPin_SessionClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedPin_SessionClass_SessionClassId",
                table: "UsedPin",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedPin_SessionClass_SessionClassId",
                table: "UsedPin");

            migrationBuilder.RenameColumn(
                name: "SessionClassId",
                table: "UsedPin",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_UsedPin_SessionClassId",
                table: "UsedPin",
                newName: "IX_UsedPin_SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedPin_Session_SessionId",
                table: "UsedPin",
                column: "SessionId",
                principalTable: "Session",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
