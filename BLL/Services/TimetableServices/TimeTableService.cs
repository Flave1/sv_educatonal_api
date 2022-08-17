using BLL;
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

        public async Task<APIResponse<CreateClassTimeTable>> CreateClassTimeTableAsync(CreateClassTimeTable request)
        {
            var res = new APIResponse<CreateClassTimeTable>();

            try
            {
                var req = new ClassTimeTable
                {
                    ClassId = Guid.Parse(request.ClassId)
                };

                await context.ClassTimeTable.AddAsync(req);
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

        public async Task<APIResponse<List<GetClassActivity>>> GetClassTimeTableAsync(Guid classId)
        {
            var res = new APIResponse<List<GetClassActivity>>();

            var result = await context.ClassTimeTable
                //.Include(s => s.ClassId)
                .Include(s => s.Class)
                .Include(s => s.Days)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false && d.ClassId == classId)
                .Select(f => new GetClassActivity(f)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }
    }
}
