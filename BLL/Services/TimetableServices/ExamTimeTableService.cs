using BLL;
using BLL.LoggerService;
using Contracts.Class;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Polly;
using SMP.BLL.Constants;
using SMP.Contracts.Timetable;
using SMP.DAL.Models.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.TimetableServices
{
    public class ExamTimeTableService : IExamTimetableService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        public ExamTimeTableService(DataContext context, IHttpContextAccessor accessor, ILoggerService loggerService)
        {
            this.context = context;
            this.accessor = accessor;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }
        public async Task<APIResponse<List<GetApplicationLookups>>> GetAllActiveClassesAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            try
            {
                var activeClasses = context.ClassLookUp.Where(d => d.ClientId == smsClientId && d.Deleted != true && d.IsActive == true);

                var activeSessionTerm = await context.SessionTerm.Where(d => d.ClientId == smsClientId && d.Deleted != true && d.IsActive == true).FirstOrDefaultAsync();

                if (activeClasses.Count() == context.ExamTimeTable.Where(x => x.ClientId == smsClientId).Count())
                {
                    res.Result = await activeClasses.Select(a => new GetApplicationLookups
                    {
                        LookupId = a.ClassLookupId.ToString().ToLower(),
                        Name = a.Name,
                        IsActive = a.IsActive,
                        GradeLevelId = a.GradeGroupId.ToString(),
                    }).ToListAsync();

                    res.IsSuccessful = true;
                    return res;
                }
                var noneAddedClassIds = context.ClassLookUp.Where(s => s.ClientId == smsClientId && !context.ClassTimeTable.Select(d => d.ClassId).AsEnumerable().Contains(s.ClassLookupId)
                && s.Deleted != true && s.IsActive == true).Select(s => s.ClassLookupId).ToList();

                if (noneAddedClassIds.Any())
                {
                    foreach (var id in noneAddedClassIds)
                    {
                        var req = new ExamTimeTable
                        {
                            ClassId = id,
                            SessionTermId = activeSessionTerm.SessionTermId
                        };
                        await context.ExamTimeTable.AddAsync(req);
                    }
                    await context.SaveChangesAsync();
                }
                res.Result = await activeClasses.Select(a => new GetApplicationLookups
                {
                    LookupId = a.ClassLookupId.ToString().ToLower(),
                    Name = a.Name,
                    IsActive = a.IsActive,
                    GradeLevelId = a.GradeGroupId.ToString(),
                }).ToListAsync();

                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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

                var req = new ExamTimeTableDay
                {
                    Day = request.Day,
                    ExamTimeTableId = Guid.Parse(request.ExamTimeTableId)
                };

                await context.ExamTimeTableDay.AddAsync(req);
                await context.SaveChangesAsync();

                var classTimes = context.ExamTimeTableTime.Where(d => d.ClientId == smsClientId && d.ExamTimeTableId == Guid.Parse(request.ExamTimeTableId)).AsEnumerable();
                if (classTimes.Any())
                {
                    foreach (var time in classTimes)
                    {
                        var act = new ExamTimeTableTimeActivity
                        {
                            Activity = "",
                            ExamTimeTableTimeId = time.ExamTimeTableTimeId,
                            ExamTimeTableDayId = req.ExamTimeTableDayId
                        };
                        await context.ExamTimeTableTimeActivity.AddAsync(act);
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw ex;
            }
        }

        public async Task<APIResponse<CreateExamTimeTableTime>> CreateExamTimeTableTimeAsync(CreateExamTimeTableTime request)
        {
            var res = new APIResponse<CreateExamTimeTableTime>();
            try
            {

                var req = new ExamTimeTableTime
                {
                    Start = request.Start,
                    End = request.End,
                    ExamTimeTableId = Guid.Parse(request.ExamTimeTableId)
                };
                await context.ExamTimeTableTime.AddAsync(req);
                await context.SaveChangesAsync();

                var classDays = context.ExamTimeTable.Where(d => d.ClientId == smsClientId && d.ClassId == Guid.Parse(request.ClassId)).SelectMany(s => s.Days).AsEnumerable();
                if (classDays.Any())
                {
                    foreach (var day in classDays)
                    {
                        var act = new ExamTimeTableTimeActivity
                        {
                            Activity = "",
                            ExamTimeTableTimeId = req.ExamTimeTableTimeId,
                            ExamTimeTableDayId = day.ExamTimeTableDayId
                        };
                        await context.ExamTimeTableTimeActivity.AddAsync(act);
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw ex;
            }
        }
        public async Task<APIResponse<UpdateExamTimeTableTime>> UpdateExamTimeTableTimeAsync(UpdateExamTimeTableTime request)
        {
            var res = new APIResponse<UpdateExamTimeTableTime>();
            try
            {
                var time = context.ExamTimeTableTime.FirstOrDefault(x => x.ClientId == smsClientId && x.ExamTimeTableTimeId == Guid.Parse(request.ExamTimeTableTimeId));

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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        public async Task<APIResponse<UpdateExamTimeTableDay>> UpdateExamTimeTableDayAsync(UpdateExamTimeTableDay request)
        {
            var res = new APIResponse<UpdateExamTimeTableDay>();
            try
            {
                var day = context.ExamTimeTableDay.FirstOrDefault(d => d.ClientId == smsClientId && d.ExamTimeTableDayId == Guid.Parse(request.ExamTimeTableDayId));
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        public async Task<APIResponse<UpdateExamTimeTableTimeActivity>> UpdateExamTimeTableTimeActivityAsync(UpdateExamTimeTableTimeActivity request)
        {
            var res = new APIResponse<UpdateExamTimeTableTimeActivity>();

            try
            {
                var req = context.ExamTimeTableTimeActivity.FirstOrDefault(d => d.ClientId == smsClientId && d.ExamTimeTableTimeActivityId == Guid.Parse(request.ActivityId));
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        public async Task<APIResponse<GetExamTimeActivity>> GetExamTimeTableAsync(Guid classId)
        {
            var res = new APIResponse<GetExamTimeActivity>();
            try
            {
                var result = await context.ExamTimeTable
                    .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.ClassId == classId)
                    .Include(s => s.Class)
                    .Include(s => s.SessionTerm).Where(x=> x.SessionTerm.IsActive)
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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
                var classDays = context.ExamTimeTableTime.Where(x => x.ClientId == smsClientId).Include(d => d.Activities).FirstOrDefault(d => d.ExamTimeTableTimeId == Guid.Parse(request.Item));
                context.ExamTimeTableTime.Remove(classDays);
                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        public async Task<APIResponse<SingleDelete>> DeleteExamTimeTableDayAsync(SingleDelete request)
        {
            var res = new APIResponse<SingleDelete>();
            try
            {
                var day = context.ExamTimeTableDay.Where(x => x.ClientId == smsClientId).Include(d => d.Activities).FirstOrDefault(d => d.ExamTimeTableDayId == Guid.Parse(request.Item));
                if (day is not null)
                {
                    if (day.Activities.Any())
                    {
                        context.ExamTimeTableTimeActivity.RemoveRange(day.Activities);
                    }
                    context.ExamTimeTableDay.Remove(day);
                    await context.SaveChangesAsync();
                }
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw ex;
            }
        }

        public async Task<APIResponse<List<GetExamTimeActivityByDay>>> GetExamTimeActivityByDayAsync(string day)
        {
            var res = new APIResponse<List<GetExamTimeActivityByDay>>();
            try
            {
                var classList = context.ExamTimeTable.Where(x => x.ClientId == smsClientId && x.Deleted == false).Include(d => d.Class).ToList();

                var result = await context.ExamTimeTable.Where(x => x.ClientId == smsClientId)
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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
                var result = await context.ExamTimeTable.Where(x => x.ClientId == smsClientId)
                  .Include(s => s.Class)
                  .Include(s=>s.SessionTerm).Where(s=>s.SessionTerm.IsActive)
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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
                    var result = await context.ExamTimeTable.Where(x => x.ClientId == smsClientId)
                      .Include(s => s.Class)
                      .Include(s => s.SessionTerm).Where(s => s.SessionTerm.IsActive)
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }
}
