using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class somecodefg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_StudentContactId",
                table: "StudentContact");

            migrationBuilder.RenameColumn(
                name: "SessionClassId",
                table: "StudentContact",
                newName: "ClassSessionClassId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentContact_SessionClassId",
                table: "StudentContact",
                newName: "IX_StudentContact_ClassSessionClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_ClassSessionClassId",
                table: "StudentContact",
                column: "ClassSessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_ClassSessionClassId",
                table: "StudentContact");

            migrationBuilder.RenameColumn(
                name: "ClassSessionClassId",
                table: "StudentContact",
                newName: "SessionClassId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentContact_ClassSessionClassId",
                table: "StudentContact",
                newName: "IX_StudentContact_SessionClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_SessionClassId",
                table: "StudentContact",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_StudentContactId",
                table: "StudentContact",
                column: "StudentContactId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
