using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class activitiesuigh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleActivity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanImport = table.Column<bool>(type: "bit", nullable: false),
                    CanExport = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleActivity_Activity_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleActivity_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleActivity_ActivityId",
                table: "RoleActivity",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleActivity_RoleId",
                table: "RoleActivity",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleActivity");
        }
    }
}
