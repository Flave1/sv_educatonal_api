using BLL;
using BLL.LoggerService;
using Contracts.Class;
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
    public class TimeTableService : ITimeTableService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        public TimeTableService(DataContext context, IHttpContextAccessor accessor, ILoggerService loggerService)
        {
            this.context = context;
            this.accessor = accessor;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> ITimeTableService.GetAllActiveClassesAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var activeClasses =  context.ClassLookUp.Where(d => d.ClientId == smsClientId && d.Deleted != true && d.IsActive == true);

            if (activeClasses.Count() == context.ClassTimeTable.Where(x => x.ClientId == smsClientId).Count())
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
                    var req = new ClassTimeTable
                    {
                        ClassId = id
                    };
                    await context.ClassTimeTable.AddAsync(req);
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

        public async Task<APIResponse<CreateClassTimeTableDay>> CreateClassTimeTableDayAsync(CreateClassTimeTableDay request)
        {
            var res = new APIResponse<CreateClassTimeTableDay>();

            
            try
            {

                var req = new ClassTimeTableDay
                {
                    Day = request.Day,
                    ClassTimeTableId = Guid.Parse(request.ClassTimeTableId)
                };

                await context.ClassTimeTableDay.AddAsync(req);
                await context.SaveChangesAsync();

                var classTimes = context.ClassTimeTableTime.Where(d => d.ClientId == smsClientId && d.ClassTimeTableId == Guid.Parse(request.ClassTimeTableId)).AsEnumerable();
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw ex;
            }
        }

        public async Task<APIResponse<CreateClassTimeTableTime>> CreateClassTimeTableTimeAsync(CreateClassTimeTableTime request)
        {
            var res = new APIResponse<CreateClassTimeTableTime>();
            try
            {

                var req = new ClassTimeTableTime
                {
                    Start = request.Start,
                    End = request.End,
                    ClassTimeTableId = Guid.Parse(request.ClassTimeTableId)
                };
                await context.ClassTimeTableTime.AddAsync(req);
                await context.SaveChangesAsync();

                var classDays = context.ClassTimeTable.Where(d => d.ClientId == smsClientId && d.ClassId == Guid.Parse(request.ClassId)).SelectMany(s => s.Days).AsEnumerable();
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw ex;
            }
        }

        async Task<APIResponse<UpdateClassTimeTableTime>> ITimeTableService.UpdateClassTimeTableTimeAsync(UpdateClassTimeTableTime request)
        {
            var res = new APIResponse<UpdateClassTimeTableTime>();
            try
            {
                var time = context.ClassTimeTableTime.FirstOrDefault(x => x.ClientId == smsClientId && x.ClassTimeTableTimeId == Guid.Parse(request.ClassTimeTableTimeId));

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


        async Task<APIResponse<UpdateClassTimeTableDay>> ITimeTableService.UpdateClassTimeTableDayAsync(UpdateClassTimeTableDay request)
        {
            var res = new APIResponse<UpdateClassTimeTableDay>();
            try
            {
                var day = context.ClassTimeTableDay.FirstOrDefault(d => d.ClientId == smsClientId && d.ClassTimeTableDayId == Guid.Parse(request.ClassTimeTableDayId));
                if(day == null)
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

        public async Task<APIResponse<UpdateClassTimeTableTimeActivity>> UpdateClassTimeTableTimeActivityAsync(UpdateClassTimeTableTimeActivity request)
        {
            var res = new APIResponse<UpdateClassTimeTableTimeActivity>();

            try
            {
                var req = context.ClassTimeTableTimeActivity.FirstOrDefault(d => d.ClientId == smsClientId && d.ClassTimeTableTimeActivityId == Guid.Parse(request.ActivityId));
                if(req is not null)
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

        public async Task<APIResponse<GetClassTimeActivity>> GetClassTimeTableAsync(Guid classId)
        {
            var res = new APIResponse<GetClassTimeActivity>();

            var result = await context.ClassTimeTable
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.ClassId == classId)
                .Include(s => s.Class)
                .Include(s => s.Days).ThenInclude(s => s.Activities).ThenInclude(d => d.Day)
                 .Include(s => s.Times).ThenInclude(d => d.Activities).ThenInclude(d => d.Day)
                .Select(f => new GetClassTimeActivity(f)).FirstOrDefaultAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        public async Task<APIResponse<List<GetClassTimeActivityByDay>>> GetClassTimeActivityByDayAsync(string day)
        {
            var res = new APIResponse<List<GetClassTimeActivityByDay>>();
            var classList = context.ClassTimeTable.Where(x => x.ClientId == smsClientId && x.Deleted == false).Include(d => d.Class).ToList();

            var result = await context.ClassTimeTable.Where(x => x.ClientId == smsClientId)
                .Include(s => s.Days)
                 .Include(s => s.Times).ThenInclude(d => d.Activities)
                .Where(d => d.Deleted == false && d.Days.Select(d => d.Day).Contains(day))
                .Select(f => new GetClassTimeActivityByDay(f, classList)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }
        async Task<APIResponse<SingleDelete>> ITimeTableService.DeleteClassTimeTableTimeAsync(SingleDelete request)
        {
            var res = new APIResponse<SingleDelete>();
            try
            {
                var classDays = context.ClassTimeTableTime.Where(x=>x.ClientId == smsClientId).Include(d => d.Activities).FirstOrDefault(d => d.ClassTimeTableTimeId == Guid.Parse(request.Item));
                context.ClassTimeTableTime.Remove(classDays); 
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

        async Task<APIResponse<SingleDelete>> ITimeTableService.DeleteClassTimeTableDayAsync(SingleDelete request)
        {
            var res = new APIResponse<SingleDelete>();
            try
            {
                var day = context.ClassTimeTableDay.Where(x=>x.ClientId == smsClientId).Include(d => d.Activities).FirstOrDefault(d => d.ClassTimeTableDayId == Guid.Parse(request.Item));
                if(day is not null)
                {
                    if(day.Activities.Any())
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw ex;
            }
        }
        public async Task<APIResponse<List<GetClassDays>>> GetAllClassDaysAsync()
        {
            var res = new APIResponse<List<GetClassDays>>();
            res.Result = await context.ClassTimeTableDay.Where(x => x.ClientId == smsClientId).Distinct().Select(d => new GetClassDays(d)).ToListAsync();
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetClassTimeActivity>> ITimeTableService.GetClassTimeTableByStudentAsync()
        {
            var res = new APIResponse<GetClassTimeActivity>();
            var studentContactId = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId")?.Value;
            if (!string.IsNullOrEmpty(studentContactId))
            {
                var studentAct = context.StudentContact.Where(x => x.ClientId == smsClientId).Include(s => s.SessionClass).FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactId));
                var result = await context.ClassTimeTable.Where(x => x.ClientId == smsClientId)
                  .Include(s => s.Class)
                  .Include(s => s.Days).ThenInclude(s => s.Activities).ThenInclude(d => d.Day)
                   .Include(s => s.Times).ThenInclude(d => d.Activities).ThenInclude(d => d.Day)
                  .Where(d => d.Deleted == false && d.ClassId == studentAct.SessionClass.ClassId)
                  .Select(f => new GetClassTimeActivity(f)).FirstOrDefaultAsync();
                res.Result = result;
            }
           
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetClassTimeActivity>> ITimeTableService.GetClassTimeTableByParentsAsync(Guid classlkpId)
        {
            var res = new APIResponse<GetClassTimeActivity>();
            var result = await context.ClassTimeTable.Where(x => x.ClientId == smsClientId)
              .Include(s => s.Class)
              .Include(s => s.Days).ThenInclude(s => s.Activities).ThenInclude(d => d.Day)
               .Include(s => s.Times).ThenInclude(d => d.Activities).ThenInclude(d => d.Day)
              .Where(d => d.Deleted == false && d.ClassId == classlkpId)
              .Select(f => new GetClassTimeActivity(f)).FirstOrDefaultAsync();
            res.Result = result;

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

    }
}
