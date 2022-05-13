using Contracts.Class;
using Contracts.Common;
using DAL;
using DAL.SubjectModels;
using Microsoft.EntityFrameworkCore;
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

        public SubjectService(DataContext context)
        {
            this.context = context;
        }

        async Task<APIResponse<Subject>> ISubjectService.CreateSubjectAsync(string subjectName)
        {
            var res = new APIResponse<Subject>();

            if (context.Subject.Any(r => subjectName.Trim().ToLower() == r.Name.Trim().ToLower()))
            {
                res.Message.FriendlyMessage = "Subject Name Already exist";
                return res;
            }
            var lookup = new Subject
            {
                Name = subjectName,
                IsActive = true,
            };
            context.Subject.Add(lookup);
            await context.SaveChangesAsync();


            res.Result = lookup;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have successfuly created a class lookp";
            return res;
        }

        async Task<APIResponse<Subject>> ISubjectService.UpdateSubjectAsync(string Name, string Id, bool isActive)
        {
            var res = new APIResponse<Subject>();

            if (context.Subject.Any(r => Name.ToLower().Trim() == r.Name.Trim().ToLower() && r.SubjectId != Guid.Parse(Id)))
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
            var result = await context.SaveChangesAsync();

            res.Result = lookup;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have successfuly updated a class lookp";
            return res;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> ISubjectService.GetAllSubjectsAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var result =  await context.Subject.Where(d => d.Deleted != true).Select(a => new GetApplicationLookups { LookupId = a.SubjectId.ToString(), Name = a.Name, IsActive = a.IsActive }).ToListAsync();
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
    }
}
