using BLL;
using BLL.Utilities;
using Contracts.AttendanceContract;
using Contracts.Common;
using DAL;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.Contracts.Assessment;
using SMP.DAL.Models.AssessmentEntities;
using SMP.DAL.Models.Attendance;
using SMP.DAL.Models.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AssessmentServices
{
    public class HomeAssessmentService : IHomeAssessmentService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        public HomeAssessmentService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }
        async Task<APIResponse<CreateHomeAssessmentRequest>> IHomeAssessmentService.CreateHomeAssessmentAsync(CreateHomeAssessmentRequest request)
        {
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
                        DeadLine = request.DeadLine,
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
                        DeadLine = request.DeadLine,
                    };
                    await context.HomeAssessment.AddAsync(reg);
                }

                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        async Task<APIResponse<UpdateHomeAssessmentRequest>> IHomeAssessmentService.UpdateHomeAssessmentAsync(UpdateHomeAssessmentRequest request)
        {
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
                assessment.DeadLine = request.DeadLine;
                assessment.SessionClassGroupId = request.SessionClassGroupId == "all-students" ? Guid.Parse("eba102ba-d96c-4920-812a-080c8fdbe767") :
                    Guid.Parse(request.SessionClassGroupId); //DO NOT CHANGE ID PLEASE....
                assessment.SessionClassSubjectId = Guid.Parse(request.SessionClassSubjectId);
                assessment.SessionTermId = context.SessionTerm.FirstOrDefault(s => s.IsActive == true).SessionTermId;
                assessment.Type = (int)AssessmentTypes.HomeAssessment;
                assessment.Title = request.Title;
                assessment.Status = request.ShouldSendToStudents ? assessment.Status : (int)HomeAssessmentStatus.Saved;

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

        async Task<APIResponse<List<GetHomeAssessmentRequest>>> IHomeAssessmentService.GetSubjectHomeAssessmentAsync(Guid SessionClassSubjectId)
        {
            var res = new APIResponse<List<GetHomeAssessmentRequest>>();

            var result = await context.HomeAssessment
                .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                 .Include(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                 .Include(q => q.SessionTerm)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false && d.SessionClassSubjectId == SessionClassSubjectId)
                .Select(f => new GetHomeAssessmentRequest(f, f.SessionClass.Students.Count())).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }


        async Task<APIResponse<bool>> IHomeAssessmentService.DeleteHomeAssessmentAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var result = await context.HomeAssessment
                .Include(d => d.AssessmentScoreRecord)
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
            catch (Exception ex)
            {

                throw ex;
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
                 .Include(q => q.AssessmentScoreRecord)
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
            var selectedClass = context.SessionClass.FirstOrDefault(s => s.SessionClassId == sessionClasId);
            var homeAssessment = context.HomeAssessment.Where(d => d.SessionClassSubjectId == sessionClassSubjectId);
            var classAssessment = context.ClassAssessment.Where(d => d.SessionClassSubjectId == sessionClassSubjectId);
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

        async Task<APIResponse<List<StudentHomeAssessmentRequest>>> IHomeAssessmentService.FilterHomeAssessmentsByStudentAsync(int status)
        {
            var studentContactid = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId").Value;
            var res = new APIResponse<List<StudentHomeAssessmentRequest>>();
            res.Result = new List<StudentHomeAssessmentRequest>();
            var student = context.StudentContact.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactid));
            var result = new List<StudentHomeAssessmentRequest>();
            if(status == -1)
            {
                 result = await context.HomeAssessment
                    .Include(d => d.HomeAssessmentFeedBacks)
                    .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                    .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                    .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                     .Include(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                     .Include(q => q.SessionTerm)
                    .OrderByDescending(d => d.CreatedOn)
                    .Where(d => d.Deleted == false  && d.SessionClassId == student.SessionClassId && d.Status == (int)HomeAssessmentStatus.Opened)
                    .Select(f => new StudentHomeAssessmentRequest(f, studentContactid)).ToListAsync();
                //&& d.SessionClassGroup.GroupName == "all-students"  && d.Status == (int)HomeAssessmentStatus.Opened
            }
            else
            {
                 result = await context.HomeAssessment
                  .Include(d => d.HomeAssessmentFeedBacks)
                  .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                  .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                  .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                   .Include(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                   .Include(q => q.SessionTerm)
                  .OrderByDescending(d => d.CreatedOn)
                  .Where(d => d.Deleted == false  && d.SessionClassId == student.SessionClassId && d.Status == (int)HomeAssessmentStatus.Opened
                   && d.Status == status)
                  .Select(f => new StudentHomeAssessmentRequest(f, studentContactid)).ToListAsync();
            }
            

            result.ForEach(d =>
            {
                if (!string.IsNullOrEmpty(d.ListOfStudentContactIds) && d.ListOfStudentContactIds.Split(',').ToList().Contains(studentContactid))
                    res.Result.Add(d);
            });

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
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
                    reg = context.HomeAssessmentFeedBack.FirstOrDefault(d => d.HomeAssessmentFeedBackId == Guid.Parse(request.HomeAssessmentFeedBackId));
                    if(reg is null)
                    {
                        res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                        return res;
                    }

                    if(reg.Status == (int)HomeAssessmentStatus.Submitted)
                    {
                        res.Message.FriendlyMessage = "Assignment has already been submited";
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
            catch (Exception ex)
            {
                throw ex;
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
                var feedBack = context.HomeAssessmentFeedBack.Include(d => d.HomeAssessment)
                    .FirstOrDefault(d => d.HomeAssessmentFeedBackId == Guid.Parse(request.HomeAssessmentFeedBackId));
                if (feedBack is null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
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

                var score = await context.AssessmentScoreRecord.FirstOrDefaultAsync(d => d.HomeAssessmentId == feedBack.HomeAssessmentId && d.StudentContactId == feedBack.StudentContactId);

                if(score is null)
                {
                    score = new AssessmentScoreRecord()
                    {
                        AssessmentType = (int)AssessmentTypes.HomeAssessment,
                        Score = request.Score,
                        StudentContactId = feedBack.StudentContactId,
                        HomeAssessmentId = feedBack.HomeAssessmentId,
                    };
                    context.AssessmentScoreRecord.Add(score);
                }
                else
                {
                    score.Score = request.Score;
                }

                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Saved;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
