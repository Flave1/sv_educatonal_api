using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class addedPublishedaaa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublishStatus",
                columns: table => new
                {
                    PublishStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishStatus", x => x.PublishStatusId);
                    table.ForeignKey(
                        name: "FK_PublishStatus_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishStatus_SessionTerm_SessionTermId",
                        column: x => x.SessionTermId,
                        principalTable: "SessionTerm",
                        principalColumn: "SessionTermId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublishStatus_SessionClassId",
                table: "PublishStatus",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishStatus_SessionTermId",
                table: "PublishStatus",
                column: "SessionTermId");
        }
    }
}
