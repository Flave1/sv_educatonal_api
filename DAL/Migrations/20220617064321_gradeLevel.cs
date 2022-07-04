using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class gradeLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassGrade");

            migrationBuilder.AddColumn<Guid>(
                name: "GradeGroupId",
                table: "SessionClass",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("64C399E2-D8BE-406A-E391-08DA4AB60955"));

            migrationBuilder.CreateIndex(
                name: "IX_SessionClass_GradeGroupId",
                table: "SessionClass",
                column: "GradeGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionClass_GradeGroup_GradeGroupId",
                table: "SessionClass",
                column: "GradeGroupId",
                principalTable: "GradeGroup",
                principalColumn: "GradeGroupId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionClass_GradeGroup_GradeGroupId",
                table: "SessionClass");

            migrationBuilder.DropIndex(
                name: "IX_SessionClass_GradeGroupId",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "GradeGroupId",
                table: "SessionClass");

            migrationBuilder.CreateTable(
                name: "ClassGrade",
                columns: table => new
                {
                    ClassGradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    GradeGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassGrade", x => x.ClassGradeId);
                    table.ForeignKey(
                        name: "FK_ClassGrade_GradeGroup_GradeGroupId",
                        column: x => x.GradeGroupId,
                        principalTable: "GradeGroup",
                        principalColumn: "GradeGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassGrade_SessionClass_SessionClassId",
                        column: x => x.SessionClassId,
                        principalTable: "SessionClass",
                        principalColumn: "SessionClassId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassGrade_GradeGroupId",
                table: "ClassGrade",
                column: "GradeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGrade_SessionClassId",
                table: "ClassGrade",
                column: "SessionClassId");
        }
    }
}
