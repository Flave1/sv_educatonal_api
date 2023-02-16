using BLL;
using BLL.Constants;
using DAL;
using DAL.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.Contracts.Dashboard;
using SMP.DAL.Models.ClassEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMP.BLL.Services.DashboardServices
{
    public class DashboardService: IDashboardService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly string smsClientId;
        public DashboardService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        APIResponse<GetDashboardCount> IDashboardService.GetDashboardCountAsync()
        {
            var res = new APIResponse<GetDashboardCount>();
            var userId = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var userRoles = context.UserRoles.Where(x => x.UserId == userId).AsEnumerable();
            var rolesActivities = context.RoleActivity.Where(x=>x.ClientId == smsClientId)
                .Include(x => x.Activity).Where(x => userRoles.Select(r => r.RoleId).Contains(x.RoleId)).AsEnumerable();

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
            var enrolledStudents = context.StudentContact.Where(x=>x.ClientId == smsClientId).Count(x => x.Deleted == false && x.EnrollmentStatus == (int)EnrollmentStatus.Enrolled);
            var totalClass = context.SessionClass.Where(x => x.ClientId == smsClientId)
                .Include(x => x.Session)
                .Count(x => x.Deleted == false && x.Session.IsActive == true);

            var totalSubject = context.Subject.Where(x => x.ClientId == smsClientId)
                .Count(x => x.Deleted == false && x.IsActive == true);

            var totalStaff = context.Teacher.Where(x => x.ClientId == smsClientId)
                .Include(x => x.User)
                .Count(x => x.Deleted == false && x.User.Active == true);

            var totalUnusedPins = context.UploadedPin.Where(x => x.ClientId == smsClientId)
                .Count(x => x.Deleted == false && !x.UsedPin.Any());

            var totalHomeAssessments = context.HomeAssessment.Where(x => x.ClientId == smsClientId)
                .Count(x => x.Deleted == false);

            var totalClassAssessments = context.ClassAssessment.Where(x => x.ClientId == smsClientId)
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
            var currentTerm = context.SessionTerm.FirstOrDefault(x => x.IsActive && x.ClientId == smsClientId);
            var totalClass = context.SessionClass.Where(x => x.ClientId == smsClientId)
                .Include(x => x.Session)
                .Count(x => x.Deleted == false && x.Session.IsActive == true && x.FormTeacherId == teacherId);


            var totalSubject = context.SessionClassSubject.Where(x => x.ClientId == smsClientId)
                .Include(x => x.Subject)
                .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                .Where(x => x.SessionClass.Session.IsActive && x.SubjectTeacherId == teacherId)
                .AsEnumerable()
                .GroupBy(x => x.SubjectId).Select(x => x.First())
                .Count(x => x.Deleted == false && x.Subject.IsActive == true);



            var totalHomeAssessments = context.HomeAssessment.Where(x => x.ClientId == smsClientId)
                .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                .Where(x => x.SessionClass.Session.IsActive && currentTerm.SessionTermId == x.SessionTermId)
                .Count(x => x.Deleted == false && x.TeacherId == teacherId);

            var totalClassAssessments = context.ClassAssessment.Where(x => x.ClientId == smsClientId)
                .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                .Where(x => x.SessionClass.Session.IsActive && currentTerm.SessionTermId == x.SessionTermId)
               .Count( x => x.Scorer == teacherId);

            return new GetDashboardCount
            {
                TotalAssessments = (totalHomeAssessments + totalClassAssessments),
                TotalClass = totalClass,
                TotaldStudent = 0,
                TotalStaff = 0,
                TotalSubjects = totalSubject,
                TotalUnusedPins = 0
            };
        }

        private GetStudentshDasboardCount GetStudentDashboardCounts(Guid studentId)
        {
            var student = context.StudentContact.Include(x => x.SessionClass).FirstOrDefault(x => x.StudentContactId == studentId);
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive && x.ClientId == smsClientId).SessionTermId;
            if (student == null)
                throw new ArgumentException("Not found");
            var totalSubject = context.SessionClassSubject
                .Count(x => x.Deleted == false && x.SessionClassId == student.SessionClassId && x.ClientId == smsClientId);

            Guid allStudentSessionClassGroupId = Guid.Parse("eba102ba-d96c-4920-812a-080c8fdbe767");//DO NOT CHANGE ID PLEASE>>>>
            var totalHomeAssessments = context.HomeAssessment.Include(x=>x.SessionClassGroup)
               .Count(x => x.ClientId == smsClientId && x.Deleted == false && x.SessionClassId == student.SessionClassId 
               && x.SessionTermId == termId && x.Status != (int)HomeAssessmentStatus.Saved 
               && ( x.SessionClassGroupId == allStudentSessionClassGroupId || x.SessionClassGroup.ListOfStudentContactIds.Contains(studentId.ToString())));

            var totalClassAssessments = context.ClassAssessment
               .Count(x => x.ClientId == smsClientId && x.SessionClassId == student.SessionClassId && x.SessionTermId == termId);

            var notes = context.StudentNote
               .Count(x => x.ClientId == smsClientId && x.StudentContactId == studentId && x.Deleted == false  && x.SessionTermId == termId);


            var classNotes = context.TeacherClassNote.Where(x => x.ClientId == smsClientId && x.Deleted == false && x.SessionTermId == termId).Include(x=>x.ClassNote)
                .Where(x=> context.ClassNote.Where(x=>x.AprrovalStatus != (int)NoteApprovalStatus.Saved).Select(x=>x.ClassNoteId).Contains(x.ClassNoteId))
                .AsEnumerable()
               .Count(x => !string.IsNullOrEmpty(x.Classes) ? x.Classes.Split(',', StringSplitOptions.None).ToList().Contains(student.SessionClass.ClassId.ToString()) : false);

            return new GetStudentshDasboardCount
            {
                TotalAssessments = (totalHomeAssessments + totalClassAssessments),
                TotalSubjects = totalSubject,
                StudentNotes = notes,
                TotaldLessonNotes = classNotes,
            };
        }

        APIResponse<List<Teacherclasses>> IDashboardService.GetTeacherMobileDashboardCountAsync()
        {
            var res = new APIResponse<List<Teacherclasses>>();
            var userId = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var userRoles = context.UserRoles.Where(x => x.UserId == userId).AsEnumerable();
            var rolesActivities = context.RoleActivity.Where(x=>x.ClientId == smsClientId).Include(x => x.Activity).Where(x => userRoles.Select(r => r.RoleId).Contains(x.RoleId)).AsEnumerable();
            IQueryable<SessionClass> classesAsASujectTeacher = null;
            IQueryable<SessionClass> classesAsAFormTeacher = null;
            var currentTerm = context.SessionTerm.FirstOrDefault(x => x.IsActive && x.ClientId == smsClientId);
            if (!string.IsNullOrEmpty(userId))
            {
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    classesAsASujectTeacher = context.SessionClass
                        .Where(e => e.ClientId == smsClientId && e.Session.IsActive == true && e.Deleted == false)
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name);

                    classesAsAFormTeacher = context.SessionClass
                        .Where(e => e.ClientId == smsClientId && e.Session.IsActive == true && e.Deleted == false)
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name);

                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))

                     classesAsASujectTeacher = context.SessionClass
                        .Where(e => e.ClientId == smsClientId && e.Session.IsActive == true && e.Deleted == false && e.SessionClassSubjects
                        .Any(d => d.SubjectTeacherId == Guid.Parse(teacherId)))
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name);

                    classesAsAFormTeacher = context.SessionClass
                        .Where(e => e.ClientId == smsClientId && e.Session.IsActive == true && e.Deleted == false && e.FormTeacherId == Guid.Parse(teacherId))
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name);

                   
                    //res.Result = GetTeacherDashboardCounts(Guid.Parse(teacherId), rolesActivities.Select(x => x.Activity.Permission).ToList());
                }
                var allClasses = classesAsASujectTeacher.ToList().Concat(classesAsAFormTeacher.ToList()).Distinct().Select(s => new { ClassName = s.Class.Name, SessionClassId = s.SessionClassId, Classid = s.ClassId }).ToList();

                var classRes = new List<Teacherclasses>();
                for (int i = 0; i < allClasses.Count; i++)
                {
                    var result = new Teacherclasses();

                    result.SessionClass = allClasses[i].ClassName;
                    result.SessionClassId = allClasses[i].SessionClassId;

                    var totalHomeAssessments = context.HomeAssessment
                        .Where(x => x.ClientId == smsClientId && x.SessionClassId == allClasses[i].SessionClassId)
                        .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                        .Where(x => x.SessionClass.Session.IsActive && currentTerm.SessionTermId == x.SessionTermId)
                        .Count(x => x.Deleted == false && x.TeacherId == Guid.Parse(teacherId));

                    var totalClassAssessments = context.ClassAssessment
                        .Where(x => x.ClientId == smsClientId && x.SessionClassId == allClasses[i].SessionClassId)
                        .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                        .Where(x => x.SessionClass.Session.IsActive && currentTerm.SessionTermId == x.SessionTermId)
                       .Count(x => x.Scorer == Guid.Parse(teacherId));

                    result.AssessmentCount = (totalHomeAssessments + totalClassAssessments);

                    result.StudentCounts = context.StudentContact.Where(s => s.SessionClassId == allClasses[i].SessionClassId && s.EnrollmentStatus == (int)EnrollmentStatus.Enrolled).Count();

                result.StudentNoteCount = context.TeacherClassNote
                    .Where(x => x.ClientId == smsClientId && x.SessionTermId == currentTerm.SessionTermId).AsEnumerable()
                    .Where(x => x.Classes.Split(',', StringSplitOptions.None).ToList().Contains(allClasses[i].Classid.ToString())).Distinct().Count();
                classRes.Add(result);
            }

            res.Result = classRes;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
    }

}
