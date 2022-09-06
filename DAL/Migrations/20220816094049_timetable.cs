using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class timetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassTimeTable",
                columns: table => new
                {
                    ClassTimeTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTimeTable", x => x.ClassTimeTableId);
                    table.ForeignKey(
                        name: "FK_ClassTimeTable_ClassLookUp_ClassId",
                        column: x => x.ClassId,
                        principalTable: "ClassLookUp",
                        principalColumn: "ClassLookupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassTimeTableDay",
                columns: table => new
                {
                    ClassTimeTableDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassTimeTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTimeTableDay", x => x.ClassTimeTableDayId);
                    table.ForeignKey(
                        name: "FK_ClassTimeTableDay_ClassTimeTable_ClassTimeTableId",
                        column: x => x.ClassTimeTableId,
                        principalTable: "ClassTimeTable",
                        principalColumn: "ClassTimeTableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassTimeTableDayTime",
                columns: table => new
                {
                    ClassTimeTableDayTimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassTimeTableDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTimeTableDayTime", x => x.ClassTimeTableDayTimeId);
                    table.ForeignKey(
                        name: "FK_ClassTimeTableDayTime_ClassTimeTableDay_ClassTimeTableDayId",
                        column: x => x.ClassTimeTableDayId,
                        principalTable: "ClassTimeTableDay",
                        principalColumn: "ClassTimeTableDayId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassTimeTableDayActivity",
                columns: table => new
                {
                    ClassTimeTableDayActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassTimeTableDayTimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTimeTableDayActivity", x => x.ClassTimeTableDayActivityId);
                    table.ForeignKey(
                        name: "FK_ClassTimeTableDayActivity_ClassTimeTableDayTime_ClassTimeTableDayTimeId",
                        column: x => x.ClassTimeTableDayTimeId,
                        principalTable: "ClassTimeTableDayTime",
                        principalColumn: "ClassTimeTableDayTimeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTable_ClassId",
                table: "ClassTimeTable",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTableDay_ClassTimeTableId",
                table: "ClassTimeTableDay",
                column: "ClassTimeTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTableDayActivity_ClassTimeTableDayTimeId",
                table: "ClassTimeTableDayActivity",
                column: "ClassTimeTableDayTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTableDayTime_ClassTimeTableDayId",
                table: "ClassTimeTableDayTime",
                column: "ClassTimeTableDayId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassTimeTableDayActivity");

            migrationBuilder.DropTable(
                name: "ClassTimeTableDayTime");

            migrationBuilder.DropTable(
                name: "ClassTimeTableDay");

            migrationBuilder.DropTable(
                name: "ClassTimeTable");
        }
    }
}
