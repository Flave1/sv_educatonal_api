using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class sesionClasssasas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_StudentContact_ClassCaptainId",
                table: "SessionClass");

            migrationBuilder.DropIndex(
                name: "IX_SessionClass_ClassCaptainId",
                table: "SessionClass");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_StudentContactId",
                table: "StudentContact",
                column: "StudentContactId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_StudentContactId",
                table: "StudentContact");

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
        }
    }
}
