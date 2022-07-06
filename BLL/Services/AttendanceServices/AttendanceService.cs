using BLL;
using BLL.Utilities;
using Contracts.AttendanceContract;
using Contracts.Common;
using DAL;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor accessor;

        public AttendanceService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        async Task<APIResponse<List<GetAttendance>>> IAttendanceService.CreateClassRegisterAsync(Guid SessionClassId)
        {
            var res = new APIResponse<List<GetAttendance>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var reg = new ClassRegister
            {
                SessionClassId = SessionClassId,
                RegisterLabel = $"ATTENDANCE AS AT {DateTime.UtcNow}",
            };
            await context.ClassRegister.AddAsync(reg);
            await context.SaveChangesAsync();

            var classAttendance = await context.ClassRegister.Include(s => s.SessionClass).ThenInclude(d => d.Students).ThenInclude(d => d.User).Where(d =>
            d.ClassRegisterId == reg.ClassRegisterId)
                .Select(s => new GetAttendance(s, regNoFormat)).ToListAsync();

            res.Result = classAttendance;
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
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false)
                .Select(f => new GetAttendance(f)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<AttendanceList>>> IAttendanceService.GetAllStudentPresentAsync(Guid classRegisterId)
        {
            var res = new APIResponse<List<AttendanceList>> ();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var classRegister = await context.ClassRegister
                .Include(s => s.StudentAttendances).ThenInclude(q => q.StudentContact).ThenInclude(x => x.User)
                .Where(x => x.ClassRegisterId == classRegisterId).FirstOrDefaultAsync();
            
            if (classRegister == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            var presentStudent = classRegister.StudentAttendances.Select(x => x.StudentContact).Select(d => new AttendanceList(d, regNoFormat, true)).ToList();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true; 
            res.Result = presentStudent;
            return res;
        }

        async Task<APIResponse<List<AttendanceList>>> IAttendanceService.GetAllAbsentStudents(Guid classRegisterId)
        {
            var res = new APIResponse<List<AttendanceList>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var classRegister = await context.ClassRegister
                .Include(q => q.SessionClass).ThenInclude(s => s.Students)
                .Include(q => q.StudentAttendances).ThenInclude(x=>x.StudentContact)
                .Where(x => x.ClassRegisterId == classRegisterId).FirstOrDefaultAsync();
           
            if (classRegister == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            var presentStudentIds = classRegister.StudentAttendances.Select(x=>x.ClassRegisterId).ToList();
            var absentStudents = classRegister.SessionClass.Students.Where(x => !presentStudentIds.Contains(x.StudentContactId)).Select(s => new AttendanceList(s, regNoFormat, false)).ToList();
           

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            res.Result = absentStudents;
            return res;

        }

        async Task<APIResponse<bool>> IAttendanceService.DeleteClassRegisterAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var result = await context.ClassRegister.FirstOrDefaultAsync(x => x.ClassRegisterId == Guid.Parse(request.Item));

            if (result != null)
            {
                result.Deleted = true;
                await context.SaveChangesAsync();
            }

            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            res.IsSuccessful = true;
            res.Result = true;
            return res;
        }
        async Task<APIResponse<UpdateClassRegister>> IAttendanceService.UpdateClassRegisterLabel(UpdateClassRegister request)
        {
            var res = new APIResponse<UpdateClassRegister>();
            var result = await context.ClassRegister
                .FirstOrDefaultAsync(x => x.ClassRegisterId == request.ClassRegisterId && x.Deleted == false);

            if (result != null)
            {
                result.RegisterLabel = request.RegisterLabel;
                await context.SaveChangesAsync();
            }
            res.Message.FriendlyMessage = Messages.Updated;
            res.IsSuccessful = true;
            res.Result = request;
            return res;
        }
    }
}
