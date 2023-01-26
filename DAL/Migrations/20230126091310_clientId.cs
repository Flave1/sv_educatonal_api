using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMP.DAL.Migrations
{
    public partial class clientId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "UsedPin",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "UploadedPin",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "TeacherClassNoteComment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "TeacherClassNote",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Teacher",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "StudentSessionClassHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "StudentNoteComment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "StudentNote",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "StudentContact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "StudentAttendance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "SessionTerm",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SessionTerm",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "SessionTerm",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "SessionTerm",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SessionTerm",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "SessionTerm",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "SessionClassSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "SessionClassGroup",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "SessionClassArchive",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "SessionClass",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Session",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ScoreEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ScoreEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "ScoreEntryHistory",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ScoreEntryHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ScoreEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "ScoreEntryHistory",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ScoreEntry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "RoleActivity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ResultSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "RefreshToken",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RefreshToken",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "RefreshToken",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "RefreshToken",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Parents",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Parents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "Parents",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "NotificationSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "HomeAssessmentFeedBack",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "HomeAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "GradeGroup",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Grade",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ClassTimeTableTimeActivity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ClassTimeTableTimeActivity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "ClassTimeTableTimeActivity",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ClassTimeTableTimeActivity",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ClassTimeTableTimeActivity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "ClassTimeTableTimeActivity",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ClassTimeTableTime",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ClassTimeTableDay",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ClassTimeTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ClassScoreEntry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ClassScoreEntry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "ClassScoreEntry",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ClassScoreEntry",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ClassScoreEntry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "ClassScoreEntry",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ClassRegister",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ClassNote",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ClassLookUp",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ClassAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ClassAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "ClassAssessment",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ClassAssessment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ClassAssessment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "ClassAssessment",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AssessmentScoreRecord",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AssessmentScoreRecord",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "AssessmentScoreRecord",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "AssessmentScoreRecord",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AssessmentScoreRecord",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "AssessmentScoreRecord",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AppActivityParent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AppActivity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Announcement",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AdmissionSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AdmissionNotifications",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "UsedPin");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "UploadedPin");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "TeacherClassNoteComment");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "TeacherClassNote");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "StudentSessionClassHistory");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "StudentNoteComment");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "StudentNote");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "StudentContact");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "StudentAttendance");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "SessionTerm");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SessionTerm");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "SessionTerm");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "SessionTerm");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SessionTerm");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "SessionTerm");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "SessionClassSubject");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "SessionClassGroup");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "SessionClassArchive");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "SessionClass");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ScoreEntryHistory");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ScoreEntryHistory");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ScoreEntryHistory");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ScoreEntryHistory");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ScoreEntryHistory");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "ScoreEntryHistory");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ScoreEntry");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "RoleActivity");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ResultSetting");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "NotificationSetting");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "HomeAssessmentFeedBack");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "HomeAssessment");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "GradeGroup");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Grade");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClassTimeTableTimeActivity");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ClassTimeTableTimeActivity");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ClassTimeTableTimeActivity");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ClassTimeTableTimeActivity");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ClassTimeTableTimeActivity");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "ClassTimeTableTimeActivity");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClassTimeTableTime");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClassTimeTableDay");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClassTimeTable");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClassScoreEntry");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ClassScoreEntry");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ClassScoreEntry");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ClassScoreEntry");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ClassScoreEntry");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "ClassScoreEntry");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClassRegister");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClassNote");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClassLookUp");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ClassAssessment");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ClassAssessment");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ClassAssessment");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ClassAssessment");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ClassAssessment");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "ClassAssessment");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AssessmentScoreRecord");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssessmentScoreRecord");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AssessmentScoreRecord");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "AssessmentScoreRecord");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AssessmentScoreRecord");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "AssessmentScoreRecord");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AppActivityParent");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AppActivity");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Announcement");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AdmissionSettings");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AdmissionNotifications");
        }
    }
}
