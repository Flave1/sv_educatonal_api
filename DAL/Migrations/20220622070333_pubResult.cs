using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class pubResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublishStatusId",
                table: "SessionClass",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PublishStatus",
                columns: table => new
                {
                    PublishStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    SessionTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishStatus", x => x.PublishStatusId);
                    table.ForeignKey(
                        name: "FK_PublishStatus_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PublishStatus_SessionTerm_SessionTermId",
                        column: x => x.SessionTermId,
                        principalTable: "SessionTerm",
                        principalColumn: "SessionTermId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_PublishStatusId",
                table: "SessionClass",
                column: "PublishStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishStatus_SessionClassId",
                table: "PublishStatus",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishStatus_SessionTermId",
                table: "PublishStatus",
                column: "SessionTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClass_PublishStatus_PublishStatusId",
                table: "SessionClass",
                column: "PublishStatusId",
                principalTable: "PublishStatus",
                principalColumn: "PublishStatusId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_PublishStatus_PublishStatusId",
                table: "SessionClass");

            migrationBuilder.DropTable(
                name: "PublishStatus");

            migrationBuilder.DropIndex(
                name: "IX_SessionClass_PublishStatusId",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "PublishStatusId",
                table: "SessionClass");
        }
    }
}
