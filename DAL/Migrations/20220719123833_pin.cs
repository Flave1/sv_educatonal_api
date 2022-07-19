using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class pin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UploadedPin",
                columns: table => new
                {
                    UploadedPinId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Pin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedPin", x => x.UploadedPinId);
                });

            migrationBuilder.CreateTable(
                name: "UsedPin",
                columns: table => new
                {
                    UsedPinId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedPinId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedPin", x => x.UsedPinId);
                    table.ForeignKey(
                        name: "FK_UsedPin_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Session",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsedPin_SessionTerm_SessionTermId",
                        column: x => x.SessionTermId,
                        principalTable: "SessionTerm",
                        principalColumn: "SessionTermId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsedPin_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsedPin_UploadedPin_UploadedPinId",
                        column: x => x.UploadedPinId,
                        principalTable: "UploadedPin",
                        principalColumn: "UploadedPinId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsedPin_SessionId",
                table: "UsedPin",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_UsedPin_SessionTermId",
                table: "UsedPin",
                column: "SessionTermId");

            migrationBuilder.CreateIndex(
                name: "IX_UsedPin_StudentContactId",
                table: "UsedPin",
                column: "StudentContactId");

            migrationBuilder.CreateIndex(
                name: "IX_UsedPin_UploadedPinId",
                table: "UsedPin",
                column: "UploadedPinId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsedPin");

            migrationBuilder.DropTable(
                name: "UploadedPin");
        }
    }
}
