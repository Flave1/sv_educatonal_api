using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class ScoreEntryHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScoreEntryHistory",
                columns: table => new
                {
                    ScoreEntryHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionTermId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionClassId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subjectid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreEntryHistory", x => x.ScoreEntryHistoryId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreEntryHistory");
        }
    }
}
