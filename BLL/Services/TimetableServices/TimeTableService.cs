using BLL;
using Contracts.Class;
using DAL;
using Microsoft.EntityFrameworkCore;
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
    public class TimeTableService : ITimeTableService
    {
        private readonly DataContext context;
        public TimeTableService(DataContext context)
        {
            this.context = context;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> ITimeTableService.GetAllActiveClassesAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var activeClasses =  context.ClassLookUp.Where(d => d.Deleted != true && d.IsActive == true);

            if (activeClasses.Count() == context.ClassTimeTable.Count())
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
            var noneAddedClassIds = context.ClassLookUp.Where(s => !context.ClassTimeTable.Select(d => d.ClassId).AsEnumerable().Contains(s.ClassLookupId)
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

        public async Task<APIResponse<CreateClassTimeTableTimeActivity>> CreateClassTimeTableTimeActivityAsync(CreateClassTimeTableTimeActivity request)
        {
            var res = new APIResponse<CreateClassTimeTableTimeActivity>();

            try
            {
                var req = new ClassTimeTableTimeActivity
                {
                    Activity = request.Activity,
                    ClassTimeTableTimeId = Guid.Parse(request.ClassTimeTableTimeId),
                    ClassTimeTableDayId = Guid.Parse(request.ClassTimeTableDayId)
                };

                await context.ClassTimeTableTimeActivity.AddAsync(req);
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

      
        public async Task<APIResponse<List<GetClassTimeActivity>>> GetClassTimeTableAsync(Guid classId)
        {
            var res = new APIResponse<List<GetClassTimeActivity>>();

            var result = await context.ClassTimeTable
                .Include(s => s.Class)
                .Include(s => s.Days)
                 .Include(s => s.Times).ThenInclude(d => d.Activities)
                .Where(d => d.Deleted == false && d.ClassId == classId)
                .Select(f => new GetClassTimeActivity(f)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        public async Task<APIResponse<List<GetClassTimeActivityByDay>>> GetClassTimeActivityByDayAsync(string day)
        {
            var res = new APIResponse<List<GetClassTimeActivityByDay>>();
            var actualDay = context.ClassTimeTableDay.Where(x => x.Day == day).FirstOrDefault();

            var result = await context.ClassTimeTable
                .Include(s => s.Days)
                 .Include(s => s.Times).ThenInclude(d => d.Activities)
                .Where(d => d.Deleted == false && d.Days.Contains(actualDay))
                .Select(f => new GetClassTimeActivityByDay(f, actualDay)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }
    }
}
