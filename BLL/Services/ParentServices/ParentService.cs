using BLL;
using BLL.AuthenticationServices;
using BLL.Filter;
using BLL.Utilities;
using BLL.Wrappers;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.Contracts.ParentModels;
using SMP.DAL.Models.Parents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ParentServices
{
    public class ParentService : IParentService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;
        private readonly IUserService userService;

        public ParentService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService, IUserService userService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
            this.userService = userService;
        }

        async Task<Guid> IParentService.SaveParentDetail(string email, string name, string relationship, string number, Guid id)
        {
            string userid = string.Empty;
            var parent = context.Parents.FirstOrDefault(x => x.Email.ToLower() == email.Trim().ToLower()) ?? null;
            if (parent == null)
                parent = context.Parents.FirstOrDefault(x => x.Parentid == id) ?? null;

            if (parent == null)
            {
                userid = await userService.CreateParentUserAccountAsync(email, number);
                parent = new Parents();
                parent.Name = name;
                parent.Relationship = relationship;
                parent.Number = number;
                parent.Email = email.Trim();
                parent.UserId = userid;
                context.Parents.Add(parent);
            }
            else
            {
                parent.Name = name;
                parent.Relationship = relationship;
                parent.Number = number;
                parent.Email = email.Trim();
                await userService.UpdateParentUserAccountAsync(email, number, parent.UserId);
            }
            context.SaveChanges();
            return parent.Parentid;
        }

        async Task<APIResponse<PagedResponse<List<MyWards>>>> IParentService.GetMyWardsAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<MyWards>>>();
            var userName = accessor.HttpContext.User.FindFirst(e => e.Type == "userName")?.Value;

            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            if (!string.IsNullOrEmpty(userName))
            {
                var query = context.StudentContact
                    .Include(x => x.Parent)
                    .Where(x => x.Parent.Email == userName)
                        .Include(d => d.User)
                        .Include(x => x.SessionClass).ThenInclude(x => x.Class)
                        .OrderByDescending(d => d.User.FirstName)
                        .Where(d => d.Deleted == false);

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(x => new MyWards(x, regNoFormat)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;

        }


    }
}
