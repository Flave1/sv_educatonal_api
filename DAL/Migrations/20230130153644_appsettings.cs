using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class appsettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLayoutSetting",
                columns: table => new
                {
                    AppLayoutSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    scheme = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colorcustomizer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colorinfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colorprimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    schemeDir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sidebarcolor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sidebarType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sidebarActiveStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    navbarstyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    loginTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLayoutSetting", x => x.AppLayoutSettingId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLayoutSetting");
        }
    }
}
