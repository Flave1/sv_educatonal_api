﻿using BLL;
using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.Filter;
using BLL.LoggerService;
using BLL.StudentServices;
using BLL.Wrappers;
using Contracts.Options;
using DAL;
using DAL.Authentication;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.ParentServices;
using SMP.BLL.Services.SessionServices;
using SMP.BLL.Services.WebRequestServices;
using SMP.BLL.Utilities;
using SMP.Contracts.Admissions;
using SMP.Contracts.Authentication;
using SMP.Contracts.Options;
using SMP.Contracts.Routes;
using SMP.DAL.Models.StudentImformation;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AdmissionServices
{
    public class AdmissionService : IAdmissionService
    {
        private readonly DataContext context;
        private readonly IPaginationService paginationService;
        private readonly IWebRequestService webRequestService;
        private readonly UserManager<AppUser> manager;
        private readonly IWebHostEnvironment environment;
        private readonly IFileUploadService fileUploadService;
        private readonly IHttpContextAccessor accessor;
        private readonly IUtilitiesService utilitiesService;
        private readonly ILoggerService loggerService;
        private readonly FwsConfigSettings fwsOptions;
        private readonly IParentService parentService;
        private readonly string smsClientId;
        private readonly ITermService termService;
        private readonly IUserService userService;
        private readonly IStudentService studentService;

        public AdmissionService(DataContext context, IPaginationService paginationService, IUserService userService, IOptions<FwsConfigSettings> options,
            IParentService parentService, IWebRequestService webRequestService, UserManager<AppUser> manager, IWebHostEnvironment environment,
            IFileUploadService fileUploadService, IHttpContextAccessor httpContext, IUtilitiesService utilitiesService, ILoggerService 
            loggerService, ITermService termService, IStudentService studentService)
        {
            this.context = context;
            this.paginationService = paginationService;
            this.webRequestService = webRequestService;
            this.manager = manager;
            this.environment = environment;
            this.fileUploadService = fileUploadService;
            this.accessor = httpContext;
            this.utilitiesService = utilitiesService;
            this.loggerService = loggerService;
            fwsOptions = options.Value;
            this.parentService = parentService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.termService = termService;
            this.userService = userService;
            this.studentService = studentService;
        }

        public async Task<APIResponse<bool>> EnrollCandidate(EnrollCandidate request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var admission = await context.Admissions
                    .Where(x => x.ClientId == smsClientId && x.AdmissionId == Guid.Parse(request.AdmissionId) && x.Deleted != true).FirstOrDefaultAsync();

                if (admission == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "AdmissionId doesn't exist!";
                }
                if (admission.CandidateAdmissionStatus == (int)CandidateAdmissionStatus.Admitted)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Candidate already admitted";
                }
                var parent = await context.Parents.FirstOrDefaultAsync(x => x.Parentid == admission.ParentId);

                var student = new StudentContactCommand(admission, parent, request.SessionClassId);

                var result = await utilitiesService.GenerateStudentRegNo();
                if (!result.Any())
                {
                    res.Message.FriendlyMessage = "School registration number not setup";
                    return res;
                }

                student.Email = "";
                var userId = await userService.CreateStudentUserAccountAsync(student, result.Keys.First(), studentService.GetRegistrationFormat());

                string photoUrl = "";

                if (!string.IsNullOrEmpty(admission.Photo))
                {
                    string admissionFileName = student.Photo.Split("AdmissionPassport/")[1];
                    var admissionPath = Path.Combine(environment.ContentRootPath, "wwwroot/" + smsClientId + "/AdmissionPassport", admissionFileName);
                    string profilePhotoPath = Path.Combine(environment.ContentRootPath, "wwwroot/" + smsClientId + "/ProfileImage", admissionFileName);

                    fileUploadService.CopyFile(admissionPath, profilePhotoPath);
                    var host = accessor.HttpContext.Request.Host.ToUriComponent();
                    photoUrl = $"{accessor.HttpContext.Request.Scheme}://{host}/{smsClientId}/ProfileImage/{admissionFileName}";
                }

                var parentId = await parentService.SaveParentDetail(
                    student.ParentOrGuardianEmail,
                    student.ParentOrGuardianFirstName, 
                    student.ParentOrGuardianLastName, 
                    student.ParentOrGuardianRelationship, 
                    student.ParentOrGuardianPhone, Guid.Empty);

                var item = new StudentContact
                {
                    CityId = student.CityId,
                    CountryId = student.CountryId,
                    EmergencyPhone = student.EmergencyPhone,
                    HomeAddress = student.HomeAddress,
                    ParentId = parentId,
                    HomePhone = student.HomePhone,
                    StateId = student.StateId,
                    UserId = userId,
                    ZipCode = student.ZipCode,
                    RegistrationNumber = result.Keys.First(),
                    StudentContactId = Guid.NewGuid(),
                    Status = (int)StudentStatus.Active,
                    SessionClassId = Guid.Parse(student.SessionClassId),
                    EnrollmentStatus = (int)EnrollmentStatus.Enrolled,
                    LastName = student.LastName,
                    DOB = student.DOB,
                    FirstName = student.FirstName,
                    MiddleName = student.MiddleName,
                    Phone = student.Phone,
                    Photo = photoUrl
                };
                context.StudentContact.Add(item);

                admission.CandidateAdmissionStatus = (int)CandidateAdmissionStatus.Admitted;

                await context.SaveChangesAsync();
                await studentService.CreateStudentSessionClassHistoryAsync(item);

                res.Message.FriendlyMessage = "Successfully Enrolled";
                res.Result = true;
                res.IsSuccessful = true;
                return res;
            }
            catch (DuplicateNameException ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
            catch (ArgumentException ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Error Occurred trying to create student account!! Please contact system administrator";
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }

        public async Task<APIResponse<bool>> EnrollMultipleCandidates(EnrollCandidates request)
        {
            var res = new APIResponse<bool>();

            try
            {
                var admissions = await context.Admissions.Where(x => x.ClientId == smsClientId && request.AdmissionIds.Contains(x.AdmissionId.ToString()) && x.Deleted != true).ToListAsync();

                if (!admissions .Any())
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "AdmissionIds doesn't exist!";
                }

                var studentContactList = new List<StudentContactCommand>();

                foreach (var admission in admissions)
                {
                    if (admission.CandidateAdmissionStatus != (int)CandidateAdmissionStatus.Admitted)
                    {
                        var parent = await context.Parents.FirstOrDefaultAsync(x => x.Parentid == admission.ParentId);

                        var student = new StudentContactCommand(admission, parent, request.SessionClassId);
                        
                        studentContactList.Add(student);

                        admission.CandidateAdmissionStatus = (int)CandidateAdmissionStatus.Admitted;
                    }
                }

                foreach (var student in studentContactList)
                {
                    var result = await utilitiesService.GenerateStudentRegNo();
                    if (!result.Any())
                    {
                        res.Message.FriendlyMessage = "School registration number not setup";
                        return res;
                    }
                    student.Email = "";
                    var userId = await userService.CreateStudentUserAccountAsync(student, result.Keys.First(), studentService.GetRegistrationFormat());
                    
                    var parentId = await parentService
                        .SaveParentDetail(
                        student.ParentOrGuardianEmail, 
                        student.ParentOrGuardianFirstName, 
                        student.ParentOrGuardianLastName, 
                        student.ParentOrGuardianRelationship, 
                        student.ParentOrGuardianPhone, Guid.Empty);
                    
                    var item = new StudentContact
                    {
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        MiddleName = student.MiddleName,
                        CityId = student.CityId,
                        CountryId = student.CountryId,
                        EmergencyPhone = student.EmergencyPhone,
                        HomeAddress = student.HomeAddress,
                        ParentId = parentId,
                        HomePhone = student.HomePhone,
                        StateId = student.StateId,
                        UserId = userId,
                        ZipCode = student.ZipCode,
                        RegistrationNumber = result.Keys.First(),
                        StudentContactId = Guid.NewGuid(),
                        Status = (int)StudentStatus.Active,
                        SessionClassId = Guid.Parse(student.SessionClassId),
                        EnrollmentStatus = (int)EnrollmentStatus.Enrolled
                    };
                    context.StudentContact.Add(item);

                    await context.SaveChangesAsync();

                    await studentService.CreateStudentSessionClassHistoryAsync(item);
                }

                res.Message.FriendlyMessage = "Successfully Enrolled";
                res.Result = true;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Error Occurred trying to enroll student account!! Please contact system administrator";
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }

        public async Task<APIResponse<bool>> ExportCandidatesToCbt(ExportCandidateToCbt request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var admission = context.Admissions.Where(x => x.ClientId == smsClientId && x.Deleted != true && x.ClassId == Guid.Parse(request.ClassId));
                if (!admission.Any())
                {
                    res.Message.FriendlyMessage = "Ooops! No Candidate available for export";
                    return res;
                }
                var candidates = new CreateAdmissionCandidateCbt
                {
                    CandidateCategory = request.CandidateCategory,
                    CategoryName = request.CategoryName,
                    AdmissionCandidateList = await admission.Select(x => new AdmissionCandidateList
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
                    ClientId = smsClientId,
                };
                var fwsRequest = await webRequestService.PostAsync<SmsClientInformation, SmsClientInformationRequest>($"{fwsRoutes.clientInformation}clientId={smsClientId}&apiKey={fwsOptions.Apikey}", apiCredentials);

                var clientDetails = new Dictionary<string, string>();
                clientDetails.Add("userId", fwsRequest.Result.UserId);
                clientDetails.Add("smsClientId", fwsRequest.Result.ClientId);

                var result = await webRequestService.PostAsync<APIResponse<SMPCbtCreateCandidateResponse>, CreateAdmissionCandidateCbt>($"{cbtRoutes.createCbtCandidate}", candidates, clientDetails);
                if (result.Result == null)
                {
                    res.Message.FriendlyMessage = result.Message.FriendlyMessage;
                    return res;
                }

                foreach (var item in admission)
                {
                    item.CandidateCategory = result.Result.CategoryId;
                    item.CandidateCategoryName = result.Result.CategoryName;
                }
                await context.SaveChangesAsync();

                res.Result = true;
                res.Message.FriendlyMessage = Messages.Created;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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
                var result = await context.Admissions
                    .Where(c => c.ClientId == smsClientId && c.Deleted != true && c.AdmissionId == Guid.Parse(admissionId))
                    .Select(d => new SelectAdmission(d, 
                    context.ClassLookUp.Where(x => x.ClientId == smsClientId && x.ClassLookupId == d.ClassId).FirstOrDefault(), 
                    context.Parents.Where(x => x.ClientId == smsClientId && x.Parentid == d.ParentId).FirstOrDefault())).FirstOrDefaultAsync();

                if (result == null)
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                else
                    res.Message.FriendlyMessage = Messages.GetSuccess;

                res.IsSuccessful = true;
                res.Result = result;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<PagedResponse<List<SelectAdmission>>>> GetAllAdmission(PaginationFilter filter, string classId, string examStatus, string admissionSettingsId)
        {
            var res = new APIResponse<PagedResponse<List<SelectAdmission>>>();
            try
            {
                if(string.IsNullOrEmpty(admissionSettingsId))
                {
                    var admissionSettings = await context.AdmissionSettings.FirstOrDefaultAsync(x => x.AdmissionStatus == true && x.ClientId == smsClientId);

                    if (string.IsNullOrWhiteSpace(classId) && string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                        .Where(c => c.Deleted != true && c.AdmissionSettingId == admissionSettings.AdmissionSettingId)
                        .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }

                    if (!string.IsNullOrWhiteSpace(classId) && !string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == admissionSettings.AdmissionSettingId && c.ClassId == Guid.Parse(classId) && c.ExaminationStatus == int.Parse(examStatus))
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                    if (!string.IsNullOrWhiteSpace(classId) && string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == admissionSettings.AdmissionSettingId && c.ClassId == Guid.Parse(classId))
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                    if (string.IsNullOrWhiteSpace(classId) && !string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == admissionSettings.AdmissionSettingId && c.ExaminationStatus == int.Parse(examStatus))
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(classId) && string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                        .Where(c => c.Deleted != true && c.AdmissionSettingId == Guid.Parse(admissionSettingsId))
                        .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }

                    if (!string.IsNullOrWhiteSpace(classId) && !string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == Guid.Parse(admissionSettingsId) && c.ClassId == Guid.Parse(classId) && c.ExaminationStatus == int.Parse(examStatus))
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                    if (!string.IsNullOrWhiteSpace(classId) && string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == Guid.Parse(admissionSettingsId) && c.ClassId == Guid.Parse(classId))
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                    if (string.IsNullOrWhiteSpace(classId) && !string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == Guid.Parse(admissionSettingsId) && c.ExaminationStatus == int.Parse(examStatus))
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                }

                

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<bool>> ImportCbtResult(string classId)
        {
            var res = new APIResponse<bool>();
            try
            {
                var admissions = context.Admissions.Where(x => x.ClientId == smsClientId && x.Deleted != true && x.ClassId == Guid.Parse(classId));
                if (!admissions.Any())
                {
                    res.Message.FriendlyMessage = "Ops! No Candidate available on the selected class";
                    return res;
                }

                if(admissions.Any(x=> string.IsNullOrEmpty(x.CandidateCategory)))
                {
                    res.Message.FriendlyMessage = "Ops! Kindly export all Candidates to CBT.";
                    return res;
                }

                var admission = await admissions.FirstOrDefaultAsync();
                var apiCredentials = new SmsClientInformationRequest
                {
                    ApiKey = fwsOptions.Apikey,
                    ClientId = smsClientId,
                };
                var fwsRequest = await webRequestService.PostAsync<SmsClientInformation, SmsClientInformationRequest>($"{fwsRoutes.clientInformation}clientId={smsClientId}&apiKey={fwsOptions.Apikey}", apiCredentials);

                var clientDetails = new Dictionary<string, string>();
                clientDetails.Add("userId", fwsRequest.Result.UserId);
                clientDetails.Add("smsClientId", "");
                clientDetails.Add("productBaseurlSuffix", fwsRequest.Result.BaseUrlAppendix);

                var result = await webRequestService.GetAsync<APIResponse<List<GetCbtResult>>>($"{cbtRoutes.getCbtResult}?candidateCategoryId={admission.CandidateCategory}", clientDetails);
                if (result.Result == null)
                {
                    res.Message.FriendlyMessage = result.Message.FriendlyMessage;
                    return res;
                }

                foreach (var item in result.Result)
                {
                    var candidate = await admissions.FirstOrDefaultAsync(x => x.ClientId == smsClientId && x.Email.ToLower() == item.CandidateEmail.ToLower() && x.Deleted != true);
                    if (candidate != null)
                    {
                        if (item.Status.ToLower() == "passed")
                            candidate.ExaminationStatus = (int)AdmissionExaminationStatus.Passed;
                        if (item.Status.ToLower() == "failed")
                            candidate.ExaminationStatus = (int)AdmissionExaminationStatus.Failed;
                        if (item.Status.ToLower() == "not taken")
                            candidate.ExaminationStatus = (int)AdmissionExaminationStatus.NotTaken;

                        candidate.ExaminationId = item.ExaminationId;
                    }
                }
                await context.SaveChangesAsync();

                res.Result = true;
                res.Message.FriendlyMessage = Messages.Created;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }

}
