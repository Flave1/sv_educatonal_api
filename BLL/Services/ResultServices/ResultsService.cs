using BLL;
using BLL.ClassServices;
using BLL.Constants;
using BLL.Filter;
using BLL.LoggerService;
using BLL.StudentServices;
using BLL.Wrappers;
using Contracts.Class;
using Contracts.Session;
using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using DAL.SubjectModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.Ocsp;
using Polly;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.SessionServices;
using SMP.BLL.Utilities;
using SMP.Contracts.PinManagement;
using SMP.Contracts.ResultModels;
using SMP.Contracts.Session;
using SMP.DAL.Migrations;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.ResultModels;
using SMP.DAL.Models.SessionEntities;
using SMP.DAL.Models.StudentImformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ResultServices
{
    public class ResultsService: IResultsService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        private readonly IUtilitiesService utilitiesService;
        private readonly IStudentService studentService;
        private readonly IClassGroupService classGroupService;
        private readonly IScoreEntryHistoryService scoreEntryService;
        private readonly ITermService termService;

        public ResultsService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService, ILoggerService loggerService, IUtilitiesService utilitiesService, IStudentService studentService, IClassGroupService classGroupService, IScoreEntryHistoryService scoreEntryService, ITermService termService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.utilitiesService = utilitiesService;
            this.studentService = studentService;
            this.classGroupService = classGroupService;
            this.scoreEntryService = scoreEntryService;
            this.termService = termService;
        }

        async Task<APIResponse<List<GetClasses>>> IResultsService.GetCurrentStaffClassesAsync()
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value; 
            var res = new APIResponse<List<GetClasses>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = await context.SessionClass.Where(x=>x.ClientId == smsClientId && x.Deleted == false)
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true).Select(s => new GetClasses(s)).ToListAsync();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
                //GET TEACHER CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                   var classesAsASujectTeacher = context.SessionClass.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true && e.SessionClassSubjects
                        .Any(d => d.SubjectTeacherId == Guid.Parse(teacherId)));

                    var classesAsAFormTeacher = context.SessionClass.Where(x => x.ClientId == smsClientId && x.Deleted == false && x.FormTeacherId == Guid.Parse(teacherId))
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true);


                    res.Result = classesAsASujectTeacher.ToList().Concat(classesAsAFormTeacher.ToList()).Distinct().Select(s => new GetClasses(s)).ToList();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }

            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetClasses>>> IResultsService.GetFormTeacherClassesAsync()
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var res = new APIResponse<List<GetClasses>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = await context.SessionClass.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true).Select(s => new GetClasses(s)).ToListAsync();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
                //GET TEACHER CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                //    var classesAsASujectTeacher = context.SessionClass
                //         .Include(s => s.Class)
                //         .Include(s => s.Session)
                //         .Include(s => s.SessionClassSubjects)
                //         .OrderBy(s => s.Class.Name)
                //         .Where(e => e.Session.IsActive == true && e.Deleted == false && e.SessionClassSubjects
                //         .Any(d => d.SubjectTeacherId == Guid.Parse(teacherId)));

                    var classesAsAFormTeacher = context.SessionClass.Where(x => x.ClientId == smsClientId && x.Deleted == false && x.FormTeacherId == Guid.Parse(teacherId))
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true);


                    res.Result = classesAsAFormTeacher.ToList().Distinct().Select(s => new GetClasses(s)).ToList();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }

            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetClassSubjects>>> IResultsService.GetCurrentStaffClassSubjectsAsync(Guid sessionClassId)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId= accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var res = new APIResponse<List<GetClassSubjects>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = await context.SessionClassSubject.Where(x=>x.ClientId == smsClientId && x.SessionClassId == sessionClassId)
                        .Include(d => d.Subject)
                        .Where(e => e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s)).ToListAsync();

                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }

                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    var subjectTeacherSubjects = context.SessionClassSubject.Where(x=>x.ClientId == smsClientId && x.SubjectTeacherId == Guid.Parse(teacherId)
                        && x.SessionClassId == sessionClassId)
                        .Include(d => d.Subject)
                        .Where(e => e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s));

                    var formTeacherSubjects = context.SessionClassSubject.Where(x=>x.ClientId == smsClientId && x.SessionClassId == sessionClassId
                        && x.SessionClass.FormTeacherId == Guid.Parse(teacherId))
                        .Include(d => d.Subject)
                        .Include(d => d.SessionClass)
                        .Where(e => e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s));

                    res.Result = subjectTeacherSubjects.AsEnumerable().Concat(formTeacherSubjects.AsEnumerable()).Distinct().GroupBy(x => x.SubjectId).Select(x => x.First()).ToList();

                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetClassSubjects>>> IResultsService.GetCurrentStaffClassSubjects2Async(Guid classId, Guid sessionClassId)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var res = new APIResponse<List<GetClassSubjects>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = await context.SessionClassSubject.Where(x => x.ClientId == smsClientId && x.SessionClassId == sessionClassId)
                        .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                        .Include(d => d.Subject)
                        .Where(e => e.SessionClass.ClassId == classId 
                        && e.SessionClass.Session.IsActive == true 
                        && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s)).ToListAsync();

                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }

                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    var subjectTeacherSubjects = context.SessionClassSubject.Where(x=>x.ClientId == smsClientId && x.SubjectTeacherId == Guid.Parse(teacherId) 
                    && x.SessionClassId == sessionClassId)
                         .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                        .Include(d => d.Subject)
                        .Where(e =>  e.SessionClass.ClassId == classId 
                        && e.SessionClass.Session.IsActive == true
                        && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s));

                    var formTeacherSubjects = context.SessionClassSubject.Where(x=>x.ClientId == smsClientId && x.SessionClassId == sessionClassId)
                        .Include(d => d.Subject)
                        .Include(d => d.SessionClass).ThenInclude(x => x.Session)
                        .Where(e => e.SessionClass.ClassId == classId 
                        && e.SessionClass.FormTeacherId == Guid.Parse(teacherId) 
                        && e.SessionClass.Session.IsActive == true 
                        && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s));

                    res.Result = subjectTeacherSubjects.AsEnumerable().Concat(formTeacherSubjects.AsEnumerable()).Distinct().GroupBy(x => x.SubjectId).Select(x => x.First()).ToList();

                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PagedResponse<GetClassScoreEntry>>> IResultsService.GetClassEntryAsync(Guid sessionClassId, Guid subjectId, PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<GetClassScoreEntry>>();
            SessionClass sessClass = GetSessionClass(sessionClassId).FirstOrDefault();
            if(sessClass is null)
            {
                res.Message.FriendlyMessage = "Class not found";
                return res;
            }
            string regNoFormat = await GetRegNumberFormat();
            GetClassScoreEntry result = await GetClassScoreEntryHeader(subjectId, sessClass);

            if (result is null)
            {
                res.Message.FriendlyMessage = "Subject not found";
                return res;
            }

            if (sessClass.Session.IsActive)
            {
                var currentTerm = termService.GetCurrentTerm();
                if (result != null)
                {
                    var query = context.StudentContact.Where(x=>x.ClientId == smsClientId && x.SessionClassId == sessionClassId && x.EnrollmentStatus == (int)EnrollmentStatus.Enrolled);
                    var sts = await paginationService.GetPagedResult(query, filter).ToListAsync();
                   
                    foreach (var student in sts)
                    {
                        var scoreEntrySheet = new ScoreEntrySheet();
                        var scoreEntry = scoreEntryService.GetScoreEntry(currentTerm.SessionTermId, student.StudentContactId, subjectId);
                        scoreEntrySheet.AssessmentScore = scoreEntry?.AssessmentScore ?? 0;
                        scoreEntrySheet.ExamsScore = scoreEntry?.ExamScore ?? 0;
                        scoreEntrySheet.RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
                        scoreEntrySheet.StudentContactId = student.StudentContactId.ToString();
                        scoreEntrySheet.StudentName = student.FirstName + " " + student.LastName + " " + student.MiddleName;
                        scoreEntrySheet.IsOffered = scoreEntry?.IsOffered ?? false;
                        scoreEntrySheet.IsSaved = scoreEntry?.IsSaved ?? false;
                        scoreEntrySheet.TotalScore = scoreEntry?.ExamScore ?? 0 + scoreEntry?.AssessmentScore ?? 0;
                        result.ClassScoreEntries.Add(scoreEntrySheet);
                    }

                    var totaltRecord = query.Count();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
                }
            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PagedResponse<PreviewClassScoreEntry>>> IResultsService.PreviewClassScoreEntry(Guid sessionClassId, Guid subjectId, PaginationFilter filter)
        {
            filter.PageSize = 50;
            var res = new APIResponse<PagedResponse<PreviewClassScoreEntry>>();
            var regNoFormat = await GetRegNumberFormat();
            SessionTermDto currentTerm = termService.GetCurrentTerm();
            var result = await GetClassScoreEntryPreviewHeader(sessionClassId, subjectId, regNoFormat);
            if (result is not null)
            {
                SessionClassSubject subject = await GetSessionClassSubject(subjectId, sessionClassId);
                if (subject is not null)
                    result.SubjectTeacher = subject.SubjectTeacher.FirstName + " " + subject.SubjectTeacher.LastName;

                var classGrades = context.GradeGroup.Where(x => x.ClientId == smsClientId).Include(x => x.Grades).Where(x => x.Classes.Select(s => s.ClassLookupId).Contains(result.ClassLookupId)).FirstOrDefault();

                var query = scoreEntryService.GetScoreEntriesQuery(subjectId, sessionClassId, currentTerm.SessionTermId)
                    .Include(x => x.SessionTerm)
                    .Include(x => x.StudentContact)
                    .Select(d => new ScoreEntrySheet(d, regNoFormat, classGrades));

                result.ClassScoreEntries = await paginationService.GetPagedResult(query, filter).ToListAsync();
                var totaltRecord = query.Count();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<ScoreEntry>> IResultsService.UpdateExamScore(UpdateScore request)
        {
            var res = new APIResponse<ScoreEntry>();
            try
            {
                SessionTermDto selectedTerm = null;
                if (!string.IsNullOrEmpty(request.SessionTermId))
                    selectedTerm = termService.SelectTerm(Guid.Parse(request.SessionTermId));
                else
                {
                    selectedTerm = termService.GetCurrentTerm();
                    request.SessionTermId = selectedTerm.SessionTermId.ToString();
                }

                if (selectedTerm != null)
                {
                    var studentEntry = scoreEntryService.GetScoreEntry(selectedTerm.SessionTermId, Guid.Parse(request.StudentContactId), Guid.Parse(request.SubjectId));

                    if (studentEntry != null)
                        studentEntry = await scoreEntryService.UpdateScoreEntryForExam(request, studentEntry);
                    else
                        studentEntry =  await scoreEntryService.CreateScoreEntryForExam(request);

                    res.Result = studentEntry;
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Successful";
                }
                else
                {
                    var errorMsg = "System facing some technical issues with running term";
                    loggerService.Debug(errorMsg + "clentId:"+ smsClientId);
                    res.Message.FriendlyMessage = errorMsg;
                }
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex?.Message;
                return res;
            }
        }

        async Task<APIResponse<ScoreEntry>> IResultsService.UpdateAssessmentScore(UpdateScore request)
        {
            var res = new APIResponse<ScoreEntry>();
            try
            {
                SessionTermDto selectedTerm = null;
                if (!string.IsNullOrEmpty(request.SessionTermId))
                    selectedTerm = termService.SelectTerm(Guid.Parse(request.SessionTermId));
                else
                {
                    selectedTerm = termService.GetCurrentTerm();
                    request.SessionTermId = selectedTerm.SessionTermId.ToString();
                }
                    

                if (selectedTerm != null)
                {
                    var entry = scoreEntryService.GetScoreEntry(selectedTerm.SessionTermId, Guid.Parse(request.StudentContactId), Guid.Parse(request.SubjectId));

                    if (entry != null)
                        entry = await scoreEntryService.UpdateScoreEntryForAssessment(request, entry);
                    else
                        entry = await scoreEntryService.CreateScoreEntryForAssessment(request);

                    res.Result = entry;
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Successful";
                }
                else
                {
                    var errorMsg = "System facing some technical issues with running term";
                    loggerService.Debug(errorMsg + "clentId:" + smsClientId);
                    res.Message.FriendlyMessage = errorMsg;
                }
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex?.Message;
                return res;
            }
        }
       
        async Task<APIResponse<MasterList>> IResultsService.GetMasterListAsync(Guid sessionClassId, Guid termId)
        {
            var regNoFormat = await GetRegNumberFormat();

            var term = termService.SelectTerm(termId);
            var res = new APIResponse<MasterList>();

            var sessClass = GetSessionClass(sessionClassId).Include(x => x.Teacher).FirstOrDefault();

            var mList = new MasterList(sessClass, term);

            var result = scoreEntryService.GetMasterListFromScoreEntry(sessionClassId, termId, regNoFormat);

            if (result.Any())
            {
                var averages = result.Select(d => d.AverageScore);
                var studentPositions = Tools.GetStudentPositions(averages);
                foreach(var item in result)
                    item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position?? "";

                result = result.OrderByDescending(d => d.AverageScore).ToList();
                mList.ResultList = result;
            }
            res.IsSuccessful = true;
            res.Result = mList;
            return res;
        }

        async Task<APIResponse<PagedResponse<StudentResult>>> IResultsService.GetClassResultListAsync(Guid sessionClassId, Guid termId, PaginationFilter filter)//bb
        {
            var res = new APIResponse<PagedResponse<StudentResult>>();
            var regNoFormat = await GetRegNumberFormat();

            try
            {
                var sessClass = GetSessionClass(sessionClassId).FirstOrDefault();
                var term = termService.SelectTerm(termId);
                res.Result = new PagedResponse<StudentResult>();
                res.Result.Data = new StudentResult();

                if (sessClass.Session.IsActive)
                {
                    var query = scoreEntryService.GetClassStudentInQuery(sessionClassId).Include(d => d.ScoreEntries);

                    var result = await paginationService.GetPagedResult(query, filter).Select(g => new StudentResultDetail(g, regNoFormat, sessionClassId, term.SessionTermId)).ToListAsync();
                    
                    if (result != null)
                    {
                        var averages = result.Select(d => d.AverageScore);
                        var studentPositions = Tools.GetStudentPositions(averages);

                        foreach (var item in result)
                            item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position ?? "";

                        result = result.OrderByDescending(d => d.AverageScore).ToList();
                        res.Result.Data.IsPublished  = IsResultPublished(sessionClassId, termId, term.IsActive);
                        res.Result.Data.PublishResult = result;
                        var totaltRecord = query.Count();
                        res.Result = paginationService.CreatePagedReponse(res.Result.Data, filter, totaltRecord);
                    }
                    res.IsSuccessful = true;
                    return res;
                }
                else
                {
                    var query = scoreEntryService.GetResultFromScoreEntryQuery(sessionClassId, termId);

                    var result = paginationService.GetPagedResult(query, filter).Select(entries => new StudentResultDetail(entries, regNoFormat)).ToList() ?? null;

                    if (result != null)
                    {
                        var averages = result.Select(x => x.AverageScore);
                        var studentPositions = Tools.GetStudentPositions(averages);
                        foreach (var item in result)
                        {
                            item.Position = studentPositions.FirstOrDefault(d => d.Average == (decimal)item.AverageScore)?.Position ?? "";
                        }
                        res.Result.Data.PublishResult = result.OrderByDescending(d => d.AverageScore).ToList();
                        res.Result.Data.IsPublished = IsResultPublished(sessionClassId, termId, term.IsActive);

                        var totaltRecord = query.Count();
                        res.Result = paginationService.CreatePagedReponse(res.Result.Data, filter, totaltRecord);
                    }
                    res.IsSuccessful = true;
                    return res;
                }


            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }

        async Task<APIResponse<PublishResultRequest>> IResultsService.PublishResultAsync(PublishResultRequest request)
        {
            var res = new APIResponse<PublishResultRequest>();
            try
            {
                var sessClass = GetSessionClass(request.SessionClassId)
                .Include(x => x.Students).FirstOrDefault();

                if (sessClass.Session.IsActive)
                    foreach (var student in sessClass.Students.Where(d => d.EnrollmentStatus == (int)EnrollmentStatus.Enrolled))
                        await SaveSessionClassArchiveAsync(sessClass, request.SessionTermId, student.StudentContactId, request.Publish);
                else
                {
                    List<StudentSessionClassHistory> stdsArchive = scoreEntryService.GetStudentsFromArchiveQuery(request.SessionClassId, request.SessionTermId).ToList();
                    foreach (var studentArchive in stdsArchive)
                        await SaveSessionClassArchiveAsync(sessClass, studentArchive.SessionTermId, studentArchive.StudentContactId, request.Publish);
                }
                await context.SaveChangesAsync();

                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = request.Publish ? $"You have Successfully published student results" : $"You have Successfully Unpublished student results";
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex?.Message;
                res.Message.TechnicalMessage = ex?.Message;
                return res;
            }
        }

        async Task<APIResponse<PagedResponse<GetClassScoreEntry>>> IResultsService.GetPreviousTermsClassSubjectScoreEntriesAsync(Guid sessionClassId, Guid subjectId, Guid sessionTermId, PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<GetClassScoreEntry>>();
            var regNoFormat = await GetRegNumberFormat();

            var sessClass = GetSessionClass(sessionClassId).FirstOrDefault();

            var result = await scoreEntryService.GetScoreEntriesQuery(subjectId, sessionClassId, sessionTermId)
                  .Include(d => d.SessionClass).ThenInclude(d => d.Class)
                .Select(s => new GetClassScoreEntry(s, regNoFormat)).FirstOrDefaultAsync();

            if (result is not null)
            {
                SessionClassSubject subject = await GetSessionClassSubject(subjectId, sessionClassId);
                if (subject is not null)
                {
                    result.SubjectTeacher = subject.SubjectTeacher.FirstName + " " + subject.SubjectTeacher.LastName;
                    result.AssessmentScore = subject.AssessmentScore;
                    result.ExamsScore = subject.ExamScore;
                }
            }

            if (sessClass.Session.IsActive)
            {
                var currentTerm = termService.SelectTerm(sessionTermId);
                if (result != null)
                {
                    var query = scoreEntryService.GetClassStudentInQuery(sessionClassId);
                    var sts = await paginationService.GetPagedResult(query, filter).ToListAsync();
                    foreach (var student in sts)
                    {
                        var scoreEntrySheet = new ScoreEntrySheet();
                        var scoreEntry = scoreEntryService.GetScoreEntry(sessionTermId, student.StudentContactId, subjectId);
                        scoreEntrySheet.AssessmentScore = scoreEntry?.AssessmentScore ?? 0;
                        scoreEntrySheet.ExamsScore = scoreEntry?.ExamScore ?? 0;
                        scoreEntrySheet.RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
                        scoreEntrySheet.StudentContactId = student.StudentContactId.ToString();
                        scoreEntrySheet.StudentName = student.FirstName + " " + student.LastName + " " + student.MiddleName;
                        scoreEntrySheet.IsOffered = scoreEntry?.IsOffered ?? false;
                        scoreEntrySheet.IsSaved = scoreEntry?.IsSaved ?? false;
                        scoreEntrySheet.TotalScore = scoreEntry?.ExamScore ?? 0 + scoreEntry?.AssessmentScore ?? 0;
                        result.ClassScoreEntries.Add(scoreEntrySheet);
                    }

                    var totaltRecord = query.Count();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
                }
            }
            else
            {

                if (result != null)
                {
                    var query = scoreEntryService.GetStudentsFromArchiveQuery(sessionClassId, sessionTermId);
                    var sts = await paginationService.GetPagedResult(query, filter).ToListAsync();
                    foreach (var student in sts)
                    {
                        ScoreEntrySheet scoreEntrySheet = new ScoreEntrySheet();
                        ScoreEntry scoreEntry = scoreEntryService.GetScoreEntry(sessionTermId, student.StudentContactId, subjectId);
                        StudentContact studenInfor = studentService.GetStudent(student.StudentContactId).FirstOrDefault();
                        scoreEntrySheet.AssessmentScore = scoreEntry?.AssessmentScore ?? 0;
                        scoreEntrySheet.ExamsScore = scoreEntry?.ExamScore ?? 0;
                        scoreEntrySheet.RegistrationNumber = regNoFormat.Replace("%VALUE%", studenInfor.RegistrationNumber);
                        scoreEntrySheet.StudentContactId = student.StudentContactId.ToString();
                        scoreEntrySheet.StudentName = studenInfor.FirstName + " " + studenInfor.LastName + " " + studenInfor.MiddleName;
                        scoreEntrySheet.IsOffered = scoreEntry?.IsOffered ?? false;
                        scoreEntrySheet.IsSaved = scoreEntry?.IsSaved ?? false;
                        scoreEntrySheet.TotalScore = scoreEntry?.ExamScore ?? 0 + scoreEntry?.AssessmentScore ?? 0;
                        result.ClassScoreEntries.Add(scoreEntrySheet);
                    }

                    var totaltRecord = query.Count();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
                }

            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PagedResponse<PreviewClassScoreEntry>>> IResultsService.PreviewPreviousTermsClassScoreEntry(Guid sessionClassId, Guid subjectId, Guid sessionTermId, PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<PreviewClassScoreEntry>>();
            var regNoFormat = await GetRegNumberFormat();

            var result = await GetClassScoreEntryPreviewHeader(sessionClassId, subjectId, regNoFormat);

            if (result is not null)
            {
                SessionClassSubject subject = await GetSessionClassSubject(subjectId, sessionClassId);
                if (subject is not null)
                {
                    result.SubjectTeacher = subject.SubjectTeacher.FirstName + " " + subject.SubjectTeacher.LastName;
                    result.AssessmentScore = subject.AssessmentScore;
                    result.ExamsScore = subject.ExamScore;
                }

                var classGrades = classGroupService.GetClassGrade(result.ClassLookupId);

                var query = scoreEntryService.GetScoreEntriesQuery(subjectId, sessionClassId, sessionTermId)
                    .Include(s => s.StudentContact)
                    .Select(d => new ScoreEntrySheet(d, regNoFormat, classGrades));

                result.ClassScoreEntries = await paginationService.GetPagedResult(query, filter).ToListAsync();
                var totaltRecord = query.Count();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<CumulativeMasterList>> IResultsService.GetCumulativeMasterListAsync(Guid sessionClassId, Guid termId)
        {
            var res = new APIResponse<CumulativeMasterList>();
            var regNoFormat = await GetRegNumberFormat();
            var sessClass = GetSessionClass(sessionClassId)
                .Include(r => r.Teacher).FirstOrDefault();
            var cMList = new CumulativeMasterList(sessClass);

            var result = scoreEntryService.GetCumulativeMasterListFromScoreEntry(sessionClassId, regNoFormat);

            if (result.Any())
            {
                var averages = result.Select(d => d.AverageScore);
                var studentPositions = Tools.GetStudentPositions(averages);

                foreach (var item in result)
                    item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position ?? "";

                result = result.OrderByDescending(d => d.AverageScore).ToList();
                cMList.ResultList = result;
            }
            res.IsSuccessful = true;
            res.Result = cMList;
            return res;
        }

        async Task<APIResponse<StudentCoreEntry>> IResultsService.GetSingleStudentScoreEntryAsync(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            var res = new APIResponse<StudentCoreEntry>();

            var sessClass = context.SessionClass.Where(x=>x.ClientId == smsClientId).Include(x => x.Class).Include(x => x.Session).FirstOrDefault(d => d.SessionClassId == sessionClassId);
            if (sessClass.Session.IsActive)
            {
                res.Result = GetSessionClass(sessionClassId)
                 .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(d => d.Subject)
                 .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(x => x.SessionTerm)
                 .Include(r => r.Students)
                 .Include(r => r.Students).ThenInclude(d => d.SessionClass).ThenInclude(d => d.Class).ThenInclude(d => d.GradeLevel).ThenInclude(d => d.Grades)
                 .Select(g => new StudentCoreEntry(g.Students.FirstOrDefault(x => x.StudentContactId == studentContactId), regNoFormat, termId)).FirstOrDefault();


                if (res.Result != null)
                    res.Result.IsPublished = IsResultPublished(sessionClassId, termId, studentContactId);
            }
            else
            {
                var student = context.StudentContact.Where(x=>x.ClientId == smsClientId).FirstOrDefault(s => s.StudentContactId == studentContactId);
                var studentResult = new StudentCoreEntry(student, regNoFormat);
                studentResult.SessionClassName = sessClass.Class.Name;
                var result = await scoreEntryService.GetScoreEntriesQuery(studentContactId, sessionClassId, termId)
                    .Include(d => d.Subject)
                    .Include(d => d.SessionClass).ThenInclude(d => d.Class).ThenInclude(d => d.GradeLevel).ThenInclude(d => d.Grades)
                    .Select(g => new StudentSubjectEntries(g, g.SessionClass.Class.GradeLevel)).ToListAsync();

                studentResult.StudentSubjectEntries = result;
                res.Result = studentResult;

                if (res.Result != null)
                    res.Result.IsPublished = IsResultPublished(sessionClassId, termId, studentContactId);
            }
                

          
            res.IsSuccessful = true;
            return res;
        }

        StudentResultRecord IResultsService.GetStudentResultOnPromotion(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var entryRecord =  context.ScoreEntry.Where(x => x.ClientId == smsClientId && x.StudentContactId == studentContactId && x.SessionTermId == termId)
                    .AsEnumerable()
                    .GroupBy(x => x.StudentContactId)
                    .Select(g => new StudentResultRecord(g.ToList())).FirstOrDefault() ?? new StudentResultRecord();

            entryRecord.ShouldPromoteStudent = entryRecord.AverageScore > context.SessionClass.FirstOrDefault(x => x.ClientId == smsClientId && x.SessionClassId == sessionClassId).PassMark;
            entryRecord.StudentContactId = studentContactId.ToString();
            return entryRecord;
        }
   
        private async Task SaveSessionClassArchiveAsync(SessionClass sessClass, Guid? termId, Guid studentId, bool publish)
        {
            var archive = await context.StudentSessionClassHistory
                .FirstOrDefaultAsync(x => x.ClientId == smsClientId && x.SessionClassId == sessClass.SessionClassId
                && termId == x.SessionTermId && x.StudentContactId == studentId);
            sessClass.IsPublished = publish;
            if (archive is null)
            {
                archive = new StudentSessionClassHistory
                {
                    SessionClassId = sessClass.SessionClassId,
                    StudentContactId = studentId,
                    SessionTermId = termId,
                    IsPublished = publish,
                };
                await context.StudentSessionClassHistory.AddAsync(archive);
            }
            else
                archive.IsPublished = publish;
        }

        async Task<APIResponse<List<PrintResult>>> IResultsService.GetStudentResultForBatchPrintingAsync(Guid sessionClassId, Guid termId)
        {
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            var res = new APIResponse<List<PrintResult>>();
            try
            {
                var term = termService.SelectTerm(termId);
                if (term == null)
                {
                    res.Message.FriendlyMessage = "Term not found";
                    return res;
                }

                var results =  context.ScoreEntry.Where(x => x.ClientId == smsClientId)
                   .Include(d => d.StudentContact)
                   .Include(d => d.SessionClass).ThenInclude(e => e.Session)
                    .Include(d => d.SessionClass).ThenInclude(e => e.Class).ThenInclude(s => s.GradeLevel).ThenInclude(x => x.Grades)
                   .Include(s => s.Subject)
                   .Where(rr => rr.SessionClassId == sessionClassId && rr.SessionTermId == termId).AsEnumerable()
                   .GroupBy(d => d.StudentContactId, (Key, g) => new { studentId = Key, lst = g.ToList()})
                   .Select(s => new PrintResult(s.lst, regNoFormat, term))
                   .ToList() ?? new List<PrintResult>();


                if (results.Any())
                {
                    var averages = results.Select(d => d.average);
                    var studentPositions = Tools.GetStudentPositions(averages);
                    foreach(var std in results)
                    {
                        std.position = studentPositions.FirstOrDefault(d => d.Average == std.average)?.Position ?? "";
                        std.noOfStudents = results.Count();
                    } 
                    res.Result = results;
                }
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }

        async Task<APIResponse<PrintResult>> IResultsService.GetStudentResultForPrintingAsync(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            var res = new APIResponse<PrintResult>();
            try
            {
                var term = termService.SelectTerm(termId);
                if (term == null)
                {
                    res.Message.FriendlyMessage = "Term not found";
                    return res;
                }

                var results = context.ScoreEntry.Where(x => x.ClientId == smsClientId)
                   .Include(d => d.StudentContact)
                   .Include(d => d.SessionClass).ThenInclude(e => e.Session)
                   .Include(d => d.SessionClass).ThenInclude(e => e.Class).ThenInclude(s => s.GradeLevel).ThenInclude(x => x.Grades)
                   .Include(s => s.Subject)
                   .Where(rr => rr.SessionClassId == sessionClassId && rr.SessionTermId == termId).AsEnumerable()
                   .GroupBy(d => d.StudentContactId, (Key, g) => new { studentId = Key, lst = g.ToList() })
                   .Select(s => new PrintResult(s.lst, regNoFormat, term, studentContactId))
                   .ToList() ?? new List<PrintResult>();


                if (results.Any())
                {
                    var averages = results.Select(d => d.average);
                    var studentPositions = Tools.GetStudentPositions(averages);
                    var studentResult = results.FirstOrDefault(d => d.studentContactId == studentContactId);
                    studentResult.position = studentPositions.FirstOrDefault(d => d.Average == studentResult.average)?.Position ?? "";
                    studentResult.noOfStudents = results.Count();
                    studentResult.isPublished = IsResultPublished(sessionClassId, termId, studentContactId);
                    res.Result = studentResult;
                }
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }

        async Task<APIResponse<PreviewResult>> IResultsService.GetStudentResultForPreviewAsync(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            var res = new APIResponse<PreviewResult>();
            var term = termService.SelectTerm(termId);
            if (term == null)
            {
                res.Message.FriendlyMessage = "Term not found";
                return res;
            }

            var result = context.ScoreEntry.Where(rr  => rr.ClientId == smsClientId && rr.SessionClassId == sessionClassId && rr.SessionTermId == termId)
                .Include(d => d.StudentContact)
                .Include(d => d.SessionClass).ThenInclude(e => e.Session)
                .Include(d => d.SessionClass).ThenInclude(e => e.Class).ThenInclude(f => f.GradeLevel).ThenInclude(s => s.Grades)
                .Include(d => d.Subject)
                .Where(rr => rr.Subject.Deleted == false && rr.Subject.IsActive == true).AsEnumerable().GroupBy(x => x.StudentContactId)
                .Select(g => new PreviewResult(g, regNoFormat, sessionClassId)).ToList() ?? new List<PreviewResult>();

            if (result.Any())
            {
                var averages = result.Select(d => d.average);
                var studentPositions = Tools.GetStudentPositions(averages);
                var studentResult = result.FirstOrDefault(d => d.studentContactId == studentContactId);
                if(studentResult != null)
                {
                    studentResult.noOfStudents = result.Count();
                    studentResult.position = studentPositions.FirstOrDefault(d => d.Average == studentResult.average)?.Position ?? "";
                    res.Result = studentResult;
                }
               

            }
            res.IsSuccessful = true;
            return res;
        }

        private bool IsResultPublished(Guid classId, Guid termId, bool isActiveTerm)
        {
            if(!isActiveTerm)
                return context.StudentSessionClassHistory.Any(d => d.ClientId == smsClientId && d.SessionClassId == classId && termId == d.SessionTermId && d.IsPublished == true);
            return context.SessionClass.Any(d => d.ClientId == smsClientId && d.SessionClassId == classId && termId == d.SessionTermId && d.IsPublished == true);
        }

        private bool IsResultPublished(Guid classId, Guid termId, Guid studentId) =>
           context.StudentSessionClassHistory.FirstOrDefault(d => d.ClientId == smsClientId && d.SessionClassId == classId 
           && termId == d.SessionTermId && d.StudentContactId == studentId)?.IsPublished ?? false;

        async Task<bool> IResultsService.AllResultPublishedAsync()
        {
            var publishCurrentClassResults = await context.SessionClass
                .Where(x=>x.ClientId == smsClientId)
                .Include(x => x.Students)
                .Where(d => d.Session.IsActive && d.Deleted == false && d.Students.Any() && d.IsPublished == true)
                .ToListAsync();
            if (!publishCurrentClassResults.Any())
                return false;
            return true;
        }

        async Task<APIResponse<BatchPrintDetail>> IResultsService.GetStudentsForBachPrinting(Guid sessionClassId, Guid termId)
        {
            var res = new APIResponse<BatchPrintDetail>();
            res.Result = new BatchPrintDetail();
            res.Result.Students = new List<StudentResultDetail>();
           
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            try
            {
                var clas = context.SessionClass.Where(x => x.ClientId == smsClientId).Include(x => x.Session).Include(x => x.Class).FirstOrDefault(d => d.SessionClassId == sessionClassId);
                var term = termService.SelectTerm(termId);

                if (clas.Session.IsActive)
                {
                    var result = await context.StudentContact.Where(x => x.ClientId == smsClientId)
                       .Include(d => d.User)
                       .Include(d => d.ScoreEntries)
                       .Where(rr => rr.SessionClassId == sessionClassId)
                       .Select(g => new StudentResultDetail(g, regNoFormat, sessionClassId, term.SessionTermId)).ToListAsync();

                    if (result != null)
                    {
                        var averages = result.Select(d => d.AverageScore);
                        var studentPositions = Tools.GetStudentPositions(averages);
                        foreach (var item in result)
                        {
                            item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position ?? "";
                        }
                        result = result.Where(x => x.AverageScore > 0).OrderByDescending(d => d.AverageScore).ToList();
                        res.Result.Students = result;
                    } 
                
                }
                else
                {
                    var classArchive = context.StudentSessionClassHistory.Where(d => d.ClientId == smsClientId && d.SessionClassId == sessionClassId && d.SessionTermId == termId && d.IsPublished).ToList();
                    if (!classArchive.Any())
                    {
                        res.Message.FriendlyMessage = "Result for this session and term was not published";
                        return res;
                    }

                    var result = context.ScoreEntry.Where(x=>x.ClientId == smsClientId)
                        .Include(x => x.SessionClass)
                        .Include(d => d.StudentContact)
                        .Where(rr => rr.SessionClassId == sessionClassId && termId == rr.SessionTermId && rr.IsOffered).AsEnumerable().GroupBy(s => s.StudentContactId)
                        .Select(entries => new StudentResultDetail(entries, regNoFormat)).ToList();

                    if (result != null)
                    {
                        var averages = result.Select(x => x.AverageScore);
                        var studentPositions = Tools.GetStudentPositions(averages);
                        foreach (var item in result)
                        {
                            item.Position = studentPositions.FirstOrDefault(d => d.Average == (decimal)item.AverageScore)?.Position ?? "";
                        }
                        res.Result.Students = result.Where(x => x.AverageScore > 0).ToList();
                    }
                }

                var studentCount = res.Result.Students.Count();
                var rs = new BatchPrintDetail(clas, term, "Success" ,studentCount);
                rs.Students = res.Result.Students;
                res.Result = rs;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }


        }

        async Task<APIResponse<List<PublishList>>> IResultsService.GetPublishedList()
        {
            var res = new APIResponse<List<PublishList>>();
            res.Result = new List<PublishList>();
            var currentTerm = termService.GetCurrentTerm();
            
            var classes = await context.SessionClass.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                .Include(x => x.Class)
                .Include(x => x.Session)
                .Where(x => x.Session.IsActive && x.Class.IsActive).Select(c => new { id = c.SessionClassId, name = c.Class.Name }).ToListAsync();

            foreach(var clas in classes)
            {
                var pubItem = new PublishList();
                pubItem.SessionClass = clas.name;
                pubItem.Status = IsResultPublished(clas.id, currentTerm.SessionTermId, currentTerm.IsActive) ? "published" : "unpublished";
                res.Result.Add(pubItem);
            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<PrintResult>> IResultsService.PrintResultAsync(PrintResultRequest request)
        {
            var res = new APIResponse<PrintResult>();
            try
            {
                var regNo = utilitiesService.GetStudentRegNumberValue(request.RegistractionNumber);
                var studentInfo = await utilitiesService.GetStudentContactByRegNo(regNo);
                if (studentInfo == null)
                {
                    res.Message.FriendlyMessage = "Invalid student registration number";
                    return res;
                }
                var classArchive = context.StudentSessionClassHistory
                    .FirstOrDefault(s => s.ClientId == smsClientId && s.SessionTermId == Guid.Parse(request.TermId) && s.StudentContactId == studentInfo.StudentContactId);
                if (classArchive is null)
                {
                    var errorMsq = $"Student was not found in class archive";
                    loggerService.Debug(errorMsq + " client: " + smsClientId);
                    res.Message.FriendlyMessage = errorMsq;
                    return res;
                }
                request.SessionClassid = classArchive.SessionClassId;
                var studentResult = await (this as IResultsService).GetStudentResultForPrintingAsync(request.SessionClassid, Guid.Parse(request.TermId), studentInfo.StudentContactId);
                if (studentResult.Result != null)
                {
                    studentResult.Result.IsPrint = true;
                    studentResult.Result.IsPreview = false;
                    if (!studentResult.Result.isPublished)
                    {
                        res.Message.FriendlyMessage = "Result not published";
                        return res;
                    }
                }
                else
                {
                    res.Message.FriendlyMessage = "Student Result does not exist";
                    return res;
                }
            }
            catch (ArgumentException ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
            return res;
        }

        async Task<APIResponse<List<PrintResult>>> IResultsService.PrintBatchResultResultAsync(BatchPrintResultRequest2 request)
        {
            var res = new APIResponse<List<PrintResult>>();

            try
            {
                var students = context.ScoreEntry.Where(x => x.ClientId == smsClientId)
                    .Where(x => x.SessionClassId == request.SessionClassId && x.SessionTermId == request.TermId && x.IsOffered).Select(x => x.StudentContact);

                var isArchived = IsResultArchived(request.SessionClassId, request.TermId, students.Select(x => x.StudentContactId).Distinct().ToList());
                if (!isArchived)
                {
                    res.Message.FriendlyMessage = "Republish class result to capture all student results";
                    return res;
                }

                var studentResults = await (this as IResultsService).GetStudentResultForBatchPrintingAsync(request.SessionClassId, request.TermId);
                if (studentResults.Result.Any())
                {
                    foreach (var student in studentResults.Result)
                    {
                        var regNo = utilitiesService.GetStudentRegNumberValue(student.registrationNumber);
                        var studentInfor = await utilitiesService.GetStudentContactByRegNo(regNo);
                    }
                    res.Result = studentResults.Result;
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    return res;
                }
                else
                {
                    res.Message.FriendlyMessage = "Student results does not exist";
                    return res;
                }
            }
            catch (ArgumentException ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }

        }

        bool IsResultArchived(Guid classId, Guid termId, List<Guid> stdIds)
        {
            var stds = context.StudentSessionClassHistory
                .Where(s => s.SessionTermId == termId && s.ClientId == smsClientId && s.SessionClassId == classId && s.IsPublished)
                .Select(x => x.StudentContactId).ToList();

            return stdIds.All(x => stds.Contains(x));
        }

        async Task<GetClassScoreEntry> GetClassScoreEntryHeader(Guid subjectId, SessionClass sessClass)
        {
            return await context.SessionClassSubject
               .Where(e => e.ClientId == smsClientId && e.SessionClassId == sessClass.SessionClassId && e.SubjectId == subjectId)
               .Include(x => x.SubjectTeacher)
               .Select(w => new GetClassScoreEntry
               {
                   SessionClassName = sessClass.Class.Name,
                   SessionClassId = sessClass.SessionClassId.ToString(),
                   SubjectId = w.SubjectId.ToString(),
                   SubjectName = w.Subject.Name,
                   SubjectTeacher = w.SubjectTeacher.FirstName + " " + w.SubjectTeacher.LastName,
                   AssessmentScore = w.AssessmentScore,
                   ExamsScore = w.ExamScore
               }).FirstOrDefaultAsync();
        }

        async Task<PreviewClassScoreEntry> GetClassScoreEntryPreviewHeader(Guid sessionClassId, Guid subjectId, string regNoFormat) => 
            await context.ScoreEntry
                .Where(e => e.ClientId == smsClientId && e.SessionClassId == sessionClassId && e.SubjectId == subjectId)
                .Include(d => d.SessionClass).ThenInclude(d => d.Class)
                .Include(d => d.Subject)
                .AsQueryable().Select(s => new PreviewClassScoreEntry(s, regNoFormat)).FirstOrDefaultAsync();

        IQueryable<SessionClass> GetSessionClass(Guid sessionClassId) => context.SessionClass
                .Include(x => x.Session)
                .Include(x => x.Class)
                .Where(x => x.SessionClassId == sessionClassId);
        async Task<string> GetRegNumberFormat() {

            var re = await context.SchoolSettings.Where(x => x.ClientId == smsClientId).FirstOrDefaultAsync();
            return re.SCHOOLSETTINGS_StudentRegNoFormat;
        }
       
        async Task<SessionClassSubject> GetSessionClassSubject(Guid subjectId, Guid sessionClassId) =>
            await context.SessionClassSubject
                .Where(e => e.ClientId == smsClientId && e.SessionClassId == sessionClassId && e.SubjectId == subjectId)
            .Include(x => x.Subject)
            .Include(x => x.SubjectTeacher).FirstOrDefaultAsync();

       

    }
}
