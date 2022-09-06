using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class commentOnAssessmenttas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "HomeAssessmentFeedBack",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "HomeAssessmentFeedBack",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "HomeAssessmentFeedBack",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "HomeAssessmentFeedBack",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "HomeAssessmentFeedBack",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "HomeAssessmentFeedBack");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "HomeAssessmentFeedBack");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "HomeAssessmentFeedBack");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "HomeAssessmentFeedBack");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "HomeAssessmentFeedBack");
        }
    }
}
