using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class MergedSettingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLayoutSetting");

            migrationBuilder.DropTable(
                name: "NotificationSetting");

            migrationBuilder.DropTable(
                name: "ResultSetting");

            migrationBuilder.RenameColumn(
                name: "TeacherRegNoFormat",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_TeacherRegNoFormat");

            migrationBuilder.RenameColumn(
                name: "StudentRegNoFormat",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_StudentRegNoFormat");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_State");

            migrationBuilder.RenameColumn(
                name: "SchoolType",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_SchoolType");

            migrationBuilder.RenameColumn(
                name: "SchoolName",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_SchoolName");

            migrationBuilder.RenameColumn(
                name: "SchoolAddress",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_SchoolAddress");

            migrationBuilder.RenameColumn(
                name: "SchoolAbbreviation",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_SchoolAbbreviation");

            migrationBuilder.RenameColumn(
                name: "RegNoSeperator",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_RegNoSeperator");

            migrationBuilder.RenameColumn(
                name: "RegNoPosition",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_RegNoPosition");

            migrationBuilder.RenameColumn(
                name: "Photo",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_Photo");

            migrationBuilder.RenameColumn(
                name: "PhoneNo2",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_PhoneNo2");

            migrationBuilder.RenameColumn(
                name: "PhoneNo1",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_PhoneNo1");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_Email");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "SchoolSettings",
                newName: "SCHOOLSETTINGS_Country");

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_Colorcustomizer",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_Colorinfo",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_Colorprimary",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_Navbarstyle",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_Scheme",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_SchemeDir",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_SchoolUrl",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_SidebarActiveStyle",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_SidebarType",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_Sidebarcolor",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "APPLAYOUTSETTINGS_loginTemplate",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATIONSETTINGS_Announcement",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATIONSETTINGS_Assessment",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATIONSETTINGS_ClassManagement",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATIONSETTINGS_Enrollment",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATIONSETTINGS_Permission",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATIONSETTINGS_PublishResult",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATIONSETTINGS_RecoverPassword",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATIONSETTINGS_Session",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATIONSETTINGS_Staff",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RESULTSETTINGS_BatchPrinting",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RESULTSETTINGS_CumulativeResult",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RESULTSETTINGS_PrincipalStample",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RESULTSETTINGS_PromoteAll",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RESULTSETTINGS_SelectedTemplate",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RESULTSETTINGS_ShowNewsletter",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RESULTSETTINGS_ShowPositionOnResult",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_Colorcustomizer",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_Colorinfo",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_Colorprimary",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_Navbarstyle",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_Scheme",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_SchemeDir",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_SchoolUrl",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_SidebarActiveStyle",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_SidebarType",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_Sidebarcolor",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "APPLAYOUTSETTINGS_loginTemplate",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_Announcement",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_Assessment",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_ClassManagement",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_Enrollment",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_Permission",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_PublishResult",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_RecoverPassword",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_Session",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "NOTIFICATIONSETTINGS_Staff",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RESULTSETTINGS_BatchPrinting",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RESULTSETTINGS_CumulativeResult",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RESULTSETTINGS_PrincipalStample",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RESULTSETTINGS_PromoteAll",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RESULTSETTINGS_SelectedTemplate",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RESULTSETTINGS_ShowNewsletter",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RESULTSETTINGS_ShowPositionOnResult",
                table: "SchoolSettings");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_TeacherRegNoFormat",
                table: "SchoolSettings",
                newName: "TeacherRegNoFormat");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_StudentRegNoFormat",
                table: "SchoolSettings",
                newName: "StudentRegNoFormat");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_State",
                table: "SchoolSettings",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_SchoolType",
                table: "SchoolSettings",
                newName: "SchoolType");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_SchoolName",
                table: "SchoolSettings",
                newName: "SchoolName");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_SchoolAddress",
                table: "SchoolSettings",
                newName: "SchoolAddress");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_SchoolAbbreviation",
                table: "SchoolSettings",
                newName: "SchoolAbbreviation");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_RegNoSeperator",
                table: "SchoolSettings",
                newName: "RegNoSeperator");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_RegNoPosition",
                table: "SchoolSettings",
                newName: "RegNoPosition");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_Photo",
                table: "SchoolSettings",
                newName: "Photo");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_PhoneNo2",
                table: "SchoolSettings",
                newName: "PhoneNo2");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_PhoneNo1",
                table: "SchoolSettings",
                newName: "PhoneNo1");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_Email",
                table: "SchoolSettings",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "SCHOOLSETTINGS_Country",
                table: "SchoolSettings",
                newName: "Country");

            migrationBuilder.CreateTable(
                name: "AppLayoutSetting",
                columns: table => new
                {
                    AppLayoutSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    colorcustomizer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colorinfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colorprimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    loginTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    navbarstyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    scheme = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    schemeDir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    schoolUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sidebarActiveStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sidebarType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sidebarcolor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLayoutSetting", x => x.AppLayoutSettingId);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSetting",
                columns: table => new
                {
                    NotificationSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Announcement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Assessment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassManagement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Enrollment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Permission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecoverPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Session = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShouldSendToParentsOnResultPublish = table.Column<bool>(type: "bit", nullable: false),
                    Staff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSetting", x => x.NotificationSettingId);
                });

            migrationBuilder.CreateTable(
                name: "ResultSetting",
                columns: table => new
                {
                    ResultSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchPrinting = table.Column<bool>(type: "bit", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CumulativeResult = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    PrincipalStample = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PromoteAll = table.Column<bool>(type: "bit", nullable: false),
                    SelectedTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowNewsletter = table.Column<bool>(type: "bit", nullable: false),
                    ShowPositionOnResult = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultSetting", x => x.ResultSettingId);
                });
        }
    }
}
