using Contracts.Class;
using DAL;
using DAL.ClassEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.ClassServices
{
    public class ClassLookupService : IClassLookupService
    {
        private readonly DataContext context;

        public ClassLookupService(DataContext context)
        {
            this.context = context;
        }

        async Task IClassLookupService.CreateClassLookupAsync(string className)
        {
            if (context.ClassLookUp.Any(r => className.Trim().ToLower().Contains(r.Name.Trim().ToLower())))
                throw new ArgumentException("Class Name Already exist");
            var lookup = new ClassLookup
            {
                Name = className,
                IsActive = true,
            };
            context.ClassLookUp.Add(lookup);
            await context.SaveChangesAsync();
        }

        async Task IClassLookupService.UpdateClassLookupAsync(string lookupName, string lookupId, bool isActive)
        {
            if (context.ClassLookUp.Any(r => lookupName.Contains(r.Name) && r.ClassLookupId != Guid.Parse(lookupId)))
                throw new ArgumentException("Class Name Already exist");

            var lookup = context.ClassLookUp.FirstOrDefault(r => r.ClassLookupId == Guid.Parse(lookupId));
            if (lookup == null)
                throw new ArgumentException("Class Lookup does not exist");
            lookup.Name = lookupName;
            lookup.IsActive = isActive;
            var result = await context.SaveChangesAsync();
        }

        async Task<List<GetApplicationLookups>> IClassLookupService.GetAllClassLookupsAsync()
        {
            return await context.ClassLookUp.Where(d => d.Deleted != true).Select(a => new GetApplicationLookups { LookupId = a.ClassLookupId.ToString(), Name = a.Name, IsActive = a.IsActive }).ToListAsync();
        }
         

        async Task IClassLookupService.DeleteClassLookupAsync(string lookupId)
        {
            var lookup =  context.ClassLookUp.FirstOrDefault(d => d.ClassLookupId == Guid.Parse(lookupId));
            if (lookup == null)
            {
                throw new ArgumentException("Class Lookup does not exist");
            }
            lookup.Deleted = true;
            await context.SaveChangesAsync();
        }
    }
}
