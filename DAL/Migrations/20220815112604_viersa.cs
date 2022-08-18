using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class viersa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionClassGroup",
                columns: table => new
                {
                    SessionClassGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListOfStudentContactIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionClassGroup", x => x.SessionClassGroupId);
                    table.ForeignKey(
                        name: "FK_SessionClassGroup_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionClassGroup_SessionClassSubject_SessionClassSubjectId",
                        column: x => x.SessionClassSubjectId,
                        principalTable: "SessionClassSubject",
                        principalColumn: "SessionClassSubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionClassGroup_SessionClassId",
                table: "SessionClassGroup",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClassGroup_SessionClassSubjectId",
                table: "SessionClassGroup",
                column: "SessionClassSubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionClassGroup");
        }
    }
}
