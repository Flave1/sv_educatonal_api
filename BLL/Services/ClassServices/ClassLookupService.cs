using Contracts.Class;
using Contracts.Common;
using DAL;
using DAL.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.ClassServices
{
    public class ClassLookupService : IClassLookupService
    {
        private readonly DataContext context;
        private readonly string smsClientId;
        public ClassLookupService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        async Task<APIResponse<ClassLookup>> IClassLookupService.CreateClassLookupAsync(string className, bool isActive, Guid gradeLevelId)
        {
            var res = new APIResponse<ClassLookup>();
            try
            {
                if (context.ClassLookUp.Where(c=> c.ClientId == smsClientId).AsEnumerable().Any(r => Tools.ReplaceWhitespace(className) == Tools.ReplaceWhitespace(r.Name) && r.Deleted == false))
                {
                    res.Message.FriendlyMessage = "Class Name Already exist";
                    return res;
                }
                var lookup = new ClassLookup
                {
                    Name = className,
                    IsActive = isActive,
                    GradeGroupId = gradeLevelId
                };
                context.ClassLookUp.Add(lookup);
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
            res.Message.FriendlyMessage = "You have succesfully created a class lookup";
            return res;
        }

        async Task<APIResponse<ClassLookup>> IClassLookupService.UpdateClassLookupAsync(string lookupName, string lookupId, bool isActive, Guid gradeLevelId)
        {
            var res = new APIResponse<ClassLookup>();

            try
            {
                if (context.ClassLookUp.Where(c => c.ClientId == smsClientId).AsEnumerable().Any(r => Tools.ReplaceWhitespace(lookupName) == Tools.ReplaceWhitespace(r.Name) 
                && r.ClassLookupId != Guid.Parse(lookupId)))
                {
                    res.Message.FriendlyMessage = "Class Name Already exist";
                    return res;
                }

                var lookup = context.ClassLookUp.FirstOrDefault(r => r.ClassLookupId == Guid.Parse(lookupId) && r.ClientId == smsClientId);
                if (lookup == null)
                {
                    res.Message.FriendlyMessage = "Class Lookup does not exist";
                    return res;
                }
                lookup.Name = lookupName;
                lookup.IsActive = isActive;
                lookup.GradeGroupId = gradeLevelId;
                await context.SaveChangesAsync();
                res.Result = lookup;
            }
            catch (Exception ex)
            {

                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have succesfully updated a class lookup";
            return res;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> IClassLookupService.GetAllClassLookupsAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var result = await context.ClassLookUp.Where(c => c.ClientId == smsClientId)
                .OrderBy(s => s.Name)
                .Include(d => d.GradeLevel)
                .Where(d => d.Deleted != true).Select(a => new GetApplicationLookups { 
                    LookupId = a.ClassLookupId.ToString().ToLower(), 
                    Name = a.Name, 
                    IsActive = a.IsActive, 
                    GradeLevelId = a.GradeGroupId.ToString(),
                    GradeLevelName = a.GradeLevel.GradeGroupName
                }).ToListAsync();
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<List<GetApplicationLookups>>> IClassLookupService.GetAllActiveClassLookupsAsync()
        {
            var res = new APIResponse<List<GetApplicationLookups>>();
            var result = await context.ClassLookUp.Where(d => d.Deleted != true && d.IsActive == true && d.ClientId == smsClientId)
                .OrderBy(d => d.Name)
                .Select(a => new GetApplicationLookups { 
                    LookupId = a.ClassLookupId.ToString().ToLower(), 
                    Name = a.Name, 
                    IsActive = a.IsActive,
                    GradeLevelId = a.GradeGroupId.ToString(),
                }).ToListAsync();
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<ClassLookup>> IClassLookupService.DeleteClassLookupAsync(MultipleDelete request)
        {
            var res = new APIResponse<ClassLookup>();
            foreach(var lookupId in request.Items)
            {
                var lookup = context.ClassLookUp.FirstOrDefault(d => d.ClassLookupId == Guid.Parse(lookupId) && d.ClientId == smsClientId);
                if (lookup == null)
                {
                    res.Message.FriendlyMessage = "Class Lookup does not exist";
                    return res;
                }

                if(context.SessionClass.Any(x => x.ClassId == lookup.ClassLookupId && x.Deleted == false && x.ClientId == smsClientId))
                {
                    res.Message.FriendlyMessage = "Class setup cannot be deleted";
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
