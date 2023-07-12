using BLL;
using BLL.Constants;
using DAL;
using DAL.ClassEntities;
using DAL.SessionEntities;
using DAL.SubjectModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.SessionServices;
using SMP.Contracts.Dashboard;
using SMP.Contracts.PortalSettings;
//using SMP.Contracts.PortalSettings;
using SMP.Contracts.Session;
using SMP.DAL.Models.PortalSettings;
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
        private readonly ITermService termService;
        public DashboardService(DataContext context, IHttpContextAccessor accessor, ITermService termService)
        {
            this.context = context;
            this.accessor = accessor;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.termService = termService;
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
                else if(accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
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

            var currentTerm = termService.GetCurrentTerm();
            var enrolledStudents = context.StudentContact.Where(x=>x.ClientId == smsClientId).Count(x => x.Deleted == false && x.EnrollmentStatus == (int)EnrollmentStatus.Enrolled);
            var totalClass = context.SessionClass.Where(x => x.ClientId == smsClientId)
                .Include(x => x.Session)
                .Count(x => x.Deleted == false && x.Session.IsActive == true);

            var totalSubject = context.Subject.Where(x => x.ClientId == smsClientId)
                .Count(x => x.Deleted == false && x.IsActive == true);

            var totalStaff = context.Teacher.Where(x => x.ClientId == smsClientId)
                .Include(x => x.User)
                .Count(x => x.Deleted == false );//&& x.User.Active == true

            //var totalUnusedPins = context.UploadedPin.Where(x => x.ClientId == smsClientId)
            //    .Count(x => x.Deleted == false && !x.UsedPin.Any());

            var totalHomeAssessments = context.HomeAssessment.Where(x => x.ClientId == smsClientId && currentTerm.SessionTermId == x.SessionTermId)
                .Count(x => x.Deleted == false);

            var totalClassAssessments = context.ClassAssessment.Where(x => x.ClientId == smsClientId && currentTerm.SessionTermId == x.SessionTermId)
               .Count();

            return new GetDashboardCount
            {
                TotalAssessments = (totalHomeAssessments + totalClassAssessments),
                TotalClass = totalClass,
                TotaldStudent = enrolledStudents,
                TotalStaff = totalStaff,
                TotalSubjects = totalSubject,
                TotalUnusedPins = 0
            };
        }

        private GetDashboardCount GetTeacherDashboardCounts(Guid teacherId, List<string> permissions)
        {
            var currentTerm = termService.GetCurrentTerm();
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
            var termId = termService.GetCurrentTerm().SessionTermId;
            if (student == null)
                throw new ArgumentException("Not found");
            var totalSubject = context.SessionClassSubject
                .Count(x => x.Deleted == false && x.SessionClassId == student.SessionClassId && x.ClientId == smsClientId);

            Guid allStudentSessionClassGroupId = Guid.Parse("eba102ba-d96c-4920-812a-080c8fdbe767");//DO NOT CHANGE ID PLEASE>>>>
            var totalHomeAssessments = context.HomeAssessment
                .Include(x=>x.SessionClassGroup)
               .Count(x => x.ClientId == smsClientId && x.Deleted == false && x.SessionClassId == student.SessionClassId 
               && x.SessionTermId == termId && x.Status != (int)HomeAssessmentStatus.Saved 
               && ( x.SessionClassGroupId == allStudentSessionClassGroupId
               || x.SessionClassGroup.ListOfStudentContactIds.Contains(studentId.ToString())));

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
            var currentTerm = termService.GetCurrentTerm();
            if(currentTerm is null)
            {
                res.Message.FriendlyMessage = "No active term found";
                return res;
            }
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
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
                {
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
                }
            }
            var allClasses = classesAsASujectTeacher.ToList().Concat(classesAsAFormTeacher.ToList()).Distinct().Select(s => new { ClassName = s.Class.Name, SessionClassId = s.SessionClassId, Classid = s.ClassId }).ToList();

            var classRes = new List<Teacherclasses>();
            for (int i = 0; i < allClasses.Count; i++)
            {
                var result = new Teacherclasses();

                result.SessionClass = allClasses[i].ClassName;
                result.SessionClassId = allClasses[i].SessionClassId;
                result.ClassLookupId = allClasses[i].Classid;

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
                    .Where(x => x?.Classes != null ? (bool)x.Classes.Split(',', StringSplitOptions.None).ToList().Contains(allClasses[i].Classid.ToString()) : false).Distinct().Count();
                classRes.Add(result);
            }
            res.Result = classRes;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
  
        APIResponse<List<ApplicationSetupStatus>> IDashboardService.GetApplicationStatus()
        {
            var res = new APIResponse<List<ApplicationSetupStatus>>();
            res.Result = new List<ApplicationSetupStatus>();
            var currentterm = termService.GetCurrentTerm();
            var session = context.Session.FirstOrDefault(d => d.ClientId == smsClientId && d.IsActive && d.Deleted == false) ?? null;
            var classes = context.ClassLookUp.Where(x => x.ClientId == smsClientId && x.Deleted == false && x.IsActive).ToList();
            var subs = context.Subject.Where(x => x.ClientId == smsClientId && x.Deleted == false && x.IsActive).ToList();
            var setting = context.SchoolSettings.FirstOrDefault(d => d.ClientId == smsClientId && d.Deleted == false);
            res.Result.Add(GetSessionStatus(session));
            res.Result.Add(GetClassStatus(classes, session?.SessionId, currentterm));
            res.Result.Add(GetSubjectsStatus(subs, session?.SessionId, currentterm));
            res.Result.Add(GetSchoolSettingStatus(setting)); 
            res.Result.Add(GetResultSettingStatus(setting));
            res.Result.Add(GetRegNumberStatus(setting));
            res.Result.Add(GetGradeStatus(currentterm));
            res.Result.Add(GetTemplateStatus(setting));
            return res;
        }


        ApplicationSetupStatus GetSessionStatus(Session session)
        {

            const string type = "session";
            if (session == null)
                return new ApplicationSetupStatus { Cleared = false, CompleteionStatus = 0, Setup = type };
            else
            {
                var res = new ApplicationSetupStatus { Cleared = true, CompleteionStatus = 0, Setup = type };
                if(session == null)
                    return res;
                var terms = context.SessionTerm.Where(d => d.ClientId == smsClientId && session.SessionId == d.SessionId).ToList();
                decimal totalCount = terms.Count();
                decimal percentageCompletion = 100 / totalCount;
                for (int i = 0; i < totalCount; i++)
                {
                    res.CompleteionStatus += percentageCompletion;
                }
                if (res.CompleteionStatus != 100)
                    res.Message = "Don't worry! this will turn completed when the school runs all terms;";
                else
                    res.Message = "All seems to be good here";
                return res;
            }
        }

        ApplicationSetupStatus GetClassStatus(List<ClassLookup> clas, Guid? sessionId, SessionTermDto term)
        {

            const string type = "classes";
            if (!clas.Any())
                return new ApplicationSetupStatus { Cleared = false, CompleteionStatus = 0, Setup = type };
            else
            {
                var res = new ApplicationSetupStatus { Cleared = true, CompleteionStatus = 0, Setup = type };
                if (sessionId == null)
                    return res;
                var sClasses = context.SessionClass.Include(c => c.Students)
                    .Where(d => d.ClientId == smsClientId && sessionId == d.SessionId 
                    && d.SessionTermId == term.SessionTermId
                    && clas.Select(f => f.ClassLookupId).Contains(d.ClassId)).ToList();
                decimal totalCount = sClasses.Count();
                if(totalCount == 0 )
                    return new ApplicationSetupStatus { Cleared = false, CompleteionStatus = 0, Setup = type };
                decimal percentageCompletion = 100 / totalCount;
                for (int i = 0; i < totalCount; i++)
                {
                    if (sClasses[i].Students.Count > 0)
                    {
                        res.CompleteionStatus += percentageCompletion;
                    }
                }
                if (res.CompleteionStatus != 100)
                    res.Message = "It seems some classes have no students in them. Please check";
                else
                    res.Message = "All is good here";
                return res;
            }
        }

        ApplicationSetupStatus GetSubjectsStatus(List<Subject> subs, Guid? sessionId, SessionTermDto term)
        {

            const string type = "subjects";
            if (!subs.Any())
                return new ApplicationSetupStatus { Cleared = false, CompleteionStatus = 0, Setup = type };
            else
            {
                var res = new ApplicationSetupStatus { Cleared = true, CompleteionStatus = 0, Setup = type };
                if (sessionId == null)
                    return res;
                var sessioClasSubjs = context.SessionClassSubject.Include(d => d.SessionClass)
                    .Where(d => d.ClientId == smsClientId && term.SessionTermId == d.SessionClass.SessionTermId && subs.Select(f => f.SubjectId).Contains(d.SubjectId)).ToList();
                var classesWithSubjs = sessioClasSubjs.GroupBy(d => d.SessionClassId).Select(f => f.First()).ToList();
                var allClasess = context.SessionClass.Where(d => d.Deleted == false && d.SessionTermId == term.SessionTermId).ToList();
    
                if (allClasess.Count != classesWithSubjs.Count)
                {
                    res.Message = "It seems some classes have no subjects in them. Please check";
                    return res;
                }
                res.CompleteionStatus = 100;
                if (res.CompleteionStatus != 100)
                {
                    res.Message = "It seems some classes have no subjects in them. Please check";
                    res.CompleteionStatus = 50;
                }
                else
                {
                    res.Message = "All is good here";
                    res.CompleteionStatus = 100;
                }

                
                return res;
            }
        }

        ApplicationSetupStatus GetSchoolSettingStatus(SchoolSetting setting)
        {

            const string type = "schoolsetting";
            var res = new ApplicationSetupStatus { Cleared = true, CompleteionStatus = 0, Setup = type };

            decimal totalCount = 10;
            decimal percentageCompletion = 100 / totalCount;
            if(!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_SchoolName))
                res.CompleteionStatus += percentageCompletion;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_SchoolAbbreviation))
                res.CompleteionStatus += percentageCompletion;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_Email))
                res.CompleteionStatus += percentageCompletion;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_Country))
                res.CompleteionStatus += percentageCompletion;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_State))
                res.CompleteionStatus += percentageCompletion;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_SchoolAddress))
                res.CompleteionStatus += percentageCompletion;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_PhoneNo1))
                res.CompleteionStatus += percentageCompletion;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_PhoneNo2))
                res.CompleteionStatus += percentageCompletion;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_SchoolType))
                res.CompleteionStatus += percentageCompletion;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_Photo))
                res.CompleteionStatus += percentageCompletion;

            if(res.CompleteionStatus != 100)
                res.Message = "Ensure all fields in school setting are provided";
            else
                res.Message = "All seems to be good here";
            return res;

        }

        ApplicationSetupStatus GetResultSettingStatus(SchoolSetting setting)
        {

            const string type = "resultsetting";
            var res = new ApplicationSetupStatus { Cleared = true, CompleteionStatus = 0, Setup = type };

            decimal totalCount = 1;
            decimal percentageCompletion = 100 / totalCount;
            if (!string.IsNullOrEmpty(setting.RESULTSETTINGS_PrincipalStample))
                res.CompleteionStatus = 100;


            if (res.CompleteionStatus != 100)
                res.Message = "Ensure result settings are well done to be able to print result effectively";
            else
                res.Message = "All seems to be good here";
            return res;

        }
        ApplicationSetupStatus GetRegNumberStatus(SchoolSetting setting)
        {

            const string type = "registrationnumber";
            var res = new ApplicationSetupStatus { Cleared = false, CompleteionStatus = 0, Setup = type };

            decimal totalCount = 3;
            decimal percentageCompletion = 100 / totalCount;
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_StudentRegNoFormat))
            {
                res.CompleteionStatus += percentageCompletion;
                res.Cleared = true;
            }
            if (setting.SCHOOLSETTINGS_RegNoPosition != 0)
            {
                res.CompleteionStatus += percentageCompletion;
                res.Cleared = true;
            }
            if (!string.IsNullOrEmpty(setting.SCHOOLSETTINGS_RegNoSeperator))
            {
                res.CompleteionStatus += percentageCompletion;
                res.Cleared = true;
            }

            if (res.CompleteionStatus != 100)
                res.Message = "Please ENSURE Students registration number is set up before adding students";
            else
                res.Message = "All seems to be good here";

            return res;
        }

        ApplicationSetupStatus GetGradeStatus(SessionTermDto term)
        {

            const string type = "grade";
            var gradesetting = context.Grade.FirstOrDefault(c => c.ClientId == smsClientId && c.Deleted == false);
            var res = new ApplicationSetupStatus { Cleared = false, CompleteionStatus = 0, Setup = type };
            if (gradesetting == null)
                return res;

            var activeClasses = context.SessionClass
                .Include(s => s.Class).ThenInclude(d => d.GradeLevel)
                .Where(c => c.ClientId == smsClientId && c.SessionTermId == term.SessionTermId).ToList();

            decimal totalCount = activeClasses.Count;
            if(totalCount == 0) {

                res.Message = "No Class found. Please check";
                return res; 
            }
            decimal percentageCompletion = 100 / totalCount;
            for (int i = 0; i < totalCount; i++)
            {
                if (activeClasses[i].Class.GradeLevel == null)
                    continue;
                else
                {
                    res.CompleteionStatus += percentageCompletion;
                    res.Cleared = true;
                }
            
            }
            if (res.CompleteionStatus != 100)
                res.Message = "Please ENSURE all classes are graded accordingly e.g Junior, Senior, Primary, Cretch";
            else
                res.Message = "All seems to be good here";
            return res;

        }

        ApplicationSetupStatus GetTemplateStatus(SchoolSetting setting)
        {

            const string type = "resulttemplate";
            var res = new ApplicationSetupStatus { Cleared = false, CompleteionStatus = 0, Setup = type };

            decimal totalCount = 1;
            decimal percentageCompletion = 100 / totalCount;
            if (!string.IsNullOrEmpty(setting.RESULTSETTINGS_SelectedTemplate))
            {
                res.CompleteionStatus += percentageCompletion;
                res.Cleared = true;
            }

            if (res.CompleteionStatus != 100)
                res.Message = "Please ENSURE result template is selected to be able to print result effectively";
            else
                res.Message = "All seems to be good here";

            return res;
        }
    }
}
