using BLL;
using BLL.Utilities;
using Contracts.AttendanceContract;
using Contracts.Common;
using DAL;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
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
        public HomeAssessmentService(DataContext context)
        {
            this.context = context;
        }
        async Task<APIResponse<CreateHomeAssessmentRequest>> IHomeAssessmentService.CreateHomeAssessmentAsync(CreateHomeAssessmentRequest request)
        {
            var res = new APIResponse<CreateHomeAssessmentRequest>();
            try
            {
                var reg = new HomeAssessment
                {
                    SessionClassId = Guid.Parse(request.SessionClassId),
                    AssessmentScore = request.AssessmentScore,
                    Content = request.Content,
                    SessionClassGroupId = Guid.Parse(request.SessionClassGroupId),
                    SessionClassSubjectId = Guid.Parse(request.SessionClassSubjectId),
                    SessionTermId = context.SessionTerm.FirstOrDefault(s => s.IsActive == true).SessionTermId,
                    Status = request.ShouldSendToStudents ? (int)HomeAssessmentStatus.Opened : (int)HomeAssessmentStatus.Saved,
                    Type = (int)AssessmentTypes.HomeAssessment,
                    Title = request.Title,
                };

                await context.HomeAssessment.AddAsync(reg);
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
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var assessment = await context.HomeAssessment.FirstOrDefaultAsync(d => d.HomeAssessmentId == Guid.Parse(request.HomeAssessmentId));
            if (assessment is null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            assessment.AssessmentScore = request.AssessmentScore;
            assessment.Content = request.Content;
            assessment.SessionClassGroupId = Guid.Parse(request.SessionClassGroupId);
            assessment.SessionClassSubjectId = Guid.Parse(request.SessionClassSubjectId);
            assessment.SessionTermId = context.SessionTerm.FirstOrDefault(s => s.IsActive == true).SessionTermId;
            assessment.Type = (int)AssessmentTypes.HomeAssessment;
            assessment.Title = request.Title;


            await context.HomeAssessment.AddAsync(assessment);
            await context.SaveChangesAsync();

            res.Result = request;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.Updated;
            return res;
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
                .Select(f => new GetHomeAssessmentRequest(f)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }


        async Task<APIResponse<bool>> IHomeAssessmentService.DeleteHomeAssessmentAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var result = await context.HomeAssessment.FirstOrDefaultAsync(x => x.HomeAssessmentId == Guid.Parse(request.Item));

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

        async Task<APIResponse<List<GetHomeAssessmentRequest>>> IHomeAssessmentService.GetSingleHomeAssessmentAsync(Guid homeAssessmentId, Guid sessionClasId)
        {
            var res = new APIResponse<List<GetHomeAssessmentRequest>>();

            var studentsInClass = context.SessionClass.Include(s => s.Students).Where(d => d.SessionClassId == sessionClasId).SelectMany(s => s.Students).ToList();
            var result = await context.HomeAssessment
                   .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(q => q.SessionClassSubject).ThenInclude(s => s.Subject)
                 .Include(q => q.SessionClassGroup).ThenInclude(s => s.SessionClass)
                 .Include(q => q.SessionTerm)
                 .Include(q => q.HomeAssessmentFeedBacks).ThenInclude(d => d.StudentContact).ThenInclude(s => s.User)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false && d.HomeAssessmentId == homeAssessmentId)
                .Select(f => new GetHomeAssessmentRequest(f, studentsInClass)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }



    }
}
