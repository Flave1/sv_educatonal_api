using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.AttendanceContract;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.SessionServices;
using SMP.BLL.Utilities;
using SMP.DAL.Models.Attendance;
using SMP.DAL.Models.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AttendanceServices
{
    public class AttendanceService: IAttendanceService
    {
        private readonly DataContext context;
        private readonly IPaginationService paginationService;
        private readonly string smsClientId;
        private readonly ITermService termService;

        public AttendanceService(DataContext context, IPaginationService paginationService, IHttpContextAccessor accessor, ITermService termService)
        {
            this.context = context;
            this.paginationService = paginationService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.termService = termService;
        }

        async Task CreateRegister(PostStudentAttendance2 req)
        {
            var term = termService.GetCurrentTerm();
            var reg = new ClassRegister
            {
                ClassRegisterId = req.ClassRegisterId,
                SessionClassId = req.SessionClassId,
                RegisterLabel = req.RegisterLabel,
                SessionTermId = term.SessionTermId
            };
            await context.ClassRegister.AddAsync(reg);
            await context.SaveChangesAsync();
        }


        async Task<APIResponse<GetAttendance>> IAttendanceService.CreateClassRegisterAsync(Guid SessionClassId)
        {
            var res = new APIResponse<GetAttendance>();

            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;
            var termid = termService.GetCurrentTerm().SessionTermId;

            var reg = new ClassRegister
            {
                SessionClassId = SessionClassId,
                RegisterLabel = $"ATTENDANCE AS AT {Tools.GetCurrentLocalDateTime().ToString("dd-MM-yyy hh:mm")}",
                SessionTermId = termid
            };

            await context.ClassRegister.AddAsync(reg);
            await context.SaveChangesAsync();

            var classAttendance = await context.ClassRegister.Where(c => c.ClientId == smsClientId).Include(s => s.SessionClass).ThenInclude(d => d.Students).Where(d =>
            d.ClassRegisterId == reg.ClassRegisterId)
                .Select(s => new GetAttendance(s, regNoFormat)).FirstOrDefaultAsync();

            res.Result = classAttendance;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.Created;
            return res;
        }
        async Task<APIResponse<PostStudentAttendance>> IAttendanceService.UpdateStudentAttendanceRecordFromMobile(PostStudentAttendance2 req)
        {
            var res = new APIResponse<PostStudentAttendance>();

            var register = context.ClassRegister.Find(req.ClassRegisterId);

            if(register == null)
            {
                await CreateRegister(req);
                res.Result = (this as IAttendanceService).UpdateStudentAttendanceRecord(new PostStudentAttendance
                {
                    ClassRegisterId = req.ClassRegisterId,
                    IsPresent = req.IsPresent,
                    StudentContactId = req.StudentContactId
                }
                ).Result.Result;
            }
            else
            {
                res.Result = (this as IAttendanceService).UpdateStudentAttendanceRecord(new PostStudentAttendance
                {
                    ClassRegisterId = req.ClassRegisterId,
                    IsPresent = req.IsPresent,
                    StudentContactId = req.StudentContactId
                }).Result.Result;
            }
           
            res.IsSuccessful = true;
            return res;
        }
        async Task<APIResponse<PostStudentAttendance>> IAttendanceService.UpdateStudentAttendanceRecord(PostStudentAttendance attendance)
        {
            var res = new APIResponse<PostStudentAttendance>();

            var attendanceRecord = await context.StudentAttendance.FirstOrDefaultAsync(x => x.ClassRegisterId == attendance.ClassRegisterId 
            && x.StudentContactId == attendance.StudentContactId && x.ClientId == smsClientId);

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
        async Task<APIResponse<GetAttendance>> IAttendanceService.ContinueAttendanceAsync(Guid ClassRegisterId)
        {
            var res = new APIResponse<GetAttendance>();
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            var register = await context.ClassRegister.Where(c => c.ClientId == smsClientId && c.ClassRegisterId == ClassRegisterId)
                .Include(d => d.StudentAttendances)
                .Include(s => s.SessionClass).ThenInclude(d => d.Students).ThenInclude(s => s.User)
                .Select(s => new GetAttendance(s, regNoFormat)).FirstOrDefaultAsync();

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
          
        async Task<APIResponse<PagedResponse<List<GetAttendance>>>> IAttendanceService.GetAllAttendanceRegisterAsync(string sessionClassId, string termId, PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<GetAttendance>>>();

            var query = context.ClassRegister.Where(c => c.ClientId == smsClientId)
                .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(q => q.StudentAttendances)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false);
            if (!string.IsNullOrEmpty(sessionClassId))
            {
                query = query.Where(d => d.SessionClassId == Guid.Parse(sessionClassId));
            }

            if (!string.IsNullOrEmpty(termId))
            {
                query = query.Where(d => d.SessionTermId == Guid.Parse(termId));
            }

            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(f => new GetAttendance(f)).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PagedResponse<List<GetAttendance>>>> IAttendanceService.GetAllAttendanceRegisterAsync(string sessionClassId, PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<GetAttendance>>>();
            var termId = termService.GetCurrentTerm().SessionTermId;

            var query = context.ClassRegister.Where(c => c.ClientId == smsClientId)
                .Include(s => s.SessionClass).ThenInclude(s => s.Students)
                .Include(q => q.StudentAttendances)
                .OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Deleted == false);
            if (!string.IsNullOrEmpty(sessionClassId))
            {
                query = query.Where(d => d.SessionClassId == Guid.Parse(sessionClassId));
            }

            query = query.Where(d => d.SessionTermId == termId);

            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(f => new GetAttendance(f)).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<AttendanceList>>> IAttendanceService.GetAllStudentPresentAsync(Guid classRegisterId)
        {
            var res = new APIResponse<List<AttendanceList>> ();
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            var classRegister = await context.ClassRegister
                .Where(c => c.ClientId == smsClientId && c.ClassRegisterId == classRegisterId)
                .Include(s => s.StudentAttendances).ThenInclude(q => q.StudentContact).FirstOrDefaultAsync();
            
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
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            var classRegister = await context.ClassRegister.Where(c => c.ClientId == smsClientId && c.ClassRegisterId == classRegisterId)
                .Include(q => q.SessionClass).ThenInclude(s => s.Students).ThenInclude(e => e.User)
                .Include(q => q.StudentAttendances).ThenInclude(x=>x.StudentContact).ThenInclude(e => e.User).FirstOrDefaultAsync();
           
            if (classRegister == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            var presentStudentIds = classRegister.StudentAttendances.Select(x=>x.StudentContactId).ToList();
            var absentStudents = classRegister.SessionClass.Students.Where(d => d.EnrollmentStatus == 1).Where(x => !presentStudentIds.Contains(x.StudentContactId)).Select(s => new AttendanceList(s, regNoFormat, false)).ToList();
           

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            res.Result = absentStudents;
            return res;

        }

        async Task<APIResponse<bool>> IAttendanceService.DeleteClassRegisterAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();

            var result = await context.ClassRegister.FirstOrDefaultAsync(x => x.ClassRegisterId == Guid.Parse(request.Item) && x.ClientId == smsClientId);

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
                .FirstOrDefaultAsync(x => x.ClassRegisterId == request.ClassRegisterId && x.Deleted == false && x.ClientId == smsClientId);

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
