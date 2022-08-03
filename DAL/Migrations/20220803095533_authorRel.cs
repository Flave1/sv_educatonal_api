using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class authorRel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "ClassNote",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassNote_Author",
                table: "ClassNote",
                column: "Author");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassNote_AspNetUsers_Author",
                table: "ClassNote",
                column: "Author",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassNote_AspNetUsers_Author",
                table: "ClassNote");

            migrationBuilder.DropIndex(
                name: "IX_ClassNote_Author",
                table: "ClassNote");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "ClassNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
