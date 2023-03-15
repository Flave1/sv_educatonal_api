using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class TimetableType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamTimeTableTimeActivity");

            migrationBuilder.DropTable(
                name: "ExamTimeTableDay");

            migrationBuilder.DropTable(
                name: "ExamTimeTableTime");

            migrationBuilder.DropTable(
                name: "ExamTimeTable");

            migrationBuilder.AddColumn<int>(
                name: "TimetableType",
                table: "ClassTimeTable",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimetableType",
                table: "ClassTimeTable");

            migrationBuilder.CreateTable(
                name: "ExamTimeTable",
                columns: table => new
                {
                    ExamTimeTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTimeTable", x => x.ExamTimeTableId);
                    table.ForeignKey(
                        name: "FK_ExamTimeTable_ClassLookUp_ClassId",
                        column: x => x.ClassId,
                        principalTable: "ClassLookUp",
                        principalColumn: "ClassLookupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamTimeTableDay",
                columns: table => new
                {
                    ExamTimeTableDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    ExamTimeTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTimeTableDay", x => x.ExamTimeTableDayId);
                    table.ForeignKey(
                        name: "FK_ExamTimeTableDay_ExamTimeTable_ExamTimeTableId",
                        column: x => x.ExamTimeTableId,
                        principalTable: "ExamTimeTable",
                        principalColumn: "ExamTimeTableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamTimeTableTime",
                columns: table => new
                {
                    ExamTimeTableTimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    End = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamTimeTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTimeTableTime", x => x.ExamTimeTableTimeId);
                    table.ForeignKey(
                        name: "FK_ExamTimeTableTime_ExamTimeTable_ExamTimeTableId",
                        column: x => x.ExamTimeTableId,
                        principalTable: "ExamTimeTable",
                        principalColumn: "ExamTimeTableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamTimeTableTimeActivity",
                columns: table => new
                {
                    ExamTimeTableTimeActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    ExamTimeTableDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExamTimeTableTimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTimeTableTimeActivity", x => x.ExamTimeTableTimeActivityId);
                    table.ForeignKey(
                        name: "FK_ExamTimeTableTimeActivity_ExamTimeTableDay_ExamTimeTableDayId",
                        column: x => x.ExamTimeTableDayId,
                        principalTable: "ExamTimeTableDay",
                        principalColumn: "ExamTimeTableDayId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamTimeTableTimeActivity_ExamTimeTableTime_ExamTimeTableTimeId",
                        column: x => x.ExamTimeTableTimeId,
                        principalTable: "ExamTimeTableTime",
                        principalColumn: "ExamTimeTableTimeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamTimeTable_ClassId",
                table: "ExamTimeTable",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTimeTableDay_ExamTimeTableId",
                table: "ExamTimeTableDay",
                column: "ExamTimeTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTimeTableTime_ExamTimeTableId",
                table: "ExamTimeTableTime",
                column: "ExamTimeTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTimeTableTimeActivity_ExamTimeTableDayId",
                table: "ExamTimeTableTimeActivity",
                column: "ExamTimeTableDayId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTimeTableTimeActivity_ExamTimeTableTimeId",
                table: "ExamTimeTableTimeActivity",
                column: "ExamTimeTableTimeId");
        }
    }
}
