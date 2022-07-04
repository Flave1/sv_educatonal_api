using BLL;
using BLL.Utilities;
using Contracts.AttendanceContract;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.DAL.Models.Attendance;
using SMP.DAL.Models.Register;
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

        async Task<APIResponse<ClassRegister>> IAttendanceService.CreateClassRegisterAsync(Guid SessionClassId)
        {
            var res = new APIResponse<ClassRegister>();

            var reg = new ClassRegister
            {
                SessionClassId = SessionClassId,
                RegisterLabel = $"ATTENDANCE AS AT {DateTime.UtcNow}",
            };
            await context.ClassRegister.AddAsync(reg);
            await context.SaveChangesAsync();

            res.Result = reg;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.Created;
            return res;
        }
        async Task<APIResponse<PostStudentAttendance>> IAttendanceService.UpdateStudentAttendanceRecord(PostStudentAttendance attendance)
        {
            var res = new APIResponse<PostStudentAttendance>();
            var attendanceRecord = await context.StudentAttendance.FirstOrDefaultAsync(x => x.ClassRegisterId == attendance.ClassRegisterId 
            && x.StudentContactId == attendance.StudentContactId);

            if (attendance.IsPresent && attendanceRecord == null)
            {
                attendanceRecord = new StudentAttendance()
                {
                    ClassRegisterId = attendance.ClassRegisterId,
                    StudentContactId = attendance.StudentContactId,
                };
                await context.StudentAttendance.AddAsync(attendanceRecord);
                await context.SaveChangesAsync();
            }
            else
            {
                if (attendanceRecord != null)
                {
                    context.StudentAttendance.Remove(attendanceRecord);
                    await context.SaveChangesAsync();
                }
            }
            res.Result = attendance;
            res.IsSuccessful = true;
            return res;
        }
        async Task<APIResponse<List<GetAttendance>>> IAttendanceService.ContinueAttendanceAsync(Guid ClassRegisterId)
        {
            var res = new APIResponse<List<GetAttendance>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var register = await context.ClassRegister
                .Include(d => d.StudentAttendances)
                .Include(s => s.SessionClass).ThenInclude(d => d.Students).ThenInclude(s => s.User)
                .Where(d => d.ClassRegisterId == ClassRegisterId).Select(s => new GetAttendance(s, regNoFormat)).ToListAsync();
            if(register == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            res.Message.FriendlyMessage = Messages.Created;
            res.Result = register;
            res.IsSuccessful = true;
            return res;
        }
          
        public async Task<APIResponse<List<GetAttendance>>> GetAllAttendanceRegisterAsync()
        {
            var res = new APIResponse<List<GetAttendance>>();
 
            var result = await context.ClassRegister
                .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(q => q.StudentAttendances)
                .OrderByDescending(d => d.SessionClass.SessionClassId)
                .Where(d => d.Deleted == false)
                .Select(f => new GetAttendance(f)).ToListAsync();

            if (!result.Any())
                res.Message.FriendlyMessage = "No Attendance Recorded";
                res.IsSuccessful=false;
             
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        public async Task<APIResponse<GetPresentStudentAttendance>> PresentStudentAsync(PostStudentAttendance attendance)
        {
            var res = new APIResponse<GetPresentStudentAttendance>();
            var attendances = await context.StudentAttendance.Include(q => q.SessionClass).Where(x => x.ClassRegisterId == attendance.ClassRegisterId && attendance.IsPresent == true).ToListAsync();
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

        public async Task<APIResponse<GetPresentStudentAttendance>> AbsentStudentAsync(PostStudentAttendance attendance)
        {
            var res = new APIResponse<GetPresentStudentAttendance>();
            var attendances = await context.StudentAttendance.Include(q => q.SessionClass).Where(x => x.ClassRegisterId == attendance.ClassRegisterId && attendance.IsPresent == false).ToListAsync();
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
