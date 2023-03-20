using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class AdmissionParent2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentName",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "ParentPhoneNumber",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "ParentRelationship",
                table: "Admissions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentName",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentPhoneNumber",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentRelationship",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
