using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class announcementf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SentBy",
                table: "Announcement",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Announcement_SentBy",
                table: "Announcement",
                column: "SentBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcement_AspNetUsers_SentBy",
                table: "Announcement",
                column: "SentBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcement_AspNetUsers_SentBy",
                table: "Announcement");

            migrationBuilder.DropIndex(
                name: "IX_Announcement_SentBy",
                table: "Announcement");

            migrationBuilder.AlterColumn<string>(
                name: "SentBy",
                table: "Announcement",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
