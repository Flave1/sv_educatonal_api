using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class sessionClassHistry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_ClassSessionClassId",
                table: "StudentContact");

            migrationBuilder.RenameColumn(
                name: "ClassSessionClassId",
                table: "StudentContact",
                newName: "ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentContact_ClassSessionClassId",
                table: "StudentContact",
                newName: "IX_StudentContact_ClassId");

            migrationBuilder.AddColumn<Guid>(
                name: "SessionClassId",
                table: "StudentContact",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "StudentSessionClassHistory",
                columns: table => new
                {
                    StudentSessionClassHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSessionClassHistory", x => x.StudentSessionClassHistoryId);
                    table.ForeignKey(
                        name: "FK_StudentSessionClassHistory_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSessionClassHistory_StudentContact_StudentContactId",
                        column: x => x.StudentContactId,
                        principalTable: "StudentContact",
                        principalColumn: "StudentContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessionClassHistory_SessionClassId",
                table: "StudentSessionClassHistory",
                column: "SessionClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessionClassHistory_StudentContactId",
                table: "StudentSessionClassHistory",
                column: "StudentContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_ClassId",
                table: "StudentContact",
                column: "ClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContact_SessionClass_ClassId",
                table: "StudentContact");

            migrationBuilder.DropTable(
                name: "StudentSessionClassHistory");

            migrationBuilder.DropColumn(
                name: "SessionClassId",
                table: "StudentContact");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "StudentContact",
                newName: "ClassSessionClassId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentContact_ClassId",
                table: "StudentContact",
                newName: "IX_StudentContact_ClassSessionClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContact_SessionClass_ClassSessionClassId",
                table: "StudentContact",
                column: "ClassSessionClassId",
                principalTable: "SessionClass",
                principalColumn: "SessionClassId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
