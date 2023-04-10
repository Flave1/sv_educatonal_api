using BLL;
using BLL.ClassServices;
using BLL.LoggerService;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.Contracts.Timetable;
using SMP.DAL.Models.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.TimetableServices
{
    public class ExamTimeTableService : IExamTimeTableService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        private readonly IClassService classService;
        public ExamTimeTableService(DataContext context, IHttpContextAccessor accessor, ILoggerService loggerService, IClassService classService)
        {
            this.context = context;
            this.accessor = accessor;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.classService = classService;
        }
        public async Task<APIResponse<List<GetActiveTimetableClasses>>> GetAllActiveClassesAsync()
        {
            var res = new APIResponse<List<GetActiveTimetableClasses>>();
            try
            {
                var activeClasses = context.ClassLookUp.Where(d => d.ClientId == smsClientId && d.Deleted != true && d.IsActive == true).ToList();

                var timeTableForAddedClasses = context.ClassTimeTable.Where(x => x.ClientId == smsClientId && x.TimetableType == (int)TimetableType.ExamTimetable).ToList();
                if (activeClasses.Count == timeTableForAddedClasses.Count)
                {
                    res.Result =  activeClasses.Select(a => new GetActiveTimetableClasses(a,  classService.GetSessionClassByLkp(a.ClassLookupId))).ToList();

                    res.IsSuccessful = true;
                    return res;
                }
                var noneAddedClassIds = context.ClassLookUp.Where(s => s.ClientId == smsClientId && !timeTableForAddedClasses.Select(d => d.ClassId).Contains(s.ClassLookupId)
                && s.Deleted != true && s.IsActive == true).Select(s => s.ClassLookupId).ToList();

                if (noneAddedClassIds.Any())
                {
                    foreach (var id in noneAddedClassIds)
                    {
                        var req = new ClassTimeTable
                        {
                            ClassId = id,
                            TimetableType = (int)TimetableType.ExamTimetable
                        };
                        await context.ClassTimeTable.AddAsync(req);
                    }
                    await context.SaveChangesAsync();
                }
                res.Result = activeClasses.Select(a => new GetActiveTimetableClasses(a, classService.GetSessionClassByLkp(a.ClassLookupId))).ToList();

                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
        public async Task<APIResponse<CreateExamTimeTableDay>> CreateExamTimeTableDayAsync(CreateExamTimeTableDay request)
        {
            var res = new APIResponse<CreateExamTimeTableDay>();
            try
            {
                var req = new ClassTimeTableDay
                {
                    Day = request.Day,
                    ClassTimeTableId = Guid.Parse(request.ExamTimeTableId)
                };

                await context.ClassTimeTableDay.AddAsync(req);
                await context.SaveChangesAsync();

                var classTimes = context.ClassTimeTableTime.Where(d => d.ClientId == smsClientId && d.ClassTimeTableId == Guid.Parse(request.ExamTimeTableId)).AsEnumerable();
                if (classTimes.Any())
                {
                    foreach (var time in classTimes)
                    {
                        var act = new ClassTimeTableTimeActivity
                        {
                            Activity = "",
                            ClassTimeTableTimeId = time.ClassTimeTableTimeId,
                            ClassTimeTableDayId = req.ClassTimeTableDayId
                        };
                        await context.ClassTimeTableTimeActivity.AddAsync(act);
                    }
                    await context.SaveChangesAsync();
                }


                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }

        public async Task<APIResponse<CreateExamTimeTableTime>> CreateExamTimeTableTimeAsync(CreateExamTimeTableTime request)
        {
            var res = new APIResponse<CreateExamTimeTableTime>();
            try
            {

                var req = new ClassTimeTableTime
                {
                    Start = request.Start,
                    End = request.End,
                    ClassTimeTableId = Guid.Parse(request.ExamTimeTableId)
                };
                await context.ClassTimeTableTime.AddAsync(req);
                await context.SaveChangesAsync();

                var classDays = context.ClassTimeTable.Where(d => d.ClientId == smsClientId && d.TimetableType == (int)TimetableType.ExamTimetable 
                && d.ClassId == Guid.Parse(request.ClassId)).SelectMany(s => s.Days).AsEnumerable();
                if (classDays.Any())
                {
                    foreach (var day in classDays)
                    {
                        var act = new ClassTimeTableTimeActivity
                        {
                            Activity = "",
                            ClassTimeTableTimeId = req.ClassTimeTableTimeId,
                            ClassTimeTableDayId = day.ClassTimeTableDayId
                        };
                        await context.ClassTimeTableTimeActivity.AddAsync(act);
                    }
                    await context.SaveChangesAsync();
                }

                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        public async Task<APIResponse<UpdateExamTimeTableTime>> UpdateExamTimeTableTimeAsync(UpdateExamTimeTableTime request)
        {
            var res = new APIResponse<UpdateExamTimeTableTime>();
            try
            {
                var time = context.ClassTimeTableTime.FirstOrDefault(x => x.ClientId == smsClientId && x.ClassTimeTableTimeId == Guid.Parse(request.ExamTimeTableTimeId));

                time.Start = request.Start;
                time.End = request.End;

                await context.SaveChangesAsync();


                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        public async Task<APIResponse<UpdateExamTimeTableDay>> UpdateExamTimeTableDayAsync(UpdateExamTimeTableDay request)
        {
            var res = new APIResponse<UpdateExamTimeTableDay>();
            try
            {
                var day = context.ClassTimeTableDay.FirstOrDefault(d => d.ClientId == smsClientId && d.ClassTimeTableDayId == Guid.Parse(request.ExamTimeTableDayId));
                if (day == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }
                day.Day = request.Day;

                await context.SaveChangesAsync();


                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        public async Task<APIResponse<UpdateExamTimeTableTimeActivity>> UpdateExamTimeTableTimeActivityAsync(UpdateExamTimeTableTimeActivity request)
        {
            var res = new APIResponse<UpdateExamTimeTableTimeActivity>();

            try
            {
                var req = context.ClassTimeTableTimeActivity.FirstOrDefault(d => d.ClientId == smsClientId && d.ClassTimeTableTimeActivityId == Guid.Parse(request.ActivityId));
                if (req is not null)
                {
                    req.Activity = request.Activity;
                    await context.SaveChangesAsync();
                }
                else
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }


                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Updated;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        public async Task<APIResponse<GetExamTimeActivity>> GetExamTimeTableAsync(Guid classId)
        {
            var res = new APIResponse<GetExamTimeActivity>();
            try
            {
                var result = await context.ClassTimeTable
                    .Where(d => d.ClientId == smsClientId && d.TimetableType == (int)TimetableType.ExamTimetable && d.Deleted == false && d.ClassId == classId)
                    .Include(s => s.Class)
                    .Include(s => s.Days).ThenInclude(s => s.Activities).ThenInclude(d => d.Day)
                     .Include(s => s.Times).ThenInclude(d => d.Activities).ThenInclude(d => d.Day)
                    .Select(f => new GetExamTimeActivity(f)).FirstOrDefaultAsync();

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = result;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
        public async Task<APIResponse<SingleDelete>> DeleteExamTimeTableTimeAsync(SingleDelete request)
        {
            var res = new APIResponse<SingleDelete>();
            try
            {
                var classDays = context.ClassTimeTableTime.Where(x => x.ClientId == smsClientId).Include(d => d.Activities).FirstOrDefault(d => d.ClassTimeTableTimeId == Guid.Parse(request.Item));
                context.ClassTimeTableTime.Remove(classDays);
                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        public async Task<APIResponse<SingleDelete>> DeleteExamTimeTableDayAsync(SingleDelete request)
        {
            var res = new APIResponse<SingleDelete>();
            try
            {
                var day = context.ClassTimeTableDay.Where(x => x.ClientId == smsClientId).Include(d => d.Activities).FirstOrDefault(d => d.ClassTimeTableDayId == Guid.Parse(request.Item));
                if (day is not null)
                {
                    if (day.Activities.Any())
                    {
                        context.ClassTimeTableTimeActivity.RemoveRange(day.Activities);
                    }
                    context.ClassTimeTableDay.Remove(day);
                    await context.SaveChangesAsync();
                }
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw ex;
            }
        }

        public async Task<APIResponse<List<GetExamTimeActivityByDay>>> GetExamTimeActivityByDayAsync(string day)
        {
            var res = new APIResponse<List<GetExamTimeActivityByDay>>();
            try
            {
                var classList = context.ClassTimeTable.Where(x => x.ClientId == smsClientId && x.TimetableType == (int)TimetableType.ExamTimetable && x.Deleted == false).Include(d => d.Class).ToList();

                var result = await context.ClassTimeTable.Where(x => x.ClientId == smsClientId && x.TimetableType == (int)TimetableType.ExamTimetable)
                    .Include(s => s.Days)
                     .Include(s => s.Times).ThenInclude(d => d.Activities)
                    .Where(d => d.Deleted == false && d.Days.Select(d => d.Day).Contains(day))
                    .Select(f => new GetExamTimeActivityByDay(f, classList)).ToListAsync();

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = result;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<GetExamTimeActivity>> GetExamTimeTableByParentsAsync(Guid classlkpId)
        {
            var res = new APIResponse<GetExamTimeActivity>();
            try
            {
                var result = await context.ClassTimeTable.Where(x => x.ClientId == smsClientId && x.TimetableType == (int)TimetableType.ExamTimetable)
                  .Include(s => s.Class)
                  .Include(s => s.Days).ThenInclude(s => s.Activities).ThenInclude(d => d.Day)
                   .Include(s => s.Times).ThenInclude(d => d.Activities).ThenInclude(d => d.Day)
                  .Where(d => d.Deleted == false && d.ClassId == classlkpId)
                  .Select(f => new GetExamTimeActivity(f)).FirstOrDefaultAsync();
                res.Result = result;

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<GetExamTimeActivity>> GetExamTimeTableByStudentAsync()
        {
            var res = new APIResponse<GetExamTimeActivity>();
            try
            {
                var studentContactId = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId")?.Value;
                if (!string.IsNullOrEmpty(studentContactId))
                {
                    var studentAct = context.StudentContact.Where(x => x.ClientId == smsClientId).Include(s => s.SessionClass).FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactId));
                    var result = await context.ClassTimeTable.Where(x => x.ClientId == smsClientId && x.TimetableType == (int)TimetableType.ExamTimetable)
                      .Include(s => s.Class)
                      .Include(s => s.Days).ThenInclude(s => s.Activities).ThenInclude(d => d.Day)
                       .Include(s => s.Times).ThenInclude(d => d.Activities).ThenInclude(d => d.Day)
                      .Where(d => d.Deleted == false && d.ClassId == studentAct.SessionClass.ClassId)
                      .Select(f => new GetExamTimeActivity(f)).FirstOrDefaultAsync();
                    res.Result = result;
                }

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<List<GetExamTimeActivity>>> GetAllExamTimeTableAsync()
        {
            var res = new APIResponse<List<GetExamTimeActivity>>();
            try
            {
                var result = await context.ClassTimeTable
                    .Where(d => d.ClientId == smsClientId && d.TimetableType == (int)TimetableType.ExamTimetable && d.Deleted == false)
                    .Include(s => s.Class)
                    .Include(s => s.Days).ThenInclude(s => s.Activities).ThenInclude(d => d.Day)
                     .Include(s => s.Times).ThenInclude(d => d.Activities).ThenInclude(d => d.Day)
                    .Select(f => new GetExamTimeActivity(f)).ToListAsync();

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = result;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }
}
