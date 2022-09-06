using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class teacgerActsa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Classes",
                table: "TeacherClassNote",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SessionClassSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "SessionClassSubject",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "SessionClassSubject",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SessionClassSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "SessionClassSubject",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Classes",
                table: "TeacherClassNote");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SessionClassSubject");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "SessionClassSubject");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "SessionClassSubject");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SessionClassSubject");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "SessionClassSubject");
        }
    }
}
