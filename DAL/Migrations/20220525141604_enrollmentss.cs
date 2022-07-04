using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class enrollmentss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_SessionClass_ClassId",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_StudentContact_StudentId",
                table: "Enrollment");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Enrollment",
                newName: "StudentContactId");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "Enrollment",
                newName: "SessionClassId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollment_StudentId",
                table: "Enrollment",
                newName: "IX_Enrollment_StudentContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollment_ClassId",
                table: "Enrollment",
                newName: "IX_Enrollment_SessionClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_SessionClass_SessionClassId",
                table: "Enrollment",
                column: "SessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_StudentContact_StudentContactId",
                table: "Enrollment",
                column: "StudentContactId",
                principalTable: "StudentContact",
                principalColumn: "StudentContactId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_SessionClass_SessionClassId",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_StudentContact_StudentContactId",
                table: "Enrollment");

            migrationBuilder.RenameColumn(
                name: "StudentContactId",
                table: "Enrollment",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "SessionClassId",
                table: "Enrollment",
                newName: "ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollment_StudentContactId",
                table: "Enrollment",
                newName: "IX_Enrollment_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollment_SessionClassId",
                table: "Enrollment",
                newName: "IX_Enrollment_ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_SessionClass_ClassId",
                table: "Enrollment",
                column: "ClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_StudentContact_StudentId",
                table: "Enrollment",
                column: "StudentId",
                principalTable: "StudentContact",
                principalColumn: "StudentContactId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
