using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class promotClassed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PromotedSessionClass_SessionClassId",
                table: "PromotedSessionClass");

            migrationBuilder.CreateIndex(
                name: "IX_PromotedSessionClass_SessionClassId",
                table: "PromotedSessionClass",
                column: "SessionClassId",
                unique: true,
                filter: "[SessionClassId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PromotedSessionClass_SessionClassId",
                table: "PromotedSessionClass");

            migrationBuilder.CreateIndex(
                name: "IX_PromotedSessionClass_SessionClassId",
                table: "PromotedSessionClass",
                column: "SessionClassId");
        }
    }
}
