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
using System.Net;
using System.Threading.Tasks;

namespace SMP.BLL.Services.CBTAssessmentServices
{
    public class CBTAssessmentService : ICBTAssessmentService
    {
        private readonly IWebRequestService webRequestService;
        private readonly FwsConfigSettings fwsOptions;
        private readonly DataContext context;
        private readonly IUtilitiesService utilitiesService;
        //private readonly FwsClientInformation fwsClientInformations;
        private readonly string smsClientId;
        private readonly IScoreEntryHistoryService scoreEntryService;
        private readonly ILoggerService loggerService;

        public CBTAssessmentService(IWebRequestService webRequestService, IOptions<FwsConfigSettings> options, DataContext context,
            IUtilitiesService utilitiesService,
            IHttpContextAccessor accessor, IScoreEntryHistoryService scoreEntryService, ILoggerService loggerService)
        {
            this.webRequestService = webRequestService;
            fwsOptions = options.Value;
            this.context = context;
            this.utilitiesService = utilitiesService;
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
                ClientId = smsClientId
            };
            var fwsClientInformation = await webRequestService.PostAsync<SmsClientInformation, SmsClientInformationRequest>($"{fwsRoutes.clientInformation}clientId={fwsOptions.ClientId}&apiKey={fwsOptions.Apikey}", apiCredentials);

            var clientDetails = new Dictionary<string, string>();
            clientDetails.Add("userId", fwsClientInformation.Result.UserId);
            clientDetails.Add("smsClientId", smsClientId);

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


                    var fwsClientInformation = await GetClientInformationAsync();

                    var clientDetails = new Dictionary<string, string>();
                    clientDetails.Add("examinationId", examId);
                    clientDetails.Add("candidateId_regNo", stdRegNumber);
                    clientDetails.Add("userId", fwsClientInformation.Result.UserId);
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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

                if (string.IsNullOrEmpty(studentRegNos))
                {
                    res.Message.FriendlyMessage = "No student has taken this examination";
                    return res;
                }
                var students = studentRegNos.Split(',').ToArray();
                foreach (var stdRegNumber in students)
                {
                    var studentRegNo = utilitiesService.GetStudentRegNumberValue(stdRegNumber);
                    var student = await utilitiesService.GetStudentContactByRegNo(studentRegNo);

                    var fwsClientInformation = await GetClientInformationAsync();

                    var clientDetails = new Dictionary<string, string>();
                    clientDetails.Add("examinationId", examId);
                    clientDetails.Add("candidateId_regNo", stdRegNumber);
                    clientDetails.Add("userId", fwsClientInformation.Result.UserId);
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }

            res.Message.FriendlyMessage = Include ? "Assessment Scores Included Successfully" : "Assessment Scores Successfully Removed from score entry";
            res.IsSuccessful = true;
            return res;
        }

      
        private async Task<SmsClientInformation> GetClientInformationAsync()
        {
            var apiCredentials = new SmsClientInformationRequest
            {
                ApiKey = fwsOptions.Apikey,
                ClientId = smsClientId
            };
            return await webRequestService.PostAsync<SmsClientInformation, SmsClientInformationRequest>($"{fwsRoutes.clientInformation}clientId={fwsOptions.ClientId}&apiKey={fwsOptions.Apikey}", apiCredentials);

        }

    }
}
