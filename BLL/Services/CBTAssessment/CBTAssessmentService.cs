using BLL;
using BLL.LoggerService;
using BLL.Wrappers;
using DAL;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SMP.BLL.Services.ResultServices;
using SMP.BLL.Services.WebRequestServices;
using SMP.BLL.Utilities;
using SMP.Contracts.Assessment;
using SMP.Contracts.Authentication;
using SMP.Contracts.Options;
using SMP.Contracts.Routes;
using SMP.DAL.Models;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.CBTAssessmentServices
{
    public class CBTAssessmentService : ICBTAssessmentService
    {
        private readonly IWebRequestService webRequestService;
        private readonly FwsConfigSettings fwsOptions;
        private readonly DataContext context;
        private readonly IUtilitiesService utilitiesService;
        private readonly FwsClientInformation fwsClientInformations;
        private readonly string smsClientId;
        private readonly IScoreEntryHistoryService scoreEntryService;
        private readonly ILoggerService loggerService;

        public CBTAssessmentService(IWebRequestService webRequestService, IOptions<FwsConfigSettings> options, DataContext context,
            IUtilitiesService utilitiesService, FwsClientInformation fwsClientInformations,
            IHttpContextAccessor accessor, IScoreEntryHistoryService scoreEntryService, ILoggerService loggerService)
        {
            this.webRequestService = webRequestService;
            fwsOptions = options.Value;
            this.context = context;
            this.utilitiesService = utilitiesService;
            this.fwsClientInformations = fwsClientInformations;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.scoreEntryService = scoreEntryService;
            this.loggerService = loggerService;
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
            CopyClientInformation(fwsClientInformation);

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

        async Task<APIResponse<bool>> ICBTAssessmentService.IncludeCBTAssessmentToScoreEntryAsAssessment(string sessionClassId, string subjectId, string studentRegNos, bool Include, string examId)
        {
            var res = new APIResponse<bool>();
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive == true && x.ClientId == smsClientId).SessionTermId;

            try
            {
                var students = studentRegNos.Split(',').ToArray();
                foreach (var stdRegNumber in students)
                {
                    var studentRegNo = utilitiesService.GetStudentRegNumberValue(stdRegNumber);
                    var student =  await utilitiesService.GetStudentContactByRegNo(studentRegNo);
                    
                   
                    var clientDetails = new Dictionary<string, string>();
                    clientDetails.Add("examinationId", examId);
                    clientDetails.Add("candidateId_regNo", stdRegNumber);
                    clientDetails.Add("userId", fwsClientInformations.UserId);
                    var studentResult = await webRequestService.GetAsync<APIResponse<SelectResult>>($"{cbtRoutes.studentResult}", clientDetails);
                    if(studentResult.Result == null)
                        continue;

                    var scoreHistory = scoreEntryService.GetScoreEntryHistory(student.SessionClassId.ToString(), subjectId, termId.ToString(), student.StudentContactId.ToString());

                    float score = 0;
                    if (scoreHistory is null)
                        score = await scoreEntryService.CreateNewScoreEntryHistoryAndReturnScore(scoreHistory, studentResult.Result.TotalScore, student.StudentContactId.ToString(), sessionClassId, subjectId.ToString(), termId, Include);
                    else
                    {
                        score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, Include, studentResult.Result.TotalScore);
                    }
                    var scoreEntry = scoreEntryService.GetScoreEntry(termId, student.StudentContactId, Guid.Parse(subjectId));

                    if (scoreHistory is null)
                    {
                        score = await scoreEntryService.CreateNewScoreEntryHistoryAndReturnScore(scoreHistory, studentResult.Result.TotalScore, student.StudentContactId.ToString(), sessionClassId, subjectId, termId, Include);
                    }
                    else
                    {
                        score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, Include, studentResult.Result.TotalScore);
                    }
                    if (scoreEntry is null)
                    {
                        scoreEntryService.CreateNewScoreEntryForAssessment(scoreEntry, termId, (float)studentResult.Result.TotalScore, student.StudentContactId, Guid.Parse(subjectId), Guid.Parse(sessionClassId));
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        scoreEntryService.UpdateScoreEntryForAssessment(scoreEntry, score);
                        await context.SaveChangesAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                await loggerService.Error($"Error occurred || {ex}");
                throw;
            }

            res.Message.FriendlyMessage = Include ? "Assessment Scores Included Successfully" : "Assessment Scores Successfully Removed from score entry";
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<bool>> ICBTAssessmentService.IncludeCBTAssessmentToScoreEntryAsExamination(string sessionClassId, string subjectId, string studentRegNos, bool Include, string examId)
        {
            var res = new APIResponse<bool>();
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive == true && x.ClientId == smsClientId).SessionTermId;

            try
            {
                var students = studentRegNos.Split(',').ToArray();
                foreach (var stdRegNumber in students)
                {
                    var studentRegNo = utilitiesService.GetStudentRegNumberValue(stdRegNumber);
                    var student = await utilitiesService.GetStudentContactByRegNo(studentRegNo);

                    var clientDetails = new Dictionary<string, string>();
                    clientDetails.Add("examinationId", examId);
                    clientDetails.Add("candidateId_regNo", stdRegNumber);
                    clientDetails.Add("userId", fwsClientInformations.UserId);
                    var studentResult = await webRequestService.GetAsync<APIResponse<SelectResult>>($"{cbtRoutes.studentResult}", clientDetails);
                    if (studentResult.Result == null)
                    {
                        continue;
                    }
                    var scoreHistory = scoreEntryService.GetScoreEntryHistory(student.SessionClassId.ToString(), subjectId, termId.ToString(), student.StudentContactId.ToString());

                    float score = 0;
                    if (scoreHistory is null)
                        score = await scoreEntryService.CreateNewScoreEntryHistoryAndReturnScore(scoreHistory, studentResult.Result.TotalScore, student.StudentContactId.ToString(), sessionClassId, subjectId.ToString(), termId, Include);
                    else
                    {
                        score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, Include, studentResult.Result.TotalScore);
                    }
                    var scoreEntry = scoreEntryService.GetScoreEntry(termId, student.StudentContactId, Guid.Parse(subjectId));

                    if (scoreHistory is null)
                    {
                        score = await scoreEntryService.CreateNewScoreEntryHistoryAndReturnScore(scoreHistory, studentResult.Result.TotalScore, student.StudentContactId.ToString(), sessionClassId, subjectId, termId, Include);
                    }
                    else
                    {
                        score = scoreEntryService.IncludeAndExcludeThenReturnScore(scoreHistory, Include, studentResult.Result.TotalScore);
                    }
                    if (scoreEntry is null)
                    {
                        scoreEntryService.CreateNewScoreEntryForExam(scoreEntry, termId, (float)studentResult.Result.TotalScore, student.StudentContactId, Guid.Parse(subjectId), Guid.Parse(sessionClassId));
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        scoreEntryService.UpdateScoreEntryForExam(scoreEntry, score);
                        await context.SaveChangesAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                await loggerService.Error($"Error occurred || {ex}");
                throw;
            }

            res.Message.FriendlyMessage = Include ? "Assessment Scores Included Successfully" : "Assessment Scores Successfully Removed from score entry";
            res.IsSuccessful = true;
            return res;
        }

      

        void CopyClientInformation(SmsClientInformation fwsClientInformation)
        {
            fwsClientInformations.ClientId = fwsClientInformation.Result.ClientId;
            fwsClientInformations.Address = fwsClientInformation.Result.Address;
            fwsClientInformations.BaseUrlAppendix = fwsClientInformation.Result.BaseUrlAppendix;
            fwsClientInformations.Country = fwsClientInformation.Result.Country;
            fwsClientInformations.IpAddress = fwsClientInformation.Result.IpAddress;
            fwsClientInformations.PasswordHash = fwsClientInformation.Result.PasswordHash;
            fwsClientInformations.SchoolName = fwsClientInformation.Result.SchoolName;
            fwsClientInformations.SmsapI_KEY = fwsClientInformation.Result.SmsapI_KEY;
            fwsClientInformations.State = fwsClientInformation.Result.State;
            fwsClientInformations.UserId = fwsClientInformation.Result.UserId;
            fwsClientInformations.UserName = fwsClientInformation.Result.UserName;
        }
    }
}
