using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class notificationUpdatea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyByEmail",
                table: "NotificationSetting");

            migrationBuilder.RenameColumn(
                name: "NotifyBySms",
                table: "NotificationSetting",
                newName: "ShouldSendToParentsOnResultPublish");

            migrationBuilder.AddColumn<string>(
                name: "Announcement",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Assessment",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassManagement",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Enrollment",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Permission",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublishResult",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecoverPassword",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Session",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Staff",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Announcement",
                table: "NotificationSetting");

            migrationBuilder.DropColumn(
                name: "Assessment",
                table: "NotificationSetting");

            migrationBuilder.DropColumn(
                name: "ClassManagement",
                table: "NotificationSetting");

            migrationBuilder.DropColumn(
                name: "Enrollment",
                table: "NotificationSetting");

            migrationBuilder.DropColumn(
                name: "Permission",
                table: "NotificationSetting");

            migrationBuilder.DropColumn(
                name: "PublishResult",
                table: "NotificationSetting");

            migrationBuilder.DropColumn(
                name: "RecoverPassword",
                table: "NotificationSetting");

            migrationBuilder.DropColumn(
                name: "Session",
                table: "NotificationSetting");

            migrationBuilder.DropColumn(
                name: "Staff",
                table: "NotificationSetting");

            migrationBuilder.RenameColumn(
                name: "ShouldSendToParentsOnResultPublish",
                table: "NotificationSetting",
                newName: "NotifyBySms");

            migrationBuilder.AddColumn<bool>(
                name: "NotifyByEmail",
                table: "NotificationSetting",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
