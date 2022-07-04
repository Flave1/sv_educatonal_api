using BLL;
using Contracts.AttendanceContract;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.DAL.Models.Attendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AttendanceServices
{
    public class AttendanceService: IAttendanceService
    {
        private readonly DataContext context;

        public AttendanceService(DataContext context)
        {
            this.context = context;
        }

        public async Task<APIResponse<Attendance>> CreateAttendanceAsync(PostAttendance attendance)
        {
            var res = new APIResponse<Attendance>();
            var student = context.StudentContact.FirstOrDefaultAsync(x=>x.StudentContactId == attendance.StudentContactId);
            if(student != null)
            {
                var attendances = await context.Attendance
               //.Include(s => s.StudentContact)
               //.ThenInclude(x => x.User)
               //.Include(q => q.SessionClass)
               .FirstOrDefaultAsync(x => x.ClassRegisterId == attendance.ClassRegisterId && x.StudentContactId == attendance.StudentContactId);

                var newAttendance = new Attendance()
                {
                    ClassRegisterId = attendance.ClassRegisterId,
                    StudentContactId = attendance.StudentContactId,
                    Deleted = false,

                };
                await context.Attendance.AddAsync(newAttendance);
                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = Messages.Cretaed;
                res.Result = newAttendance;
                res.IsSuccessful = true;

            }
            return res;
        }
        public async Task<APIResponse<Attendance>> UpdateAttendanceAsync(PostAttendance attendance)
        {
            var res = new APIResponse<Attendance>();
            var student = context.StudentContact.FirstOrDefaultAsync(x => x.StudentContactId == attendance.StudentContactId);
            if (student != null)
            {
                var attendances = await context.Attendance
                //.Include(s => s.StudentContact)
                //.ThenInclude(x => x.User)
                //.Include(q => q.SessionClass)
                .FirstOrDefaultAsync(x => x.ClassRegisterId == attendance.ClassRegisterId && x.StudentContactId == attendance.StudentContactId);
                if (attendances == null)
                {
                    res.Message.FriendlyMessage = "No record found";
                    attendance.IsPresent = false;
                }
                attendances.ClassRegisterId = attendance.ClassRegisterId;
                attendances.StudentContactId = attendance.StudentContactId;
                attendances.Deleted = false;
                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = Messages.Cretaed;
                res.Result = attendances;
                res.IsSuccessful = true;

            }
            return res;
        }
          
        public async Task<APIResponse<List<GetAttendance>>> GetAllAttendanceRegisterAsync()
        {
            var res = new APIResponse<List<GetAttendance>>();
 
            var result = await context.Attendance
                .Include(s=>s.StudentContact)
                .ThenInclude(x=>x.User)
                .Include(q => q.SessionClass)
                .OrderByDescending(d => d.SessionClass.SessionClassId)
                .Where(d => d.Deleted == false)
                .Select(f => new GetAttendance(f, f.StudentContact.User)).ToListAsync();
            if (!result.Any())
                res.Message.FriendlyMessage = "No Attendance Recorded";
                res.IsSuccessful=false;
             
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        public async Task<APIResponse<GetPresentStudentAttendance>> PresentStudentAsync(PostAttendance attendance)
        {
            var res = new APIResponse<GetPresentStudentAttendance>();
            var attendances = await context.Attendance.Include(q => q.SessionClass).Where(x => x.ClassRegisterId == attendance.ClassRegisterId && attendance.IsPresent == true).ToListAsync();
            var presentStudent = attendances.Count();
            var presentStudents = new GetPresentStudentAttendance
            {
                PresentStudent = presentStudent,
            };
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            res.Result = presentStudents;
            return res;


        }

        public async Task<APIResponse<GetPresentStudentAttendance>> AbsentStudentAsync(PostAttendance attendance)
        {
            var res = new APIResponse<GetPresentStudentAttendance>();
            var attendances = await context.Attendance.Include(q => q.SessionClass).Where(x => x.ClassRegisterId == attendance.ClassRegisterId && attendance.IsPresent == false).ToListAsync();
            var abentStudent = attendances.Count();
            var absentStudents = new GetPresentStudentAttendance
            {
                AbsentStudent = abentStudent,
            };
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            res.Result = absentStudents;
            return res;
        }
    }
}
