using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class classSubjectss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClassSubject_SessionClass_ClassId",
                table: "SessionClassSubject");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "SessionClassSubject",
                newName: "SessionClassId");

            migrationBuilder.RenameColumn(
                name: "ClassSubjectId",
                table: "SessionClassSubject",
                newName: "SessionClassSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_SessionClassSubject_ClassId",
                table: "SessionClassSubject",
                newName: "IX_SessionClassSubject_SessionClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClassSubject_SessionClass_SessionClassId",
                table: "SessionClassSubject",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClassSubject_SessionClass_SessionClassId",
                table: "SessionClassSubject");

            migrationBuilder.RenameColumn(
                name: "SessionClassId",
                table: "SessionClassSubject",
                newName: "ClassId");

            migrationBuilder.RenameColumn(
                name: "SessionClassSubjectId",
                table: "SessionClassSubject",
                newName: "ClassSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_SessionClassSubject_SessionClassId",
                table: "SessionClassSubject",
                newName: "IX_SessionClassSubject_ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClassSubject_SessionClass_ClassId",
                table: "SessionClassSubject",
                column: "ClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
