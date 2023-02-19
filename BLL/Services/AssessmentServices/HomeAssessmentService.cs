using BLL;
using BLL.Constants;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SMP.API.Hubs;
using SMP.BLL.Constants;
using SMP.BLL.Hubs;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.NotififcationServices;
using SMP.BLL.Services.ResultServices;
using SMP.Contracts.Assessment;
using SMP.Contracts.NotificationModels;
using SMP.DAL.Models.AssessmentEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AssessmentServices
{
    public class HomeAssessmentService : IHomeAssessmentService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;
        public readonly IFileUploadService uploadService;
        private readonly IHubContext<NotificationHub> hub;
        private readonly INotificationService notificationService;
        private readonly string smsClientId;
        private readonly IScoreEntryHistoryService scoreEntryService;

        public HomeAssessmentService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService, IFileUploadService uploadService, IHubContext<NotificationHub> hub, INotificationService notificationService, IScoreEntryHistoryService scoreEntryHistoryService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
            this.uploadService = uploadService;
            this.hub = hub;
            this.notificationService = notificationService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.scoreEntryService = scoreEntryHistoryService;
        }
        async Task<APIResponse<CreateHomeAssessmentRequest>> IHomeAssessmentService.CreateHomeAssessmentAsync(CreateHomeAssessmentRequest request)
        {
            var teacherId = accessor.HttpContext.User.FindFirst(x => x.Type == "teacherId").Value;
            var res = new APIResponse<CreateHomeAssessmentRequest>();
            try
            {
                if(request.SessionClassGroupId == "all-students")
                {
                    var homeAssessment = new HomeAssessment
                    {
                        SessionClassId = Guid.Parse(request.SessionClassId),
                        AssessmentScore = request.AssessmentScore,
                        Content = request.Content,
                        Comment = request.Comment,
                        SessionClassGroupId = Guid.Parse("eba102ba-d96c-4920-812a-080c8fdbe767"),//DO NOT CHANGE ID PLEASE>>>>
                        SessionClassSubjectId = Guid.Parse(request.SessionClassSubjectId),
                        SessionTermId = context.SessionTerm.FirstOrDefault(s => s.IsActive == true).SessionTermId,
                        Status = request.ShouldSendToStudents ? (int)HomeAssessmentStatus.Opened : (int)HomeAssessmentStatus.Saved,
                        Type = (int)AssessmentTypes.HomeAssessment,
                        Title = request.Title,
                        DateDeadLine = request.DateDeadLine,
                        TimeDeadLine = request.TimeDeadLine,
                        TeacherId = Guid.Parse(teacherId)
                    };
                    await context.HomeAssessment.AddAsync(homeAssessment);
                    await context.SaveChangesAsync();

                    await NotifyAllStudentsOnAssessmentCreationAsync(homeAssessment, request.ShouldSendToStudents);
                }
                else
                {
                    var homeAssessment = new HomeAssessment
                    {
                        SessionClassId = Guid.Parse(request.SessionClassId),
                        AssessmentScore = request.AssessmentScore,
                        Content = request.Content,
                        Comment = request.Comment,
                        SessionClassGroupId = Guid.Parse(request.SessionClassGroupId),
                        SessionClassSubjectId = Guid.Parse(request.SessionClassSubjectId),
                        SessionTermId = context.SessionTerm.FirstOrDefault(s => s.IsActive == true).SessionTermId,
                        Status = request.ShouldSendToStudents ? (int)HomeAssessmentStatus.Opened : (int)HomeAssessmentStatus.Saved,
                        Type = (int)AssessmentTypes.HomeAssessment,
                        Title = request.Title,
                        DateDeadLine = request.DateDeadLine,
                        TimeDeadLine = request.TimeDeadLine,
                        TeacherId = Guid.Parse(teacherId)
                    };
                    await context.HomeAssessment.AddAsync(homeAssessment);
                    await context.SaveChangesAsync();

                    await NotifyGroupOfStudentsOnAssessmentCreationAsync(homeAssessment, request.ShouldSendToStudents);


                }

                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        async Task<APIResponse<UpdateHomeAssessmentRequest>> IHomeAssessmentService.UpdateHomeAssessmentAsync(UpdateHomeAssessmentRequest request)
        {
            var teacherId = accessor.HttpContext.User.FindFirst(x => x.Type == "teacherId").Value;
            var res = new APIResponse<UpdateHomeAssessmentRequest>();
            try
            {
                var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).StudentRegNoFormat;
                var assessment = await context.HomeAssessment.FirstOrDefaultAsync(d => d.HomeAssessmentId == Guid.Parse(request.HomeAssessmentId) && d.ClientId == smsClientId);
                if (assessment is null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                assessment.AssessmentScore = request.AssessmentScore;
                assessment.Content = request.Content;
                assessment.Comment = request.Comment;
                assessment.DateDeadLine = request.DateDeadLine;
                assessment.TimeDeadLine = request.TimeDeadLine;
                assessment.SessionClassGroupId = request.SessionClassGroupId == "all-students" ? Guid.Parse("eba102ba-d96c-4920-812a-080c8fdbe767") :
                    Guid.Parse(request.SessionClassGroupId); //DO NOT CHANGE ID PLEASE....
                assessment.SessionClassSubjectId = Guid.Parse(request.SessionClassSubjectId);
                assessment.SessionTermId = context.SessionTerm.FirstOrDefault(s => s.IsActive == true).SessionTermId;
                assessment.Type = (int)AssessmentTypes.HomeAssessment;
                assessment.Title = request.Title;
                assessment.Status = request.ShouldSendToStudents ? assessment.Status : (int)HomeAssessmentStatus.Saved;
                assessment.TeacherId = Guid.Parse(teacherId);
                await context.SaveChangesAsync();


                if (request.SessionClassGroupId == "all-students")
                {
                    await NotifyAllStudentsOnAssessmentCreationAsync(assessment, request.ShouldSendToStudents);
                }
                else
                {
                    await NotifyGroupOfStudentsOnAssessmentCreationAsync(assessment, request.ShouldSendToStudents);
                }
                

                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Updated;
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
        }

        async Task<APIResponse<PagedResponse<List<GetHomeAssessmentRequest>>>> IHomeAssessmentService.GetSubjectHomeAssessmentAsync(string classId, string sessionClassSubjectId, string groupId, PaginationFilter filter)
        {
            var teacherId = accessor?.HttpContext?.User?.FindFirst(x => x.Type == "teacherId")?.Value;
            var res = new APIResponse<PagedResponse<List<GetHomeAssessmentRequest>>>();
            var activeTerm = context.SessionTerm.FirstOrDefault(d => d.ClientId == smsClientId && d.IsActive);
            
            var query = context.HomeAssessment.Where(d => d.SessionTermId == activeTerm.SessionTermId && d.ClientId == smsClientId)
                .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                 .Include(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                 .Include(q => q.SessionTerm)
                .OrderByDescending(d => d.CreatedOn).Where(d=> d.Deleted == false);

            if (!accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH) && !accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
            {
                query = query.Where(x => x.TeacherId == Guid.Parse(teacherId));
            }

            if (!string.IsNullOrEmpty(classId))
            {
                query = query.Where(d => d.SessionClassId == Guid.Parse(classId));
            }
            if (!string.IsNullOrEmpty(sessionClassSubjectId))
            {
                query = query.Where(d => d.SessionClassSubjectId == Guid.Parse(sessionClassSubjectId));
            }
            if (!string.IsNullOrEmpty(groupId))
            {
                if(groupId == "all-students")
                    query = query.Where(d => d.SessionClassGroupId == Guid.Parse("eba102ba-d96c-4920-812a-080c8fdbe767") && d.ClientId == smsClientId);
                else
                    query = query.Where(d => d.SessionClassGroupId == Guid.Parse(groupId) && d.ClientId == smsClientId);
            }

            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(f => new GetHomeAssessmentRequest(f, f.SessionClass.Students.Count(), false)).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<bool>> IHomeAssessmentService.DeleteHomeAssessmentAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var result = await context.HomeAssessment
                .Include(d => d.HomeAssessmentFeedBacks)
                .FirstOrDefaultAsync(x => x.HomeAssessmentId == Guid.Parse(request.Item) && x.ClientId == smsClientId);

            if (result != null)
            {
                context.HomeAssessment.Remove(result);
                await context.SaveChangesAsync();
            }

            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            res.IsSuccessful = true;
            res.Result = true;
            return res;
        }

        async Task<APIResponse<bool>> IHomeAssessmentService.SendHomeAssessmentToStudentsAsync(SendHomeAssessmentRequest request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var assessment = await context.HomeAssessment.FirstOrDefaultAsync(x => x.HomeAssessmentId == Guid.Parse(request.HomeAssessmentId) && x.ClientId == smsClientId);

                if (assessment != null)
                {
                    assessment.Status = request.ShouldSendToStudents ? (int)HomeAssessmentStatus.Opened : assessment.Status;
                    await context.SaveChangesAsync();

                    if (assessment.SessionClassGroupId == Guid.Parse("eba102ba-d96c-4920-812a-080c8fdbe767"))//DO NOT CHNGE PLEASE
                    {
                        await NotifyAllStudentsOnAssessmentCreationAsync(assessment, request.ShouldSendToStudents);
                    }
                    else
                    {
                        await NotifyGroupOfStudentsOnAssessmentCreationAsync(assessment, request.ShouldSendToStudents);
                    }
                }

                res.Message.FriendlyMessage = "Successful";
                res.IsSuccessful = true;
                res.Result = true;
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<APIResponse<GetHomeAssessmentRequest>> IHomeAssessmentService.GetSingleHomeAssessmentAsync(Guid homeAssessmentId, string sessionClasId)
        {
            var res = new APIResponse<GetHomeAssessmentRequest>();

            if (string.IsNullOrEmpty(sessionClasId))
            {
                var studentContactid = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId").Value;
                var student = context.StudentContact.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactid) && d.ClientId == smsClientId);
                sessionClasId = student.SessionClassId.ToString();
            }

            var studentsInClass = context.StudentContact
                .Where(f => f.ClientId == smsClientId && f.EnrollmentStatus == (int)EnrollmentStatus.Enrolled && f.SessionClassId == Guid.Parse(sessionClasId))
                .Include(s => s.User)
                .ToList();
            var ds = context.HomeAssessment;

            var result = await context.HomeAssessment
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.HomeAssessmentId == homeAssessmentId)
                .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                .Include(q => q.SessionClassGroup)
                .Include(q => q.SessionTerm)
                .Include(q => q.HomeAssessmentFeedBacks)
                .OrderByDescending(d => d.CreatedOn)
                .Select(f => new GetHomeAssessmentRequest(f, studentsInClass)).FirstOrDefaultAsync();

            if(result is not null)
            {
                var teacher = context.Teacher.Where(x=>x.ClientId == smsClientId).FirstOrDefault(x => x.TeacherId == result.TeacherId);
                result.TeacherName = teacher.FirstName + " " + teacher.LastName;
            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetHomeAssessmentRequest>> IHomeAssessmentService.GetSingleHomeAssessmentOnMobileAsync(Guid homeAssessmentId, string sessionClasId)
        {
            var res = new APIResponse<GetHomeAssessmentRequest>();

            if (string.IsNullOrEmpty(sessionClasId))
            {
                var studentContactid = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId").Value;
                var student = context.StudentContact.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactid) && d.ClientId == smsClientId);
                sessionClasId = student.SessionClassId.ToString();
            }

            var studentsInClass = context.StudentContact
                .Where(f => f.ClientId == smsClientId && f.EnrollmentStatus == (int)EnrollmentStatus.Enrolled && f.SessionClassId == Guid.Parse(sessionClasId))
                
                .ToList();
            var ds = context.HomeAssessment;

            var result = await context.HomeAssessment
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.HomeAssessmentId == homeAssessmentId)
                .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                .Include(q => q.SessionClassGroup)
                .Include(q => q.SessionTerm)
                .Include(q => q.HomeAssessmentFeedBacks)
                .OrderByDescending(d => d.CreatedOn)
                .Select(f => new GetHomeAssessmentRequest(f, studentsInClass, true)).FirstOrDefaultAsync();

            if (result is not null)
            {
                var teacher = context.Teacher.Where(x=>x.ClientId == smsClientId).FirstOrDefault(x => x.TeacherId == result.TeacherId);
                result.TeacherName = teacher.FirstName + " " + teacher.LastName;
            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<SubmittedAndUnsubmittedStudents>>> IHomeAssessmentService.GetSingleHomeAssessmentStudentsAsync(Guid homeAssessmentId, string sessionClasId)
        {
            var res = new APIResponse<List<SubmittedAndUnsubmittedStudents>>();

            if (string.IsNullOrEmpty(sessionClasId))
            {
                var studentContactid = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId").Value;
                var student = context.StudentContact.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactid) && d.ClientId == smsClientId);
                sessionClasId = student.SessionClassId.ToString();
            }

            var studentsInClass = context.StudentContact
                .Where(f => f.ClientId == smsClientId && f.EnrollmentStatus == (int)EnrollmentStatus.Enrolled && f.SessionClassId == Guid.Parse(sessionClasId))
                .Include(s => s.User)
                .ToList();




            var hmAss = await context.HomeAssessment
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.HomeAssessmentId == homeAssessmentId)
                .Include(q => q.SessionClassGroup)
                .Include(q => q.HomeAssessmentFeedBacks)
                .OrderByDescending(d => d.CreatedOn).FirstOrDefaultAsync();

            var studentIds = !string.IsNullOrEmpty(hmAss.SessionClassGroup.ListOfStudentContactIds) ?
               hmAss.SessionClassGroup.ListOfStudentContactIds.Split(',').ToList() : new List<string>();

            if (hmAss.SessionClassGroup.GroupName == "all-students")
            {
                studentsInClass.Select(s => s.StudentContactId).ToList().ForEach(ele =>
                {
                    studentIds.Add(ele.ToString());
                });
            }

            var result = studentIds.OrderByDescending(x => hmAss.HomeAssessmentFeedBacks.Select(s => s.StudentContactId.ToString())
            .Contains(x)).Select(id => new SubmittedAndUnsubmittedStudents(id, hmAss.HomeAssessmentFeedBacks, studentsInClass)).ToList();



            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetClassAssessmentRecord>> IHomeAssessmentService.GetSubjectAssessmentScoreRecordAsync(Guid sessionClassSubjectId, Guid sessionClasId)
        {
            var res = new APIResponse<GetClassAssessmentRecord>();
            var activeTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive && d.ClientId == smsClientId);
            var selectedClass = context.SessionClass.FirstOrDefault(s => s.SessionClassId == sessionClasId && s.ClientId == smsClientId);
            var homeAssessment = context.HomeAssessment.Where(d => d.SessionClassSubjectId == sessionClassSubjectId && d.SessionTermId == activeTerm.SessionTermId && d.ClientId == smsClientId);
            var classAssessment = context.ClassAssessment.Where(d => d.SessionClassSubjectId == sessionClassSubjectId && d.SessionTermId == activeTerm.SessionTermId && d.ClientId == smsClientId);
            var homeAScore = homeAssessment.Sum(d => d.AssessmentScore);
            var classAScore = classAssessment.Sum(d => d.AssessmentScore);

            var total = homeAScore + classAScore;

            res.IsSuccessful = true;
            var ass = new GetClassAssessmentRecord();
            ass.TotalAssessment = selectedClass.AssessmentScore;
            ass.Used = total;
            ass.Unused = Convert.ToDecimal((selectedClass.AssessmentScore - ass.Used).ToString().Trim('-'));
            res.Result = ass;
            return await Task.Run(() => res);
        }

        [Obsolete]
        async Task<APIResponse<List<StudentHomeAssessmentRequest>>> IHomeAssessmentService.GetHomeAssessmentsByStudentAsync()
        {
            
            var res = new APIResponse<List<StudentHomeAssessmentRequest>>();
            res.Result = new List<StudentHomeAssessmentRequest>();

            var activeTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive && d.ClientId == smsClientId);
            var studentContactid = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId").Value;
            var student = context.StudentContact.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactid) && d.ClientId == smsClientId);
            var result = await context.HomeAssessment.Where(x=>x.ClientId == smsClientId)
                .Include(d => d.HomeAssessmentFeedBacks)
                .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                 .Include(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                 .Include(q => q.SessionTerm)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false 
                && (d.Status == (int)HomeAssessmentStatus.Opened
                && d.SessionTermId ==activeTerm.SessionTermId
                && d.SessionClassId == student.SessionClassId)
                 && d.SessionClassGroup.GroupName == "all-students")
                .Select(f => new StudentHomeAssessmentRequest(f, studentContactid)).ToListAsync();

            result.ForEach(d =>
            {
                if(!string.IsNullOrEmpty(d.ListOfStudentContactIds) && d.ListOfStudentContactIds.Split(',').ToList().Contains(studentContactid))
                    res.Result.Add(d);
            });

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PagedResponse<List<StudentHomeAssessmentRequest>>>> IHomeAssessmentService.FilterHomeAssessmentsByStudentAsync(int status, PaginationFilter filter)
        {
            var studentContactid = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId").Value;
            var res = new APIResponse<PagedResponse<List<StudentHomeAssessmentRequest>>>();

            var activeTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive && d.ClientId == smsClientId);
            var student = await context.StudentContact.FirstOrDefaultAsync(d => d.StudentContactId == Guid.Parse(studentContactid) && d.ClientId == smsClientId);

            var query = context.HomeAssessment
                .Where(d => d.SessionClassId == student.SessionClassId && d.SessionTermId == activeTerm.SessionTermId && d.ClientId == smsClientId)
                .OrderByDescending(d => d.CreatedOn)
                .Include(d => d.HomeAssessmentFeedBacks)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                .Include(q => q.SessionClassGroup)
                .Where(x => x.Deleted == false);

            if (status == -1)
            {
                query = query.Where(x => x.Status == (int)HomeAssessmentStatus.Opened);
            }
            if (status >= 0 && status != 3)
            {
                query = query.Where(x => x.Status == status);
            }
            if (status == 3)
            {
                query = query.Where(x => x.HomeAssessmentFeedBacks.Any(x => x.StudentContactId == Guid.Parse(studentContactid)));
            }
            if (query is not null)
            {
                query = query.AsEnumerable().Where(d => !string.IsNullOrEmpty(d.SessionClassGroup.ListOfStudentContactIds) 
                && d.SessionClassGroup.ListOfStudentContactIds.Split(',').Contains(studentContactid) || d.SessionClassGroup.GroupName == "all-students").AsQueryable();
            }

            var totaltRecord = query.Count();
            var result = paginationService.GetPagedResult(query, filter).Select(f => new StudentHomeAssessmentRequest(f, studentContactid)).ToList();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);


            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<CreateHomeAssessmentFeedback>> IHomeAssessmentService.SubmitHomeAssessmentByStudentAsync(CreateHomeAssessmentFeedback request)
        {
            var studentContactid = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId").Value;
            var res = new APIResponse<CreateHomeAssessmentFeedback>();
            var reg = new HomeAssessmentFeedBack();
            try
            {
                if (!string.IsNullOrEmpty(request.HomeAssessmentFeedBackId))
                {
                    reg = context.HomeAssessmentFeedBack.Where(x=> x.ClientId == smsClientId).Include(x => x.HomeAssessment).FirstOrDefault(d => d.HomeAssessmentFeedBackId == Guid.Parse(request.HomeAssessmentFeedBackId));
                    if(reg is null)
                    {
                        res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                        return res;
                    }

                    if (reg.Included)
                    {
                        res.Message.FriendlyMessage = "Assignment has already been marked";
                        return res;
                    }

                    if (reg.HomeAssessment.Status == (int)HomeAssessmentStatus.Closed)
                    {
                        res.Message.FriendlyMessage = "Assignment has already been closed";
                        return res;
                    }

                    var files = uploadService.UpdateFeedbackFiles(reg.AttachmentUrls);
                    reg.Content = request.Content;
                    reg.Status = request.ShouldSubmit ? (int)HomeAssessmentStatus.Submitted : (int)HomeAssessmentStatus.Saved;
                    reg.AttachmentUrls = files;
                    reg.HomeAssessmentId = Guid.Parse(request.HomeAssessmentId);
                }
                else
                {
                    var assessment = context.HomeAssessment.FirstOrDefault(d => d.HomeAssessmentId == Guid.Parse(request.HomeAssessmentId) && d.ClientId == smsClientId);

                    if (assessment.Status == (int)HomeAssessmentStatus.Closed)
                    {
                        res.Message.FriendlyMessage = "Assignment has already been closed";
                        return res;
                    }

                    reg = new HomeAssessmentFeedBack();

                    var files = uploadService.UploadFeedbackFiles();
                    reg.StudentContactId = Guid.Parse(studentContactid);
                    reg.Content = request.Content;
                    reg.Status = request.ShouldSubmit ? (int)HomeAssessmentStatus.Submitted : (int)HomeAssessmentStatus.Saved;
                    reg.AttachmentUrls = "";
                    reg.HomeAssessmentId = Guid.Parse(request.HomeAssessmentId);
                    await context.HomeAssessmentFeedBack.AddAsync(reg);
                }
                
                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = request.ShouldSubmit ? "Successfully submitted" : "Successfully Saved";
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task<APIResponse<GetHomeAssessmentFeedback>> IHomeAssessmentService.GetSingleHomeAssessmentsByStudentAsync(Guid homeAssessmentFeedBackId)
        {
            var res = new APIResponse<GetHomeAssessmentFeedback>();


            var result = await context.HomeAssessmentFeedBack
              .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.HomeAssessmentFeedBackId == homeAssessmentFeedBackId)
              .Include(x => x.StudentContact).ThenInclude(x => x.User)
              .Select(f => new GetHomeAssessmentFeedback(f)).FirstOrDefaultAsync();

            if (result != null)
            {
                var assessment = context.HomeAssessment.Where(x => x.ClientId == smsClientId)
                    .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                    .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                    .Include(q => q.SessionClassGroup)
                    .Include(q => q.SessionTerm)
                    .FirstOrDefault(x => x.HomeAssessmentId == result.HomeAssessmentId);
                result.Assessment = new GetHomeAssessmentRequest(assessment, 0);

                var teacher = context.Teacher.Where(x=> x.ClientId == smsClientId).FirstOrDefault(x => x.TeacherId == assessment.TeacherId);
                result.TeacherName = teacher.FirstName + " " + teacher.LastName;
            }


            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetHomeAssessmentFeedback>> IHomeAssessmentService.GetSingleHomeAssessmentsByStudentOnMobileAsync(Guid homeAssessmentFeedBackId)
        {
            var res = new APIResponse<GetHomeAssessmentFeedback>();

            var result = await context.HomeAssessmentFeedBack
              .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.HomeAssessmentFeedBackId == homeAssessmentFeedBackId)
              .Include(d => d.StudentContact)
              .ThenInclude(s => s.User)
              .Select(f => new GetHomeAssessmentFeedback(f)).FirstOrDefaultAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetHomeAssessmentFeedback>> IHomeAssessmentService.GetSingleHomeAssessmentsByTeacherAsync(Guid homeAssessmentFeedBackId)
        {
            var res = new APIResponse<GetHomeAssessmentFeedback>();

            var result = await context.HomeAssessmentFeedBack
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.HomeAssessmentFeedBackId == homeAssessmentFeedBackId)
                .Select(f => new GetHomeAssessmentFeedback(f)).FirstOrDefaultAsync();

            if(result != null)
            {
                var assessment = context.HomeAssessment.Where(x => x.ClientId == smsClientId)
                    .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                    .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                    .Include(q => q.SessionClassGroup)
                    .Include(q => q.SessionTerm)
                    .FirstOrDefault(x => x.HomeAssessmentId == result.HomeAssessmentId);
                result.Assessment = new GetHomeAssessmentRequest(assessment, 0);

                var teacher = context.Teacher.Where(x => x.ClientId == smsClientId).FirstOrDefault(x => x.TeacherId == assessment.TeacherId);
                result.TeacherName = teacher.FirstName + " " + teacher.LastName;
            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<ScoreHomeAssessmentFeedback>> IHomeAssessmentService.ScoreHomeAssessmentByStudentAsync(ScoreHomeAssessmentFeedback request)
        {
            var res = new APIResponse<ScoreHomeAssessmentFeedback>();
            try
            {
                var feedBack = context.HomeAssessmentFeedBack.Where(x => x.ClientId == smsClientId)
                    .Include(x => x.StudentContact).ThenInclude(x => x.SessionClass)
                    .Include(x => x.HomeAssessment)
                    .ThenInclude(x => x.SessionClassSubject).FirstOrDefault(x => x.HomeAssessmentFeedBackId == Guid.Parse(request.HomeAssessmentFeedBackId));

                if (feedBack is null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                if (feedBack.HomeAssessment.Status != (int)HomeAssessmentStatus.Closed)
                {
                    res.Message.FriendlyMessage = "Marking can only be done when assessment is closed";
                    return res;
                }

                if (feedBack.Status != (int)HomeAssessmentStatus.Submitted)
                {
                    res.Message.FriendlyMessage = "Assessment cannot be marked yet";
                    return res;
                }

                if (request.Score > feedBack.HomeAssessment.AssessmentScore)
                {
                    res.Message.FriendlyMessage = $"Feedback can not be scored more than {feedBack.HomeAssessment.AssessmentScore}";
                    return res;
                }

                feedBack.Comment = request.Comment;
                feedBack.Mark = request.Score;
                feedBack.Included = false;

                if (request.Include)
                {
                    var includeRes = IncludeStudentAssessmentToScoreEntry(feedBack, request.Include);
                    if (includeRes != "success")
                    {
                        res.Message.FriendlyMessage = includeRes;
                        return res;
                    }
                }

                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Saved;
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<APIResponse<bool>> IHomeAssessmentService.CloseHomeAssessmentAsync(string homeAssessmentId)
        {
            var res = new APIResponse<bool>();
            var homeAssessment = await context.HomeAssessment
                .FirstOrDefaultAsync(x => x.HomeAssessmentId == Guid.Parse(homeAssessmentId) && x.ClientId == smsClientId);

            if (homeAssessment != null)
            {
                homeAssessment.Status = homeAssessment.Status == (int)HomeAssessmentStatus.Closed ? (int)HomeAssessmentStatus.Opened : (int)HomeAssessmentStatus.Closed;
                await context.SaveChangesAsync();
            }

            await SendAssessmentStatusNotificationAsync(((HomeAssessmentStatus)homeAssessment.Status).ToString(), homeAssessment);

            res.Message.FriendlyMessage = "Successful";
            res.IsSuccessful = true;
            res.Result = true;
            return res;
        }

        async Task<APIResponse<List<SubmittedAndUnsubmittedStudents>>> IHomeAssessmentService.GetHomeAssessmentRecord(string homeAssessmentId)
        {
            var res = new APIResponse<List<SubmittedAndUnsubmittedStudents>>();
            var result = await context.HomeAssessment.Where(x => x.ClientId == smsClientId)
                .Include(x => x.SessionClass).ThenInclude(x => x.Students).ThenInclude(x => x.User)
                .Include(x => x.HomeAssessmentFeedBacks)
                .FirstOrDefaultAsync(x => x.HomeAssessmentId == Guid.Parse(homeAssessmentId));

            var fbs = result.HomeAssessmentFeedBacks.ToList();
            if (result is null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            res.Result = result.SessionClass.Students.Select(x => new SubmittedAndUnsubmittedStudents(x, fbs)).ToList();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }
        async Task<APIResponse<bool>> IHomeAssessmentService.IncludeClassAssessmentToScoreEntry(string homeAssessmentId, bool Include)
        {
            var res = new APIResponse<bool>();
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive == true && x.ClientId == smsClientId).SessionTermId;
            var assessment = GetHomeAssessmentById(homeAssessmentId);

            if (assessment is null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            if (assessment.Status != (int)HomeAssessmentStatus.Closed)
            {
                res.Message.FriendlyMessage = "Assessment has not yet been closed";
                return res;
            }

            var students = assessment.SessionClass.Students
                .Where(en => en.EnrollmentStatus == (int)EnrollmentStatus.Enrolled)
                .Select(x => new 
                    { 
                        Score = assessment.HomeAssessmentFeedBacks.FirstOrDefault(s => s.StudentContactId == x.StudentContactId)?.Mark??0, 
                        StudentId = x.StudentContactId, 
                        SubjectId = assessment.SessionClassSubject.SubjectId,
                        Name = x.FirstName + " " + x.LastName,
                        FeedBackId = assessment.HomeAssessmentFeedBacks.FirstOrDefault(s => s.StudentContactId == x.StudentContactId)?.HomeAssessmentFeedBackId,
                        SessionClassId = assessment.SessionClassSubject.SessionClassId.ToString()
                }
                ).ToList();

            foreach(var std in students)
            {
                var scoreHistory = scoreEntryService.GetScoreEntryHistory(std.SessionClassId, std.SubjectId.ToString(), termId.ToString(), std.StudentId.ToString());

                var feedBack = context.HomeAssessmentFeedBack.FirstOrDefault(x => x.HomeAssessmentFeedBackId == std.FeedBackId && x.ClientId == smsClientId);
                if (feedBack is null)
                {
                    continue;
                }
                float score = 0;
                if (scoreHistory is null)
                    score = await scoreEntryService.CreateNewScoreEntryHistoryAndReturnScore(scoreHistory, (float)std.Score, std.StudentId.ToString(), std.SessionClassId, std.SubjectId.ToString(), termId, Include);
                else
                {
                    if(feedBack.Included && !Include)
                        score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, Include, (float)std.Score);
                    if (!feedBack.Included && Include)
                        score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, Include, (float)std.Score);
                }
                var scoreEntry = scoreEntryService.GetScoreEntry(termId, std.StudentId, std.SubjectId);

                if (scoreEntry is null)
                {
                    scoreEntryService.CreateNewScoreEntryForAssessment(scoreEntry, termId, score, std.StudentId, std.SubjectId, Guid.Parse(std.SessionClassId));
                    feedBack.Included = Include ? true : false;
                    await context.SaveChangesAsync();
                }
                else
                {
                    if (std.Score > assessment.SessionClass.AssessmentScore)
                    {
                        res.Message.FriendlyMessage = $"{std.Name}'s Assessment score can not be more than class assessment ({assessment.SessionClass.AssessmentScore})";
                        return res;
                    }

                    if (score > assessment.SessionClass.AssessmentScore)
                    {
                        res.Message.FriendlyMessage = $"{std.Name}'s Previously scored assessment + {Convert.ToInt16(feedBack.Mark)} can not be more than class assessment ({assessment.SessionClass.AssessmentScore})";
                        return res;
                    }
                    scoreEntryService.UpdateScoreEntryForAssessment(scoreEntry, score);
                    feedBack.Included = Include ? true : false;
                    await context.SaveChangesAsync();
                }

            }
            res.Message.FriendlyMessage = "Records included successfully";
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<bool>> IHomeAssessmentService.IncludeStudentAssessmentToScoreEntry(string homeAssessmentFeedbackId, bool include)
        {
            var res = new APIResponse<bool>();
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive && x.ClientId == smsClientId).SessionTermId;

            var feedBack = GetHomeAssessmentFeedBack(homeAssessmentFeedbackId);
            if (feedBack is null)
            {
                res.Message.FriendlyMessage = $"Feedback not yet submitted";
                return res;
            }
            if (feedBack.HomeAssessment.Status != (int)HomeAssessmentStatus.Closed)
            {
                res.Message.FriendlyMessage = "Assessment has not yet closed";
                return res;
            }
            var scoreHistory = scoreEntryService.GetScoreEntryHistory(feedBack.HomeAssessment.SessionClassId.ToString(), feedBack.HomeAssessment.SessionClassSubject.SubjectId.ToString(), termId.ToString(), feedBack.StudentContactId.ToString());

            float score = 0;
            if (scoreHistory is null)
                score = await scoreEntryService.CreateNewScoreEntryHistoryAndReturnScore(scoreHistory, (float)feedBack.Mark, feedBack.StudentContactId.ToString(), feedBack.HomeAssessment.SessionClassId.ToString(), feedBack.HomeAssessment.SessionClassSubject.SubjectId.ToString(), termId, include);
            else
            {
                if (feedBack.Included && !include)
                    score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, include, (float)feedBack.Mark);
                if (!feedBack.Included && include)
                    score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, include, (float)feedBack.Mark);
            }

            var scoreEntry = scoreEntryService.GetScoreEntry(termId, feedBack.StudentContactId, feedBack.HomeAssessment.SessionClassSubject.SubjectId);
            if (scoreEntry is null)
            {
                scoreEntryService.CreateNewScoreEntryForAssessment(scoreEntry, termId, score, feedBack.StudentContactId, feedBack.HomeAssessment.SessionClassSubject.SubjectId, feedBack.HomeAssessment.SessionClassId);
                feedBack.IncludedScore = (int)score;
                feedBack.Included = include ? true : false;
                await context.SaveChangesAsync();
            }
            else
            {

                if (feedBack.Mark > feedBack.StudentContact.SessionClass.AssessmentScore)
                {
                    res.Message.FriendlyMessage = $"Assessment score can not be more than class assessment ({feedBack.StudentContact.SessionClass.AssessmentScore})";
                    return res;
                }

                if (score > feedBack.StudentContact.SessionClass.AssessmentScore)
                {
                    res.Message.FriendlyMessage = $"Previously scored assessment + {Convert.ToInt16(feedBack.Mark)} can not be more than class assessment ({feedBack.StudentContact.SessionClass.AssessmentScore})";
                    return res;
                }
                feedBack.IncludedScore = (int)score;
                feedBack.Included = include ? true : false;
                scoreEntryService.UpdateScoreEntryForAssessment(scoreEntry, score);
                await context.SaveChangesAsync();
            }
            res.Message.FriendlyMessage = "Records included successfully";
            res.IsSuccessful = true;
            return res;
        }

        private string IncludeStudentAssessmentToScoreEntry(HomeAssessmentFeedBack feedBack, bool include)
        {
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive && x.ClientId == smsClientId).SessionTermId;

            if (feedBack is null)
            {
                return $"Feedback not yet submitted";
            }
            if (feedBack.HomeAssessment.Status != (int)HomeAssessmentStatus.Closed)
            {
                return "Assessment has not yet closed";
            }
            if (feedBack.Included)
            {
                return $"Feedback has already be included to score entry";
            }

            var scoreHistory = scoreEntryService.GetScoreEntryHistory(feedBack.HomeAssessment.SessionClassId.ToString(), feedBack.HomeAssessment.SessionClassSubject.SubjectId.ToString(), termId.ToString(), feedBack.StudentContactId.ToString());

            float score = 0;
            if (scoreHistory is null)
                score = scoreEntryService.CreateNewScoreEntryHistoryAndReturnScore(scoreHistory, (float)feedBack.Mark, feedBack.StudentContactId.ToString(), feedBack.HomeAssessment.SessionClassId.ToString(), feedBack.HomeAssessment.SessionClassSubject.SubjectId.ToString(), termId, include).Result;
            else
            {
                if (feedBack.Included && !include)
                    score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, include, (float)feedBack.Mark);
                if (!feedBack.Included && include)
                    score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, include, (float)feedBack.Mark);
            }
            var scoreEntry = scoreEntryService.GetScoreEntry(termId, feedBack.StudentContactId, feedBack.HomeAssessment.SessionClassSubject.SubjectId);

            if (scoreEntry is null)
            {
                scoreEntryService.CreateNewScoreEntryForAssessment(scoreEntry, termId, score, feedBack.StudentContactId, feedBack.HomeAssessment.SessionClassSubject.SubjectId, feedBack.HomeAssessment.SessionClassId);
                feedBack.IncludedScore = Convert.ToInt16(feedBack.Mark);
                feedBack.Included = include ? true : false;
            }
            else
            {
                if (feedBack.Included && !include)
                {
                    score = scoreEntry.AssessmentScore - Convert.ToInt16(feedBack.IncludedScore);
                }

                if (!feedBack.Included && include)
                {
                    score = scoreEntry.AssessmentScore + Convert.ToInt16(feedBack.Mark);
                }


                if (feedBack.Mark > feedBack.StudentContact.SessionClass.AssessmentScore)
                {
                    return $"Assessment score can not be more than class assessment ({feedBack.StudentContact.SessionClass.AssessmentScore})";
                }

                if (score > feedBack.StudentContact.SessionClass.AssessmentScore)
                {
                    return $"Previously scored assessment + {Convert.ToInt16(feedBack.Mark)} can not be more than class assessment ({feedBack.StudentContact.SessionClass.AssessmentScore})";
                    
                }

                feedBack.Included = true;
                feedBack.IncludedScore = (int)score;

                scoreEntryService.UpdateScoreEntryForAssessment(scoreEntry, score);
                context.SaveChanges();
            }
            return "success";
        }

        APIResponse<string> IHomeAssessmentService.ReadFileContent(IFormFile file)
        {
                var res = new APIResponse<string>();
            try
            {
                res.Result = uploadService.RetunFileContent(file);
                res.Message.FriendlyMessage = "Successfully read";
                res.IsSuccessful = true;
                return res;
            }
            catch (ArgumentException ex)
            {
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
        }

        async Task<APIResponse<PagedResponse<List<StudentHomeAssessmentRequest>>>> IHomeAssessmentService.FilterHomeAssessmentsByParentAsync(Guid sessionClassSubjectId, int status, string studentContactid, PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<StudentHomeAssessmentRequest>>>();

            var activeTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive && d.ClientId == smsClientId);
            var student = await context.StudentContact.FirstOrDefaultAsync(d => d.StudentContactId == Guid.Parse(studentContactid) && d.ClientId == smsClientId);

            var query = context.HomeAssessment
                .Where(d => d.ClientId == smsClientId && d.SessionClassId == student.SessionClassId && d.SessionTermId == activeTerm.SessionTermId 
                && d.SessionClassSubjectId == sessionClassSubjectId && d.Status == status)
                .OrderByDescending(d => d.CreatedOn)
                .Include(d => d.HomeAssessmentFeedBacks)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                .Include(q => q.SessionClassGroup)
                .Where(x => x.Deleted == false);

            //query = query.Where(x => x.HomeAssessmentFeedBacks.Any(e => e.StudentContactId == Guid.Parse(studentContactid)));

            if (query is not null)
            {
                query = query.Where(d => !string.IsNullOrEmpty(d.SessionClassGroup.ListOfStudentContactIds) || d.SessionClassGroup.GroupName == "all-students");

                query = query.AsEnumerable().Where(d => !string.IsNullOrEmpty(d.SessionClassGroup.ListOfStudentContactIds) ? d.SessionClassGroup.ListOfStudentContactIds.Split(',').ToArray().Contains(studentContactid) : true).AsQueryable();
            }

            var totaltRecord = query.Count();
            var result = paginationService.GetPagedResult(query, filter).Select(f => new StudentHomeAssessmentRequest(f, studentContactid)).ToList();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);


            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }
        
        private async Task NotifyAllStudentsOnAssessmentCreationAsync(HomeAssessment reg, bool shouldSendToStudents)
        {

            var notifications = context.Notification.Where(x => x.ClientId == smsClientId && x.NotificationSourceId == reg.HomeAssessmentId.ToString()).FirstOrDefault() ?? null;
            if (notifications == null)
            {
                var subject = context.SessionClassSubject.Where(x => x.ClientId == smsClientId && x.SessionClassSubjectId == reg.SessionClassSubjectId).Select(x => x.Subject.Name).FirstOrDefault();

                if (shouldSendToStudents)
                {
                    await notificationService.CreateNotitficationAsync(new NotificationDTO
                    {
                        Content = $"Home assessment for {subject} has been created",
                        NotificationPageLink = $"smp-notification/home-assessment-details?homeAssessmentId={reg.HomeAssessmentId}&sessionClassId={reg.SessionClassId}&sessionClassSubjectId={reg.SessionClassSubjectId}&groupId=all-students&type=home-assessment",
                        NotificationSourceId = reg.HomeAssessmentId.ToString(),
                        Subject = $"Home Assessment on {subject}",
                        Receivers = "all",
                        Type = "home-assessment",
                        ToGroup = "Students"
                    }, true);
                    await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
                }
            }
          
        }

        private async Task NotifyGroupOfStudentsOnAssessmentCreationAsync(HomeAssessment reg, bool shouldSendToStudents)
        {
            var notifications = context.Notification.Where(x => x.ClientId == smsClientId && x.NotificationSourceId == reg.HomeAssessmentId.ToString()).FirstOrDefault() ?? null;
            if (notifications == null)
            {
                if (shouldSendToStudents)
                {
                    var subject = context.SessionClassSubject.Where(x=>x.ClientId == smsClientId).Include(x => x.Subject).FirstOrDefault(x => x.SessionClassSubjectId == reg.SessionClassSubjectId);
                    var studentIds = context.SessionClassGroup.FirstOrDefault(x => x.SessionClassGroupId == reg.SessionClassGroupId && x.ClientId == smsClientId).ListOfStudentContactIds.Split(",").ToList();
                    var userIds = context.StudentContact.Where(x => x.ClientId == smsClientId && studentIds.Contains(x.StudentContactId.ToString())).Select(x => x.UserId).ToList();
                    string studentEmails = string.Join(",", context.Users.Where(x => x.Deleted == false && userIds.Contains(x.Id)).Select(x => x.Email).ToList());

                    await notificationService.CreateNotitficationAsync(new NotificationDTO
                    {
                        Content = $"Home assessment for {subject} has been created",
                        NotificationPageLink = $"smp-notification/home-assessment-details?homeAssessmentId={reg.HomeAssessmentId}&sessionClassId={reg.SessionClassId}&sessionClassSubjectId={reg.SessionClassSubjectId}&groupId=all-students&type=home-assessment",
                        NotificationSourceId = reg.HomeAssessmentId.ToString(),
                        Subject = $"Home Assessment on {subject}",
                        ReceiversEmail = studentEmails,
                        Type = "home-assessment",
                        ToGroup = "Students"
                    }, true);
                    await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
                }
            }
        }
    
        private async Task SendAssessmentStatusNotificationAsync(string status, HomeAssessment homeAssessment)
        {
            var subject = context.SessionClassSubject.Where(x => x.ClientId == smsClientId && x.SessionClassSubjectId == homeAssessment.SessionClassSubjectId).Include(x => x.Subject).FirstOrDefault().Subject.Name;
            var sessionClassGroup = context.SessionClassGroup.FirstOrDefault(x => x.SessionClassGroupId == homeAssessment.SessionClassGroupId);
            string studentEmails = "";
            if (sessionClassGroup.GroupName == "all-students")
            {
                studentEmails = "all";
            }
            else
            {
                var studentIds = sessionClassGroup.ListOfStudentContactIds.Split(",").ToList();
                var userIds = context.StudentContact.Where(x => x.ClientId == smsClientId && studentIds.Contains(x.StudentContactId.ToString())).Select(x => x.UserId).ToList();
                studentEmails = string.Join(",", context.Users.Where(x => x.Deleted == false && userIds.Contains(x.Id)).Select(x => x.Email).ToList());
            }
            await notificationService.CreateNotitficationAsync(new NotificationDTO
            {
                Content = $"Home assessment on {subject} has been {status}",
                NotificationPageLink = $"smp-notification/home-assessment-details?homeAssessmentId={homeAssessment.HomeAssessmentId}&sessionClassId={homeAssessment.SessionClassId}&sessionClassSubjectId={homeAssessment.SessionClassSubjectId}&groupId=all-students&type=home-assessment",
                NotificationSourceId = homeAssessment.HomeAssessmentId.ToString(),
                Subject = $"Home assessment on {subject}",
                Receivers = studentEmails,
                Type = "home-assessment",
                ToGroup = "Students"
            }, true);
            await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
        }


        private HomeAssessment GetHomeAssessmentById(string homeAssessmentId) => 
            context.HomeAssessment
                .Where(x => x.HomeAssessmentId == Guid.Parse(homeAssessmentId) && x.ClientId == smsClientId)
                .Include(x => x.HomeAssessmentFeedBacks)
                .Include(x => x.SessionClass).ThenInclude(x => x.Students)
                .Include(x => x.SessionClassSubject)
                .FirstOrDefault();
        
        
        private HomeAssessmentFeedBack GetHomeAssessmentFeedBack(string homeAssessmentFeedbackId) => 
            context.HomeAssessmentFeedBack.Where(x => x.ClientId == smsClientId)
                .Include(x => x.StudentContact).ThenInclude(x => x.SessionClass)
                .Include(x => x.HomeAssessment)
                .ThenInclude(x => x.SessionClassSubject).FirstOrDefault(x => x.HomeAssessmentFeedBackId == Guid.Parse(homeAssessmentFeedbackId));
    }
}
