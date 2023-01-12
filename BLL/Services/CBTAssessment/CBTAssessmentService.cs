using BLL;
using BLL.Wrappers;
using DAL;
using DAL.StudentInformation;
using Microsoft.Extensions.Options;
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

        public CBTAssessmentService(IWebRequestService webRequestService, IOptions<FwsConfigSettings> options, DataContext context, IUtilitiesService utilitiesService, FwsClientInformation fwsClientInformations)
        {
            this.webRequestService = webRequestService;
            fwsOptions = options.Value;
            this.context = context;
            this.utilitiesService = utilitiesService;
            this.fwsClientInformations = fwsClientInformations;
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
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive == true).SessionTermId;

            try
            {
                var students = studentRegNos.Split(',').ToArray();
                foreach (var stdRegNumber in students)
                {
                    var studentRegNo = utilitiesService.GetStudentRegNumberValue(stdRegNumber);
                    var student =  context.StudentContact.Where(x => x.RegistrationNumber == studentRegNo).FirstOrDefault();
                    var scoreEntry = context.ScoreEntry.Where(s => s.SessionTermId == termId && s.StudentContactId == student.StudentContactId && s.ClassScoreEntry.SubjectId == Guid.Parse(subjectId)).FirstOrDefault();
                    var scoreHistory = context.ScoreEntryHistory.FirstOrDefault(x => x.SessionClassId == sessionClassId && x.Subjectid == subjectId && x.SessionTermId == termId.ToString() && x.StudentId == student.StudentContactId.ToString());
                   
                    var clientDetails = new Dictionary<string, string>();
                    clientDetails.Add("examinationId", examId);
                    clientDetails.Add("candidateId_regNo", stdRegNumber);
                    clientDetails.Add("userId", fwsClientInformations.UserId);
                    var studentResult = await webRequestService.GetAsync<APIResponse<SelectResult>>($"{cbtRoutes.studentResult}", clientDetails);
                    if(studentResult.Result == null)
                    {
                        continue;
                    }
                    var score = 0;
                    if(scoreHistory is null)
                    {
                        await CreateNewScoreEntryHistory(scoreHistory, studentResult.Result, student, sessionClassId, subjectId, termId, Include);
                        score = +studentResult.Result.TotalScore;
                    }
                    else
                    {
                        if (Include)
                        {
                            scoreHistory.Score = scoreHistory.Score + '|' + studentResult.Result.TotalScore;
                            score = +studentResult.Result.TotalScore;
                        }
                        else
                        {
                            var list = scoreHistory.Score.Split('|').ToList();
                            var filtered = list.Remove(studentResult.Result.TotalScore.ToString());
                            scoreHistory.Score = string.Join('|', list);
                            score = -studentResult.Result.TotalScore;
                        }
                        await context.SaveChangesAsync();
                    }
                    if (scoreEntry is null)
                    {
                        scoreEntry = new ScoreEntry();
                        scoreEntry.SessionTermId = termId;
                        scoreEntry.ClassScoreEntryId = context.ClassScoreEntry.FirstOrDefault(x => x.SubjectId == Guid.Parse(subjectId) && x.SessionClassId == Guid.Parse(sessionClassId)).ClassScoreEntryId;
                        scoreEntry.AssessmentScore = score;
                        scoreEntry.IsOffered = true;
                        scoreEntry.IsSaved = true;
                        scoreEntry.StudentContactId = student.StudentContactId;
                        context.ScoreEntry.Add(scoreEntry);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        scoreEntry.AssessmentScore = score;
                        scoreEntry.IsOffered = true;
                        scoreEntry.IsSaved = true;
                        await context.SaveChangesAsync();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            res.Message.FriendlyMessage = "Assessment Scores Included Successfully";
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<bool>> ICBTAssessmentService.IncludeCBTAssessmentToScoreEntryAsExamination(string sessionClassId, string subjectId, string studentRegNos, bool Include, string examId)
        {
            var res = new APIResponse<bool>();
            var termId = context.SessionTerm.FirstOrDefault(x => x.IsActive == true).SessionTermId;

            try
            {
                var students = studentRegNos.Split(',').ToArray();
                foreach (var stdRegNumber in students)
                {
                    var studentRegNo = utilitiesService.GetStudentRegNumberValue(stdRegNumber);
                    var student = context.StudentContact.Where(x => x.RegistrationNumber == studentRegNo).FirstOrDefault();
                    var scoreEntry = context.ScoreEntry.Where(s => s.SessionTermId == termId && s.StudentContactId == student.StudentContactId && s.ClassScoreEntry.SubjectId == Guid.Parse(subjectId)).FirstOrDefault();
                    var scoreHistory = context.ScoreEntryHistory.FirstOrDefault(x => x.SessionClassId == sessionClassId && x.Subjectid == subjectId && x.SessionTermId == termId.ToString() && x.StudentId == student.StudentContactId.ToString());

                    var clientDetails = new Dictionary<string, string>();
                    clientDetails.Add("examinationId", examId);
                    clientDetails.Add("candidateId_regNo", stdRegNumber);
                    clientDetails.Add("userId", fwsClientInformations.UserId);
                    var studentResult = await webRequestService.GetAsync<APIResponse<SelectResult>>($"{cbtRoutes.studentResult}", clientDetails);
                    if (studentResult.Result == null)
                    {
                        continue;
                    }
                    var score = 0;
                    if (scoreHistory is null)
                    {
                        await CreateNewScoreEntryHistory(scoreHistory, studentResult.Result, student, sessionClassId, subjectId, termId, Include);
                        score = +studentResult.Result.TotalScore;
                    }
                    else
                    {
                        if (Include)
                        {
                            scoreHistory.Score = scoreHistory.Score + '|' + studentResult.Result.TotalScore;
                            score = +studentResult.Result.TotalScore;
                        }
                        else
                        {
                            var list = scoreHistory.Score.Split('|').ToList();
                            var filtered = list.Remove(studentResult.Result.TotalScore.ToString());
                            scoreHistory.Score = string.Join('|', list);
                            score = -studentResult.Result.TotalScore;
                        }
                        await context.SaveChangesAsync();
                    }
                    if (scoreEntry is null)
                    {
                        scoreEntry = new ScoreEntry();
                        scoreEntry.SessionTermId = termId;
                        scoreEntry.ClassScoreEntryId = context.ClassScoreEntry.FirstOrDefault(x => x.SubjectId == Guid.Parse(subjectId) && x.SessionClassId == Guid.Parse(sessionClassId)).ClassScoreEntryId;
                        scoreEntry.ExamScore = score;
                        scoreEntry.IsOffered = true;
                        scoreEntry.IsSaved = true;
                        scoreEntry.StudentContactId = student.StudentContactId;
                        context.ScoreEntry.Add(scoreEntry);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        scoreEntry.ExamScore = score;
                        scoreEntry.IsOffered = true;
                        scoreEntry.IsSaved = true;
                        await context.SaveChangesAsync();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            res.Message.FriendlyMessage = "Assessment Scores Included Successfully";
            res.IsSuccessful = true;
            return res;
        }

        private async Task CreateNewScoreEntryHistory(ScoreEntryHistory scoreHistory, SelectResult studentResult, StudentContact student, string sessionClassId, string subjectId, Guid termId, bool Include)
        {
            scoreHistory = new ScoreEntryHistory();
            scoreHistory.StudentId = student.StudentContactId.ToString();
            scoreHistory.SessionClassId = sessionClassId;
            scoreHistory.Subjectid = subjectId;
            scoreHistory.SessionTermId = termId.ToString();
            if (Include)
                scoreHistory.Score = scoreHistory.Score + '|' + studentResult.TotalScore;

            context.ScoreEntryHistory.Add(scoreHistory);
            await context.SaveChangesAsync();
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
