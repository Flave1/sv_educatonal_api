using BLL;
using BLL.AuthenticationServices;
using BLL.Filter;
using BLL.Utilities;
using BLL.Wrappers;
using DAL;
using DAL.StudentInformation;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.ParentServices;
using SMP.Contracts.Admissions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AdmissionServices
{
    public class AdmissionService : IAdmissionService
    {
        private readonly DataContext context;
        private readonly IPaginationService paginationService;
        private readonly IUserService userService;
        private readonly IParentService parentService;

        public AdmissionService(DataContext context, IPaginationService paginationService, IUserService userService, IParentService parentService)
        {
            this.context = context;
            this.paginationService = paginationService;
            this.userService = userService;
            this.parentService = parentService;
        }

        public async Task<APIResponse<bool>> EnrollCandidate(string admissionId)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse<SelectAdmission>> GetAdmission(string admissionId)
        {
            var res = new APIResponse<SelectAdmission>();
            try
            {
                int examStatus = (int)AdmissionExamStatus.Pending; //Change this to get examStatus from CBT result
                var result = await context.Admissions
                    .Where(c => c.Deleted != true && c.AdmissionId == Guid.Parse(admissionId))
                    .Include(c => c.AdmissionNotification)
                    .Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), examStatus)).FirstOrDefaultAsync();

                if (result == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                }
                else
                {
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                }

                res.IsSuccessful = true;
                res.Result = result;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<PagedResponse<List<SelectAdmission>>>> GetAllAdmission(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<SelectAdmission>>>();
            try
            {
                var query = context.Admissions
                    .Where(c => c.Deleted != true)
                    .Include(c => c.AdmissionNotification)
                    .OrderByDescending(c => c.CreatedOn);

                int examStatus = (int)AdmissionExamStatus.Pending; //Change this to get examStatus from CBT result
                var totalRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), examStatus)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }

}
