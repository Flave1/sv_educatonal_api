using BLL;
using BLL.Wrappers;
using Microsoft.Extensions.Options;
using SMP.BLL.Services.WebRequestServices;
using SMP.Contracts.Assessment;
using SMP.Contracts.Authentication;
using SMP.Contracts.Options;
using SMP.Contracts.Routes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.CBTAssessmentServices
{
    public class CBTAssessmentService : ICBTAssessmentService
    {
        private readonly IWebRequestService webRequestService;
        private readonly FwsConfigSettings fwsOptions;

        public CBTAssessmentService(IWebRequestService webRequestService, IOptions<FwsConfigSettings> options)
        {
            this.webRequestService = webRequestService;
            fwsOptions = options.Value;
        }

        async Task<APIResponse<PagedResponse<List<CBTExamination>>>> ICBTAssessmentService.GetCBTAssessmentsAsync(string sessionClassId, int pageNumber)
        {

            var res = new APIResponse<PagedResponse<List<CBTExamination>>>();
            var apiCredentials = new SmsClientInformationRequest
            {
                ApiKey = fwsOptions.Apikey,
                ClientId = fwsOptions.ClientId
            };
            var fwsClientInformation = await webRequestService.PostAsync<SmsClientInformation, SmsClientInformationRequest>($"{fwsRoutes.clientInformation}clientId={fwsOptions.ClientId}&apiKey={fwsOptions.Apikey}", apiCredentials);

            var clientDetails = new Dictionary<string, string>();
            clientDetails.Add("userId", fwsClientInformation.Result.UserId);
            clientDetails.Add("smsClientId", "");
            clientDetails.Add("productBaseurlSuffix", fwsClientInformation.Result.BaseUrlAppendix);


            res = await webRequestService.GetAsync<APIResponse<PagedResponse<List<CBTExamination>>>>($"{cbtRoutes.getClassCBTs}?PageNumber={pageNumber}&PageSize=20&sessionClassId={sessionClassId}", clientDetails);
            if (res.Result == null)
            {
                res.Message.FriendlyMessage = res.Message.FriendlyMessage;
                return res;
            }
            res.IsSuccessful = true;
            return res;

        }
    }
}
