using BLL;
using BLL.Constants;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.Contracts.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMP.BLL.Services.DashboardServices
{
    public class DashboardService: IDashboardService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public DashboardService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        APIResponse<GetDashboardCount> IDashboardService.GetDashboardCountAsync()
        {
            var res = new APIResponse<GetDashboardCount>();
            var userId = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var userRoles = context.UserRoles.Where(x => x.UserId == userId).AsEnumerable();
            var rolesActivities = context.RoleActivity.Include(x => x.Activity).Where(x => userRoles.Select(r => r.RoleId).Contains(x.RoleId)).AsEnumerable();

            if (!string.IsNullOrEmpty(userId))
            { 
                if(accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = GetDashboardCounts();
                }
                else if(accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    res.Result = GetTeacherDashboardCounts(Guid.Parse(teacherId), rolesActivities.Select(x => x.Activity.Permission).ToList());
                } 
            } 
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        APIResponse<GetStudentshDasboardCount> IDashboardService.GetStudentDashboardCountAsync()
        {
            var res = new APIResponse<GetStudentshDasboardCount>();

            var studentId = accessor.HttpContext.User.FindFirst(e => e.Type == "studentContactId")?.Value;
            res.Result = GetStudentDashboardCounts(Guid.Parse(studentId));
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        private GetDashboardCount GetDashboardCounts()
        {
            var enrolledStudents = context.StudentContact.Count(x => x.Deleted == false && x.EnrollmentStatus == (int)EnrollmentStatus.Enrolled);

            var totalClass = context.SessionClass
                .Include(x => x.Session)
                .Count(x => x.Deleted == false && x.Session.IsActive == true);

            var totalSubject = context.Subject
                .Count(x => x.Deleted == false && x.IsActive == true);

            var totalStaff = context.Teacher
                .Include(x => x.User)
                .Count(x => x.Deleted == false && x.User.Active == true);

            var totalUnusedPins = context.UploadedPin
                .Count(x => x.Deleted == false && !x.UsedPin.Any());

            var totalHomeAssessments = context.HomeAssessment
                .Count(x => x.Deleted == false);

            var totalClassAssessments = context.ClassAssessment
               .Count();

            return new GetDashboardCount
            {
                TotalAssessments = (totalHomeAssessments + totalClassAssessments),
                TotalClass = totalClass,
                TotaldStudent = enrolledStudents,
                TotalStaff = totalStaff,
                TotalSubjects = totalSubject,
                TotalUnusedPins = totalUnusedPins
            };
        }

        private GetDashboardCount GetTeacherDashboardCounts(Guid teacherId, List<string> permissions)
        {
            var totalClass = context.SessionClass
                .Include(x => x.Session)
                .Count(x => x.Deleted == false && x.Session.IsActive == true && x.FormTeacherId == teacherId);

            var totalSubject = context.SessionClassSubject
                .Count(x => x.Deleted == false && x.SubjectTeacherId == teacherId);

            var totalPins = context.UploadedPin
                .Count(x => x.Deleted == false);

            var totalUnusedPins = context.UploadedPin
             .Count(x => x.Deleted == false && !x.UsedPin.Any());

            var totalHomeAssessments = context.HomeAssessment
               .Count(x => x.Deleted == false && x.TeacherId == teacherId);

            var totalClassAssessments = context.ClassAssessment
               .Count(x => x.Scorer == teacherId);

            return new GetDashboardCount
            {
                TotalAssessments = (totalHomeAssessments + totalClassAssessments),
                TotalClass = totalClass,
                TotaldStudent = 0,
                TotalStaff = 0,
                TotalSubjects = totalSubject,
                TotalUnusedPins = totalUnusedPins
            };
        }

        private GetStudentshDasboardCount GetStudentDashboardCounts(Guid studentId)
        {
            var student = context.StudentContact.FirstOrDefault(x => x.StudentContactId == studentId);
            if (student == null)
                throw new ArgumentException("Not found");
            var totalSubject = context.SessionClassSubject
                .Count(x => x.Deleted == false && x.SessionClassId == student.SessionClassId);


            var totalHomeAssessments = context.HomeAssessmentFeedBack
               .Count(x => x.Deleted == false && x.StudentContactId == studentId);

            var totalClassAssessments = context.AssessmentScoreRecord
               .Count(x => x.StudentContactId == studentId);

            var notes = context.StudentNote
               .Count(x => x.StudentContactId == studentId);

            //var lessonNotes = context.TeacherClassNote
            //   .AsEnumerable().Where(x => x.Classes.Split(',').ToList().Select(Guid.Parse).Contains(studentId)).Distinct().Count();

            return new GetStudentshDasboardCount
            {
                TotalAssessments = (totalHomeAssessments + totalClassAssessments),
                TotalSubjects = totalSubject,
                StudentNotes = notes,
                TotaldLessonNotes = 0,
            };
        }
    }

}
