using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class grade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GradeGroup",
                columns: table => new
                {
                    GradeGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeGroup", x => x.GradeGroupId);
                });

            migrationBuilder.CreateTable(
                name: "ClassGrade",
                columns: table => new
                {
                    ClassGradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Grade",
                columns: table => new
                {
                    GradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GradeGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpperLimit = table.Column<int>(type: "int", nullable: false),
                    LowerLimit = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grade", x => x.GradeId);
                    table.ForeignKey(
                        name: "FK_Grade_GradeGroup_GradeGroupId",
                        column: x => x.GradeGroupId,
                        principalTable: "GradeGroup",
                        principalColumn: "GradeGroupId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Grade_GradeGroupId",
                table: "Grade",
                column: "GradeGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassGrade");

            migrationBuilder.DropTable(
                name: "Grade");

            migrationBuilder.DropTable(
                name: "GradeGroup");
        }
    }
}
