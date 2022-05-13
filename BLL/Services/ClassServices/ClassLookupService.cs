using Contracts.Class;
using Contracts.Common;
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

        async Task<APIResponse<ClassLookup>> IClassLookupService.CreateClassLookupAsync(string className)
        {
            var res = new APIResponse<ClassLookup>();
            if (context.ClassLookUp.Any(r => className.Trim().ToLower() == r.Name.Trim().ToLower().ToLower()))
            {
                res.Message.FriendlyMessage = "Class Name Already exist";
                return res;
            }
            var lookup = new ClassLookup
            {
                Name = className,
                IsActive = true,
            };
            context.ClassLookUp.Add(lookup);
            await context.SaveChangesAsync();
            res.Result = lookup;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have succesfully created a class lookup";
            return res;
        }

        async Task<APIResponse<ClassLookup>> IClassLookupService.UpdateClassLookupAsync(string lookupName, string lookupId, bool isActive)
        {
            var res = new APIResponse<ClassLookup>();

            if (context.ClassLookUp.Any(r => lookupName.ToLower().Trim() == r.Name.Trim().ToLower() && r.ClassLookupId != Guid.Parse(lookupId)))
            {
                res.Message.FriendlyMessage = "Class Name Already exist";
                return res;
            }

            var lookup = context.ClassLookUp.FirstOrDefault(r => r.ClassLookupId == Guid.Parse(lookupId));
            if (lookup == null)
            {
                res.Message.FriendlyMessage = "Class Lookup does not exist";
                return res;
            }
            lookup.Name = lookupName;
            lookup.IsActive = isActive;
            await context.SaveChangesAsync();

            res.IsSuccessful = true;
            res.Result = lookup;
            res.Message.FriendlyMessage = "You have succesfully updated a class lookup";
            return res;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> IClassLookupService.GetAllClassLookupsAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var result = await context.ClassLookUp.Where(d => d.Deleted != true).Select(a => new GetApplicationLookups { LookupId = a.ClassLookupId.ToString(), Name = a.Name, IsActive = a.IsActive }).ToListAsync();
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }
         

        async Task<APIResponse<ClassLookup>> IClassLookupService.DeleteClassLookupAsync(MultipleDelete request)
        {
            var res = new APIResponse<ClassLookup>();
            foreach(var lookupId in request.Items)
            {
                var lookup = context.ClassLookUp.FirstOrDefault(d => d.ClassLookupId == Guid.Parse(lookupId));
                if (lookup == null)
                {
                    res.Message.FriendlyMessage = "Class Lookup does not exist";
                    return res;
                }
                lookup.Deleted = true;
                await context.SaveChangesAsync();

                res.Result = lookup;
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have succesfully deleted a class lookup";
            return res;
        }
    }
}
