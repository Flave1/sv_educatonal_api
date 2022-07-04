using BLL;
using BLL.Utilities;
using Contracts.AttendanceContract;
using DAL;
using DAL.StudentInformation;
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

        async Task<APIResponse<List<StudentContact>>> IAttendanceService.PresentStudentAsync(Guid classRegisterId)
        {
            var res = new APIResponse<List<StudentContact>> ();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var present = await context.ClassRegister
                .Include(s => s.StudentAttendances)
                .ThenInclude(q => q.StudentContact).ThenInclude(x=>x.User)
                .Where(x => x.ClassRegisterId == classRegisterId).FirstOrDefaultAsync();
            if (present == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            var presentStudentIds = present.StudentAttendances.Select(x => x.ClassRegisterId).ToList();
            var presentStudent = present.SessionClass.Students.Where(x => presentStudentIds.Contains(x.StudentContactId)).ToList();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true; 
            res.Result = presentStudent;
            return res;
        }

        async Task<APIResponse<List<StudentContact>>> IAttendanceService.AbsentStudentAsync(Guid classRegisterId)
        {
            var res = new APIResponse<List<StudentContact>>(); 
            var classRegister = await context.ClassRegister
                .Include(q => q.SessionClass)
                .ThenInclude(s => s.Students)
                .Include(q => q.StudentAttendances)
                .ThenInclude(x=>x.StudentContact)
                .Where(x => x.ClassRegisterId == classRegisterId).FirstOrDefaultAsync();
            if (classRegister == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            var presentStudentIds = classRegister.StudentAttendances.Select(x=>x.ClassRegisterId).ToList();
            var absentStudents = classRegister.SessionClass.Students.Where(x => !presentStudentIds.Contains(x.StudentContactId)).ToList();
           

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            res.Result = absentStudents;
            return res;

        }

        async Task<APIResponse<DeleteClassRegisterContract>> IAttendanceService.DeleteClassRegisterAsync(DeleteClassRegisterContract ClassRegister)
        {
            var res = new APIResponse<DeleteClassRegisterContract>();
            //var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var result = await context.ClassRegister
 
                .FirstOrDefaultAsync(x => x.ClassRegisterId == ClassRegister.ClassRegisterId);

            if (result != null)
            {
                result.Deleted = true;
                await context.SaveChangesAsync();
            }

            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            res.IsSuccessful = true;
            res.Result = ClassRegister;
            return res;
        }
        async Task<APIResponse<UpdateClassRegisterContract>> IAttendanceService.UpdateClassRegisterLabel(UpdateClassRegisterContract ClassRegister)
        {
            var res = new APIResponse<UpdateClassRegisterContract>();
            //var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var result = await context.ClassRegister
                .Include(q => q.SessionClass)
                .ThenInclude(s => s.Students)
                .Include(q => q.StudentAttendances)
                .FirstOrDefaultAsync(x => x.ClassRegisterId == ClassRegister.ClassRegisterId && x.Deleted == false);

            if (result != null)
            {
                result.RegisterLabel = ClassRegister.RegisterLabel;
            }
            context.ClassRegister.Update(result);
            await context.SaveChangesAsync();
            res.Message.FriendlyMessage = Messages.Updated;
            res.IsSuccessful = true;
            res.Result = ClassRegister;
            return res;
        }
    }
}
