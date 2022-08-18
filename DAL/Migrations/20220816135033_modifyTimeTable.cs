using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class modifyTimeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassTimeTableDayActivity");

            migrationBuilder.DropTable(
                name: "ClassTimeTableDayTime");

            migrationBuilder.CreateTable(
                name: "ClassTimeTableTime",
                columns: table => new
                {
                    ClassTimeTableTimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    End = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassTimeTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassTimeTableDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTimeTableTime", x => x.ClassTimeTableTimeId);
                    table.ForeignKey(
                        name: "FK_ClassTimeTableTime_ClassTimeTable_ClassTimeTableId",
                        column: x => x.ClassTimeTableId,
                        principalTable: "ClassTimeTable",
                        principalColumn: "ClassTimeTableId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassTimeTableTime_ClassTimeTableDay_ClassTimeTableDayId",
                        column: x => x.ClassTimeTableDayId,
                        principalTable: "ClassTimeTableDay",
                        principalColumn: "ClassTimeTableDayId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassTimeTableTimeActivity",
                columns: table => new
                {
                    ClassTimeTableTimeActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassTimeTableTimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTimeTableTimeActivity", x => x.ClassTimeTableTimeActivityId);
                    table.ForeignKey(
                        name: "FK_ClassTimeTableTimeActivity_ClassTimeTableTime_ClassTimeTableTimeId",
                        column: x => x.ClassTimeTableTimeId,
                        principalTable: "ClassTimeTableTime",
                        principalColumn: "ClassTimeTableTimeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTableTime_ClassTimeTableDayId",
                table: "ClassTimeTableTime",
                column: "ClassTimeTableDayId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTableTime_ClassTimeTableId",
                table: "ClassTimeTableTime",
                column: "ClassTimeTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTableTimeActivity_ClassTimeTableTimeId",
                table: "ClassTimeTableTimeActivity",
                column: "ClassTimeTableTimeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassTimeTableTimeActivity");

            migrationBuilder.DropTable(
                name: "ClassTimeTableTime");

            migrationBuilder.CreateTable(
                name: "ClassTimeTableDayTime",
                columns: table => new
                {
                    ClassTimeTableDayTimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassTimeTableDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_ClassTimeTableDayActivity_ClassTimeTableDayTimeId",
                table: "ClassTimeTableDayActivity",
                column: "ClassTimeTableDayTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTimeTableDayTime_ClassTimeTableDayId",
                table: "ClassTimeTableDayTime",
                column: "ClassTimeTableDayId");
        }
    }
}
