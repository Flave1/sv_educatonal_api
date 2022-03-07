using Contracts.Class;
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

        async Task ISubjectService.CreateSubjectAsync(string subjectName)
        {
            if (context.Subject.Any(r => subjectName.Trim().ToLower().Contains(r.Name.Trim().ToLower())))
                throw new ArgumentException("Subject Name Already exist");
            var lookup = new Subject
            {
                Name = subjectName,
                IsActive = true,
            };
            context.Subject.Add(lookup);
            await context.SaveChangesAsync();
        }

        async Task ISubjectService.UpdateSubjectAsync(string Name, string Id, bool isActive)
        {
            if (context.Subject.Any(r => Name.Contains(r.Name) && r.SubjectId != Guid.Parse(Id)))
                throw new ArgumentException("Subject Name Already exist");

            var lookup = context.Subject.FirstOrDefault(r => r.SubjectId == Guid.Parse(Id));
            if (lookup == null)
                throw new ArgumentException("Subject  does not exist");
            lookup.Name = Name;
            lookup.IsActive = isActive;
            var result = await context.SaveChangesAsync();
        }

        async Task<List<GetApplicationLookups>> ISubjectService.GetAllSubjectsAsync()
        {
            return await context.Subject.Where(d => d.Deleted != true).Select(a => new GetApplicationLookups { LookupId = a.SubjectId.ToString(), Name = a.Name, IsActive = a.IsActive }).ToListAsync();
        }


        async Task ISubjectService.DeleteSubjectAsync(string Id)
        {
            var lookup = context.Subject.FirstOrDefault(d => d.SubjectId == Guid.Parse(Id));
            if (lookup == null)
            {
                throw new ArgumentException("Subject  does not exist");
            }
            lookup.Deleted = true;
            await context.SaveChangesAsync();
        }
    }
}
