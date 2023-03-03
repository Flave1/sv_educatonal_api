using BLL.LoggerService;
using Contracts.Class;
using Contracts.Common;
using DAL;
using DAL.SubjectModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Utilities;
using SMP.Contracts.Common;
using SMP.DAL.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.SubjectServices
{
    public class SubjectService : ISubjectService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;

        public SubjectService(DataContext context, IHttpContextAccessor accessor, ILoggerService loggerService)
        {
            this.context = context;
            this.accessor = accessor;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        async Task<APIResponse<Subject>> ISubjectService.CreateSubjectAsync(ApplicationLookupCommand subject)
        {
            var res = new APIResponse<Subject>();

            try
            {
                var subjectName = await context.Subject.FirstOrDefaultAsync(r => r.Name.ToLower() == subject.Name.ToLower() && r.ClientId == smsClientId && r.Deleted != true);
                if(subjectName != null)
                {
                    res.Message.FriendlyMessage = "Subject Name Already exist";
                    return res;
                }
                var lookup = new Subject
                {
                    Name = subject.Name,
                    IsActive = subject.IsActive,
                };
                context.Subject.Add(lookup);
                await context.SaveChangesAsync();
                res.Result = lookup;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }


            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have successfuly created a subject";
            return res;
        }

        async Task<APIResponse<Subject>> ISubjectService.UpdateSubjectAsync(string Name, string Id, bool isActive)
        {
            var res = new APIResponse<Subject>();

            try
            {
                if (context.Subject.AsEnumerable().Any(r => Tools.ReplaceWhitespace(Name) == Tools.ReplaceWhitespace(r.Name) && r.SubjectId != Guid.Parse(Id) && r.ClientId == smsClientId))
                {
                    res.Message.FriendlyMessage = "Subject Name Already exist";
                    return res;
                }

                var lookup = context.Subject.FirstOrDefault(r => r.SubjectId == Guid.Parse(Id) && r.ClientId == smsClientId);
                if (lookup == null)
                {
                    res.Message.FriendlyMessage = "Subject  does not exist";
                    return res;
                }
                lookup.Name = Name;
                lookup.IsActive = isActive;
                await context.SaveChangesAsync();
                res.Result = lookup;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have successfuly updated a subject";
            return res;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> ISubjectService.GetAllSubjectsAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var result =  await context.Subject.Where(d => d.Deleted != true && d.ClientId == smsClientId).OrderByDescending(d => d.CreatedOn).Select(a => new GetApplicationLookups { LookupId = a.SubjectId.ToString().ToLower(), Name = a.Name, IsActive = a.IsActive }).ToListAsync();
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> ISubjectService.GetAllActiveSubjectsAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var result = await context.Subject.Where(d => d.Deleted != true && d.IsActive == true && d.ClientId == smsClientId).OrderByDescending(d => d.CreatedOn).Select(a => new GetApplicationLookups { LookupId = a.SubjectId.ToString().ToLower(), Name = a.Name, IsActive = a.IsActive }).ToListAsync();
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }


        async Task<APIResponse<Subject>> ISubjectService.DeleteSubjectAsync(MultipleDelete request)
        {
            var res = new APIResponse<Subject>();
            
            foreach(var Id in request.Items)
            {
                var lookup = context.Subject.FirstOrDefault(d => d.SubjectId == Guid.Parse(Id) && d.ClientId == smsClientId);
                if (lookup == null)
                {
                    res.Message.FriendlyMessage = "Subject  does not exist";
                    return res;
                }
                if(context.SessionClassSubject.Any(x => x.SubjectId == lookup.SubjectId && x.ClientId == smsClientId))
                {
                    res.Message.FriendlyMessage = "Subject  cannot be deleted";
                    return res;
                }
                lookup.Deleted = true;
                lookup.Name = lookup.Name + "_DELETE" + DateTime.Now.ToString();
                await context.SaveChangesAsync();
                res.Result = lookup;
            }

           
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have successfuly deleted a subject";
            return res;
        }

        APIResponse<List<DropdownSelect>> ISubjectService.GetAllStudentSubjects()
        {
            var res = new APIResponse<List<DropdownSelect>>();

            var studentContactId = accessor.HttpContext.User.FindFirst(x => x.Type == "studentContactId")?.Value;

            var student = context.StudentContact.Where(x=>x.ClientId == smsClientId).Include(x => x.User).Include(d => d.SessionClass)
                .ThenInclude(s => s.SessionClassSubjects)
                .ThenInclude(d => d.Subject).FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactId));

            if(student is null)
            {
                res.Result = new List<DropdownSelect>();
                res.IsSuccessful = true;
                return res;
            }

            res.Result = student.SessionClass.SessionClassSubjects
                .Where(e => e.Subject.Deleted == false && e.Subject.IsActive == true && e.ClientId == smsClientId)
                .Select(a => new DropdownSelect { 
                    Value = a.SubjectId.ToString().ToLower(), 
                    Name = a.Subject.Name,
                    SupplimentId = a.SubjectTeacherId.ToString()
                }).ToList();

            res.IsSuccessful = true;
            return res;
        }


        APIResponse<Guid> ISubjectService.GetSubjectTeacher(Guid subjectId)
        {
            var stdId = accessor.HttpContext.User.FindFirst(d => d.Type == "studentContactId")?.Value;
            var student = context.StudentContact.FirstOrDefault(s => s.ClientId == smsClientId && s.StudentContactId == Guid.Parse(stdId));
            var res = new APIResponse<Guid>();
            var result = context.SessionClassSubject.FirstOrDefault(d => d.ClientId == smsClientId && d.SubjectId == subjectId && d.SessionClassId == student.SessionClassId).SubjectTeacherId;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }
    }
}
