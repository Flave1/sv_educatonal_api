using BLL;
using BLL.AuthenticationServices;
using BLL.Filter;
using BLL.Utilities;
using BLL.Wrappers;
using DAL;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.ParentServices;
using SMP.BLL.Services.WebRequestServices;
using SMP.Contracts.Admissions;
using SMP.Contracts.Authentication;
using SMP.Contracts.Options;
using SMP.Contracts.PinManagement;
using SMP.Contracts.ResultModels;
using SMP.Contracts.Routes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly IWebRequestService webRequestService;
        private readonly FwsConfigSettings fwsOptions;

        public AdmissionService(DataContext context, IPaginationService paginationService, IUserService userService, IOptions<FwsConfigSettings> options, 
            IParentService parentService, IWebRequestService webRequestService)
        {
            this.context = context;
            this.paginationService = paginationService;
            this.userService = userService;
            this.parentService = parentService;
            this.webRequestService = webRequestService;
            fwsOptions = options.Value;
        }

        public async Task<APIResponse<bool>> EnrollCandidate(string admissionId)
        {
            throw new NotImplementedException();
        }
        public async Task<APIResponse<bool>> ExportCandidatesToCbt(ExportCandidateToCbt request)
        {
            var res = new APIResponse<bool>();
            try
            {  
                var admission = context.Admissions.Where(x => x.Deleted != true && x.ClassId == Guid.Parse(request.ClassId));
                if(!admission.Any())
                {
                    res.Message.FriendlyMessage = "Ops! No Candidate available for export";
                    return res;
                }
                var candidates = new CreateAdmissionCandidateCbt
                {
                    CandidateCategory = request.CategoryName,
                    AdmissionCandidateList = await admission.Select(x=> new AdmissionCandidateList
                    {
                        FirstName = x.Firstname,
                        LastName = x.Lastname,
                        OtherName = x.Middlename,
                        PhoneNumber = x.PhoneNumber,
                        Email = x.Email
                    }).ToListAsync(),
                };

                var apiCredentials = new SmsClientInformationRequest
                {
                    ApiKey = fwsOptions.Apikey,
                    ClientId = fwsOptions.ClientId
                };
                var fwsRequest = await webRequestService.PostAsync<SmsClientInformation, SmsClientInformationRequest>($"{fwsOptions.FwsBaseUrl}{fwsRoutes.clientInformation}clientId={fwsOptions.ClientId}&apiKey={fwsOptions.Apikey}", apiCredentials);

                var clientDetails = new Dictionary<string, string>();
                clientDetails.Add("userId", fwsRequest.Result.ClientId);
                clientDetails.Add("smsClientId", "");
                clientDetails.Add("productBaseurlSuffix", fwsRequest.Result.BaseUrlAppendix);

                var result = await webRequestService.PostAsync<APIResponse<string>, CreateAdmissionCandidateCbt>($"{fwsOptions.FwsBaseUrl}{cbtRoutes.createCbtCandidate}", candidates, clientDetails);
                if (result.Result == null)
                {
                    res.Message.FriendlyMessage = result.Message.FriendlyMessage;
                    return res;
                }

                foreach(var item in admission)
                {
                    item.CandidateCategory = result.Result;
                }
                await context.SaveChangesAsync();

                res.Result = true;
                res.Message.FriendlyMessage = Messages.Created;
                res.IsSuccessful = true;
                return res;
            }
            catch(Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
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
