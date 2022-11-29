using Contracts.Class;
using Contracts.Common;
using DAL;
using DAL.SubjectModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Utilities;
using SMP.Contracts.Common;
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

        public SubjectService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        async Task<APIResponse<Subject>> ISubjectService.CreateSubjectAsync(ApplicationLookupCommand subject)
        {
            var res = new APIResponse<Subject>();

            try
            {
                if (context.Subject.AsEnumerable().Any(r => Tools.ReplaceWhitespace(subject.Name) == Tools.ReplaceWhitespace(r.Name)))
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
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }


            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have successfuly created a class lookp";
            return res;
        }

        async Task<APIResponse<Subject>> ISubjectService.UpdateSubjectAsync(string Name, string Id, bool isActive)
        {
            var res = new APIResponse<Subject>();

            try
            {
                if (context.Subject.AsEnumerable().Any(r => Tools.ReplaceWhitespace(Name) == Tools.ReplaceWhitespace(r.Name) && r.SubjectId != Guid.Parse(Id)))
                {
                    res.Message.FriendlyMessage = "Subject Name Already exist";
                    return res;
                }

                var lookup = context.Subject.FirstOrDefault(r => r.SubjectId == Guid.Parse(Id));
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
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have successfuly updated a class lookp";
            return res;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> ISubjectService.GetAllSubjectsAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var result =  await context.Subject.OrderByDescending(d => d.CreatedOn).Where(d => d.Deleted != true).Select(a => new GetApplicationLookups { LookupId = a.SubjectId.ToString().ToLower(), Name = a.Name, IsActive = a.IsActive }).ToListAsync();
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> ISubjectService.GetAllActiveSubjectsAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var result = await context.Subject.OrderByDescending(d => d.CreatedOn).Where(d => d.Deleted != true && d.IsActive == true).Select(a => new GetApplicationLookups { LookupId = a.SubjectId.ToString().ToLower(), Name = a.Name, IsActive = a.IsActive }).ToListAsync();
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }


        async Task<APIResponse<Subject>> ISubjectService.DeleteSubjectAsync(MultipleDelete request)
        {
            var res = new APIResponse<Subject>();
            
            foreach(var Id in request.Items)
            {
                var lookup = context.Subject.FirstOrDefault(d => d.SubjectId == Guid.Parse(Id));
                if (lookup == null)
                {
                    res.Message.FriendlyMessage = "Subject  does not exist";
                    return res;
                }
                lookup.Deleted = true;
                lookup.Name = lookup.Name + "_DELETE" + DateTime.Now.ToString();
                await context.SaveChangesAsync();
                res.Result = lookup;
            }

           
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have successfuly deleted a class lookp";
            return res;
        }

        APIResponse<List<DropdownSelect>> ISubjectService.GetAllStudentSubjects()
        {
            var res = new APIResponse<List<DropdownSelect>>();

            var studentContactId = accessor.HttpContext.User.FindFirst(x => x.Type == "studentContactId")?.Value;

            var student = context.StudentContact.Include(x => x.User).Include(d => d.SessionClass)
                .ThenInclude(s => s.SessionClassSubjects)
                .ThenInclude(d => d.Subject).FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactId));

            if(student is null)
            {
                res.Result = new List<DropdownSelect>();
                res.IsSuccessful = true;
                return res;
            }

            res.Result = student.SessionClass.SessionClassSubjects
                .Where(e => e.Subject.Deleted == false && e.Subject.IsActive == true)
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
            var student = context.StudentContact.FirstOrDefault(s => s.StudentContactId == Guid.Parse(stdId));
            var res = new APIResponse<Guid>();
            var result = context.SessionClassSubject.FirstOrDefault(d => d.SubjectId == subjectId && d.SessionClassId == student.SessionClassId).SubjectTeacherId;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }
    }
}
