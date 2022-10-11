using BLL;
using BLL.Constants;
using BLL.Filter;
using BLL.Utilities;
using BLL.Wrappers;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.FilterService;
using SMP.Contracts.Assessment;
using SMP.DAL.Models.AssessmentEntities;
using SMP.DAL.Models.ResultModels;
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
        public HomeAssessmentService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
        }
        async Task<APIResponse<CreateHomeAssessmentRequest>> IHomeAssessmentService.CreateHomeAssessmentAsync(CreateHomeAssessmentRequest request)
        {
            var teacherId = accessor.HttpContext.User.FindFirst(x => x.Type == "teacherId").Value;
            var res = new APIResponse<CreateHomeAssessmentRequest>();
            try
            {

                if(request.SessionClassGroupId == "all-students")
                {
                    var reg = new HomeAssessment
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
                    await context.HomeAssessment.AddAsync(reg);
                }
                else
                {
                    var reg = new HomeAssessment
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
                    await context.HomeAssessment.AddAsync(reg);
                }

                await context.SaveChangesAsync();
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
                var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
                var assessment = await context.HomeAssessment.FirstOrDefaultAsync(d => d.HomeAssessmentId == Guid.Parse(request.HomeAssessmentId));
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
            var teacherId = accessor.HttpContext.User.FindFirst(x => x.Type == "teacherId").Value;
            var res = new APIResponse<PagedResponse<List<GetHomeAssessmentRequest>>>();
            var activeTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive);
            var query =  context.HomeAssessment
                .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                 .Include(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                 .Include(q => q.SessionTerm)
                .OrderByDescending(d => d.CreatedOn).Where(d => d.Deleted == false)
                .Where(d => d.SessionTermId == activeTerm.SessionTermId);

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
                    query = query.Where(d => d.SessionClassGroupId == Guid.Parse("eba102ba-d96c-4920-812a-080c8fdbe767"));
                else
                    query = query.Where(d => d.SessionClassGroupId == Guid.Parse(groupId));
            }

            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(f => new GetHomeAssessmentRequest(f, f.SessionClass.Students.Count())).ToListAsync();
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
                .FirstOrDefaultAsync(x => x.HomeAssessmentId == Guid.Parse(request.Item));

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
                var result = await context.HomeAssessment.FirstOrDefaultAsync(x => x.HomeAssessmentId == Guid.Parse(request.HomeAssessmentId));

                if (result != null)
                {
                    result.Status = request.ShouldSendToStudents ? (int)HomeAssessmentStatus.Opened : result.Status;
                    await context.SaveChangesAsync();
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
                var student = context.StudentContact.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactid));
                sessionClasId = student.StudentContactId.ToString();
            }

            var studentsInClass = context.SessionClass.Include(s => s.Students)
                .Where(d => d.SessionClassId == Guid.Parse(sessionClasId)).SelectMany(s => s.Students)
                .Where(f => f.EnrollmentStatus == (int)EnrollmentStatus.Enrolled).ToList();
            var result = await context.HomeAssessment
                   .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(s => s.SessionClass).ThenInclude(s => s.Students).ThenInclude(s => s.User)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                 .Include(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                 .Include(q => q.SessionTerm)
                 //.Include(q => q.AssessmentScoreRecord)
                 .Include(q => q.HomeAssessmentFeedBacks).ThenInclude(d => d.StudentContact)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false && d.HomeAssessmentId == homeAssessmentId)
                .Select(f => new GetHomeAssessmentRequest(f, studentsInClass)).FirstOrDefaultAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetClassAssessmentRecord>> IHomeAssessmentService.GetSubjectAssessmentScoreRecordAsync(Guid sessionClassSubjectId, Guid sessionClasId)
        {
            var res = new APIResponse<GetClassAssessmentRecord>();
            var activeTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive);
            var selectedClass = context.SessionClass.FirstOrDefault(s => s.SessionClassId == sessionClasId);
            var homeAssessment = context.HomeAssessment.Where(d => d.SessionClassSubjectId == sessionClassSubjectId && d.SessionTermId == activeTerm.SessionTermId);
            var classAssessment = context.ClassAssessment.Where(d => d.SessionClassSubjectId == sessionClassSubjectId && d.SessionTermId == activeTerm.SessionTermId);
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

        async Task<APIResponse<List<StudentHomeAssessmentRequest>>> IHomeAssessmentService.GetHomeAssessmentsByStudentAsync()
        {
            
            var res = new APIResponse<List<StudentHomeAssessmentRequest>>();
            res.Result = new List<StudentHomeAssessmentRequest>();

            var activeTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive);
            var studentContactid = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId").Value;
            var student = context.StudentContact.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactid));
            var result = await context.HomeAssessment
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

            var activeTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive);
            var student = await context.StudentContact.FirstOrDefaultAsync(d => d.StudentContactId == Guid.Parse(studentContactid));

            var query = context.HomeAssessment
                .Include(d => d.HomeAssessmentFeedBacks)
                .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                .Include(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                .Include(q => q.SessionTerm)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false && d.SessionClassId == student.SessionClassId && d.SessionTermId == activeTerm.SessionTermId);

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
                    reg = context.HomeAssessmentFeedBack.Include(x => x.HomeAssessment).FirstOrDefault(d => d.HomeAssessmentFeedBackId == Guid.Parse(request.HomeAssessmentFeedBackId));
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

                    //do update 
                    //assigment cannot be edited when the status is submitted

                    reg.Content = request.Content;
                    reg.Status = request.ShouldSubmit ? (int)HomeAssessmentStatus.Submitted : (int)HomeAssessmentStatus.Saved;
                    reg.AttachmentUrls = "";
                    reg.HomeAssessmentId = Guid.Parse(request.HomeAssessmentId);
                }
                else
                {
                    var assessment = context.HomeAssessment.FirstOrDefault(d => d.HomeAssessmentId == Guid.Parse(request.HomeAssessmentId));

                    if (assessment.Status == (int)HomeAssessmentStatus.Closed)
                    {
                        res.Message.FriendlyMessage = "Assignment has already been closed";
                        return res;
                    }

                    reg = new HomeAssessmentFeedBack();

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
                res.Message.FriendlyMessage = "Successfully submitted";
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<APIResponse<GetHomeAssessmentFeedback>> IHomeAssessmentService.GetSingleHomeAssessmentsByStudentAsync(Guid homeAssessmentFeedBackId)
        {
            var res = new APIResponse<GetHomeAssessmentFeedback>();

            var result = await context.HomeAssessmentFeedBack
                .Include(s => s.HomeAssessment).ThenInclude(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(s => s.HomeAssessment).ThenInclude(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(s => s.HomeAssessment).ThenInclude(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                 .Include(s => s.HomeAssessment).ThenInclude(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                 .Include(s => s.HomeAssessment).ThenInclude(q => q.SessionTerm)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false
                && d.HomeAssessmentFeedBackId == homeAssessmentFeedBackId)
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
                .Include(s => s.HomeAssessment).ThenInclude(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(s => s.HomeAssessment).ThenInclude(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(s => s.HomeAssessment).ThenInclude(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                 .Include(s => s.HomeAssessment).ThenInclude(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                 .Include(s => s.HomeAssessment).ThenInclude(q => q.SessionTerm)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false
                && d.HomeAssessmentFeedBackId == homeAssessmentFeedBackId)
                .Select(f => new GetHomeAssessmentFeedback(f)).FirstOrDefaultAsync();



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
                //var feedBack = context.HomeAssessmentFeedBack.Include(d => d.HomeAssessment)
                //    .FirstOrDefault(d => d.HomeAssessmentFeedBackId == Guid.Parse(request.HomeAssessmentFeedBackId));

                var feedBack = context.HomeAssessmentFeedBack
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
                    var includeRes = IncludeStudentAssessmentToScoreEntry(feedBack);
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
            var result = await context.HomeAssessment
                .FirstOrDefaultAsync(x => x.HomeAssessmentId == Guid.Parse(homeAssessmentId));

            if (result != null)
            {
                result.Status = result.Status == (int)HomeAssessmentStatus.Closed ? (int)HomeAssessmentStatus.Opened : (int)HomeAssessmentStatus.Closed;
                await context.SaveChangesAsync();
            }

            res.Message.FriendlyMessage = "Successful";
            res.IsSuccessful = true;
            res.Result = true;
            return res;
        }

        async Task<APIResponse<List<SubmittedAndUnsubmittedStudents>>> IHomeAssessmentService.GetHomeAssessmentRecord(string homeAssessmentId)
        {
            var res = new APIResponse<List<SubmittedAndUnsubmittedStudents>>();
            var result = await context.HomeAssessment
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
        async Task<APIResponse<bool>> IHomeAssessmentService.IncludeClassAssessmentToScoreEntry(string homeAssessmentId)
        {
            var res = new APIResponse<bool>();
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive == true).SessionTermId;
            var assessment = await context.HomeAssessment
                .Include(x => x.HomeAssessmentFeedBacks)
                .Include(x => x.SessionClass).ThenInclude(x => x.Students).ThenInclude(x => x.User)
                .Include(x => x.SessionClassSubject)
                .FirstOrDefaultAsync(x => x.HomeAssessmentId == Guid.Parse(homeAssessmentId));

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
                        Name = x.User.FirstName + " " + x.User.LastName,
                        FeedBackId = assessment.HomeAssessmentFeedBacks.FirstOrDefault(s => s.StudentContactId == x.StudentContactId)?.HomeAssessmentFeedBackId
                }
                ).ToList();

            foreach(var std in students)
            {
                var feedBack = context.HomeAssessmentFeedBack.FirstOrDefault(x => x.HomeAssessmentFeedBackId == std.FeedBackId);
                if (feedBack is null)
                {
                    continue;
                }
                if (feedBack.Included)
                {
                    continue;
                }
                var scoreEntry = context.ScoreEntry.FirstOrDefault(s => s.SessionTermId == termId && s.StudentContactId == std.StudentId && s.ClassScoreEntry.SubjectId == std.SubjectId);
                if(scoreEntry is null)
                {
                    scoreEntry = new ScoreEntry();
                    scoreEntry.SessionTermId = termId;
                    scoreEntry.ClassScoreEntryId = context.ClassScoreEntry.FirstOrDefault(x => x.SubjectId == std.SubjectId && x.SessionClassId == assessment.SessionClassId).ClassScoreEntryId;
                    scoreEntry.AssessmentScore = Convert.ToInt16(std.Score);
                    scoreEntry.IsOffered = true;
                    scoreEntry.IsSaved = true;
                    scoreEntry.StudentContactId = std.StudentId;
                    context.ScoreEntry.Add(scoreEntry);
                    feedBack.Included = true;
                    await context.SaveChangesAsync();
                }
                else
                {
                    var score = scoreEntry.AssessmentScore + Convert.ToInt16(std.Score);
                   
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
                    scoreEntry.AssessmentScore = score;
                    scoreEntry.IsOffered = true;
                    scoreEntry.IsSaved = true;
                    feedBack.Included = true;
                    await context.SaveChangesAsync();
                }
            }
            res.Message.FriendlyMessage = "Records included successfully";
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<bool>> IHomeAssessmentService.IncludeStudentAssessmentToScoreEntry(string homeAssessmentFeedbackId)
        {
            var res = new APIResponse<bool>();
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive).SessionTermId;
            
            var feedBack = context.HomeAssessmentFeedBack
                .Include(x => x.StudentContact).ThenInclude(x => x.SessionClass)
                .Include(x => x.HomeAssessment)
                .ThenInclude(x => x.SessionClassSubject).FirstOrDefault(x => x.HomeAssessmentFeedBackId == Guid.Parse(homeAssessmentFeedbackId));
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
            if (feedBack.Included)
            {
                res.Message.FriendlyMessage = $"Feedback has already be included to score entry";
                return res;
            }
            var scoreEntry = context.ScoreEntry.FirstOrDefault(s => s.SessionTermId == termId && s.StudentContactId == feedBack.StudentContactId && s.ClassScoreEntry.SubjectId == feedBack.HomeAssessment.SessionClassSubject.SubjectId);
            if (scoreEntry is null)
            {
                scoreEntry = new ScoreEntry();
                scoreEntry.SessionTermId = termId;
                scoreEntry.ClassScoreEntryId = context.ClassScoreEntry.FirstOrDefault(x => x.SubjectId == feedBack.HomeAssessment.SessionClassSubject.SubjectId && x.SessionClassId == feedBack.HomeAssessment.SessionClassId).ClassScoreEntryId;
                scoreEntry.AssessmentScore = Convert.ToInt16(feedBack.Mark);
                scoreEntry.IsOffered = true;
                scoreEntry.IsSaved = true;
                scoreEntry.StudentContactId = feedBack.StudentContactId;
                context.ScoreEntry.Add(scoreEntry);
                feedBack.Included = true;
                await context.SaveChangesAsync();
            }
            else
            {
                var score = scoreEntry.AssessmentScore + Convert.ToInt16(feedBack.Mark);

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
                scoreEntry.AssessmentScore = score;
                scoreEntry.IsOffered = true;
                scoreEntry.IsSaved = true;
                feedBack.Included = true;
                await context.SaveChangesAsync();
            }
            res.Message.FriendlyMessage = "Records included successfully";
            res.IsSuccessful = true;
            return res;
        }

        private string IncludeStudentAssessmentToScoreEntry(HomeAssessmentFeedBack feedBack)
        {
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive).SessionTermId;

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
            var scoreEntry = context.ScoreEntry.FirstOrDefault(s => s.SessionTermId == termId && s.StudentContactId == feedBack.StudentContactId && s.ClassScoreEntry.SubjectId == feedBack.HomeAssessment.SessionClassSubject.SubjectId);
            if (scoreEntry is null)
            {
                scoreEntry = new ScoreEntry();
                scoreEntry.SessionTermId = termId;
                scoreEntry.ClassScoreEntryId = context.ClassScoreEntry.FirstOrDefault(x => x.SubjectId == feedBack.HomeAssessment.SessionClassSubject.SubjectId && x.SessionClassId == feedBack.HomeAssessment.SessionClassId).ClassScoreEntryId;
                scoreEntry.AssessmentScore = Convert.ToInt16(feedBack.Mark);
                scoreEntry.IsOffered = true;
                scoreEntry.IsSaved = true;
                scoreEntry.StudentContactId = feedBack.StudentContactId;
                context.ScoreEntry.Add(scoreEntry);
                feedBack.Included = true;
            }
            else
            {
                var score = scoreEntry.AssessmentScore + Convert.ToInt16(feedBack.Mark);

                if (feedBack.Mark > feedBack.StudentContact.SessionClass.AssessmentScore)
                {
                    return $"Assessment score can not be more than class assessment ({feedBack.StudentContact.SessionClass.AssessmentScore})";
                }

                if (score > feedBack.StudentContact.SessionClass.AssessmentScore)
                {
                    return $"Previously scored assessment + {Convert.ToInt16(feedBack.Mark)} can not be more than class assessment ({feedBack.StudentContact.SessionClass.AssessmentScore})";
                    
                }
                scoreEntry.AssessmentScore = score;
                scoreEntry.IsOffered = true;
                scoreEntry.IsSaved = true;
                feedBack.Included = true;
            }
            return "success";
        }

    }
}
