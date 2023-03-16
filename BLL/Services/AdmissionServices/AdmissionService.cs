using BLL;
using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.Filter;
using BLL.LoggerService;
using BLL.Wrappers;
using Contracts.Options;
using DAL;
using DAL.Authentication;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.ParentServices;
using SMP.BLL.Services.WebRequestServices;
using SMP.BLL.Utilities;
using SMP.Contracts.Admissions;
using SMP.Contracts.Authentication;
using SMP.Contracts.Options;
using SMP.Contracts.Routes;
using SMP.DAL.Models.Admission;
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

        public AdmissionService(DataContext context, IPaginationService paginationService, IUserService userService, IOptions<FwsConfigSettings> options,
            IParentService parentService, IWebRequestService webRequestService, UserManager<AppUser> manager, IWebHostEnvironment environment,
            IFileUploadService fileUploadService, IHttpContextAccessor httpContext, IUtilitiesService utilitiesService, ILoggerService loggerService)
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
        }

        public async Task<APIResponse<bool>> EnrollCandidate(EnrollCandidate request)
        {
            var res = new APIResponse<bool>();
            using (var transaction = await context.Database.BeginTransactionAsync())
            {

                try
                {
                    var admission = await context.Admissions.Where(x => x.ClientId == smsClientId && x.AdmissionId == Guid.Parse(request.AdmissionId) && x.Deleted != true)
                                    .Include(x => x.AdmissionNotification).FirstOrDefaultAsync();

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

                    var student = new StudentContactCommand
                    {
                        FirstName = admission.Firstname,
                        LastName = admission.Lastname,
                        MiddleName = admission.Middlename,
                        Phone = admission.PhoneNumber,
                        DOB = admission.DateOfBirth.ToString(),
                        Email = admission.Email,
                        HomePhone = admission.PhoneNumber,
                        EmergencyPhone = admission.ParentPhoneNumber,
                        ParentOrGuardianFirstName = admission.ParentName,
                        ParentOrGuardianEmail = admission.AdmissionNotification.ParentEmail,
                        HomeAddress = $"{admission.LGAOfOrigin}, {admission.StateOfOrigin}, {admission.CountryOfOrigin}",
                        CityId = admission.StateOfOrigin,
                        StateId = admission.StateOfOrigin,
                        CountryId = admission.CountryOfOrigin,
                        SessionClassId = request.SessionClassId,
                        Photo = admission.Photo,
                    };

                    var result = await utilitiesService.GenerateStudentRegNo();

                    var userId = await CreateStudentUserAccountAsync(student, result.Keys.First(), result.Values.First(), student.Photo);
                    string admissionFileName = student.Photo.Split("AdmissionPassport/")[1];

                    var admissionPath = Path.Combine(environment.ContentRootPath, "wwwroot/" + smsClientId + "/AdmissionPassport", admissionFileName);

                    string profilePhotoPath = Path.Combine(environment.ContentRootPath, "wwwroot/"+ smsClientId + "/ProfileImage", admissionFileName);

                    fileUploadService.CopyFile(admissionPath, profilePhotoPath);

                    var host = accessor.HttpContext.Request.Host.ToUriComponent();
                    var photoUrl = $"{accessor.HttpContext.Request.Scheme}://{host}/{smsClientId}/ProfileImage/{admissionFileName}";
                    var parentId = await parentService.SaveParentDetail(student.ParentOrGuardianEmail, student.ParentOrGuardianFirstName, student.ParentOrGuardianLastName, student.ParentOrGuardianRelationship, student.ParentOrGuardianPhone, Guid.Empty);
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
                    await CreateStudentSessionClassHistoryAsync(item);

                    await transaction.CommitAsync();
                    res.Message.FriendlyMessage = Messages.Created;
                    res.Result = true;
                    res.IsSuccessful = true;
                    return res;
                }
                catch (DuplicateNameException ex)
                {
                    await transaction.RollbackAsync();
                    await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                    res.Message.FriendlyMessage = ex.Message;
                    res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                    return res;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                    res.Message.FriendlyMessage = "Error Occurred trying to create student account!! Please contact system administrator";
                    res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                    return res;
                }


                finally { await transaction.DisposeAsync(); }
            }
        }
        public async Task<APIResponse<bool>> EnrollMultipleCandidates(EnrollCandidates request)
        {
            var res = new APIResponse<bool>();

            try
            {
                var admissions = context.Admissions.Where(x => x.ClientId == smsClientId && request.AdmissionIds.Contains(x.AdmissionId.ToString()) && x.Deleted != true)
                                .Include(x => x.AdmissionNotification);

                if (admissions == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "AdmissionIds doesn't exist!";
                }

                var studentContactList = new List<StudentContactCommand>();

                foreach (var admission in admissions)
                {
                    if (admission.CandidateAdmissionStatus != (int)CandidateAdmissionStatus.Admitted)
                    {
                        var student = new StudentContactCommand
                        {
                            FirstName = admission.Firstname,
                            LastName = admission.Lastname,
                            MiddleName = admission.Middlename,
                            Phone = admission.PhoneNumber,
                            DOB = admission.DateOfBirth.ToString(),
                            Email = admission.Email,
                            HomePhone = admission.PhoneNumber,
                            EmergencyPhone = admission.ParentPhoneNumber,
                            ParentOrGuardianFirstName = admission.ParentName,
                            ParentOrGuardianEmail = admission.AdmissionNotification.ParentEmail,
                            HomeAddress = $"{admission.LGAOfOrigin}, {admission.StateOfOrigin}, {admission.CountryOfOrigin}",
                            CityId = admission.StateOfOrigin,
                            StateId = admission.StateOfOrigin,
                            CountryId = admission.CountryOfOrigin,
                            SessionClassId = request.SessionClassId,
                            Photo = admission.Photo,
                        };
                        studentContactList.Add(student);

                        admission.CandidateAdmissionStatus = (int)CandidateAdmissionStatus.Admitted;
                    }
                }

                foreach (var student in studentContactList)
                {
                    var result = await utilitiesService.GenerateStudentRegNo();

                    var userId = await CreateStudentUserAccountAsync(student, result.Keys.First(), result.Values.First(), student.Photo);
                    var parentId = await parentService.SaveParentDetail(student.ParentOrGuardianEmail, student.ParentOrGuardianFirstName, student.ParentOrGuardianLastName, student.ParentOrGuardianRelationship, student.ParentOrGuardianPhone, Guid.Empty);
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
                        EnrollmentStatus = (int)EnrollmentStatus.Enrolled
                    };
                    context.StudentContact.Add(item);

                    await context.SaveChangesAsync();

                    await CreateStudentSessionClassHistoryAsync(item);
                }

                res.Message.FriendlyMessage = Messages.Created;
                res.Result = true;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Error Occurred trying to enroll student account!! Please contact system administrator";
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }
        async Task CreateStudentSessionClassHistoryAsync(StudentContact student)
        {
            var history = new StudentSessionClassHistory();
            history.SessionClassId = student.SessionClassId;
            history.StudentContactId = student.StudentContactId;
            history.SessionTermId = context.SessionTerm.FirstOrDefault(s => s.IsActive && s.ClientId == smsClientId)?.SessionTermId;
            await context.StudentSessionClassHistory.AddAsync(history);
            await context.SaveChangesAsync();
        }
        async Task<string> CreateStudentUserAccountAsync(StudentContactCommand student, string regNo, string regNoFormat, string photoPath)
        {
            try
            {
                var email = !string.IsNullOrEmpty(student.Email) ? student.Email : regNo.Replace("/", "") + "@school.com";

              
                var user = new AppUser
                {
                    UserName = email,
                    Active = true,
                    Deleted = false,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "",
                    Email = email,
                    UserType = (int)UserTypes.Student
                };
                var result = await manager.CreateAsync(user, regNoFormat);
                if (!result.Succeeded)
                {
                    if (result.Errors.Select(d => d.Code).Any(a => a == "DuplicateUserName"))
                    {
                        throw new DuplicateNameException(result.Errors.FirstOrDefault().Description);
                    }
                    else
                        throw new ArgumentException(result.Errors.FirstOrDefault().Description);
                }
                var addTorole = await manager.AddToRoleAsync(user, DefaultRoles.STUDENT);
                if (!addTorole.Succeeded)
                    throw new ArgumentException(addTorole.Errors.FirstOrDefault().Description);

                return user.Id;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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
                    .Include(c => c.AdmissionNotification)
                    .Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClientId == smsClientId && x.ClassLookupId == d.ClassId).FirstOrDefault())).FirstOrDefaultAsync();

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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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
                        .Include(c => c.AdmissionNotification)
                        .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }

                    if (!string.IsNullOrWhiteSpace(classId) && !string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == admissionSettings.AdmissionSettingId && c.ClassId == Guid.Parse(classId) && c.ExaminationStatus == int.Parse(examStatus))
                       .Include(c => c.AdmissionNotification)
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                    if (!string.IsNullOrWhiteSpace(classId) && string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == admissionSettings.AdmissionSettingId && c.ClassId == Guid.Parse(classId))
                       .Include(c => c.AdmissionNotification)
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                    if (string.IsNullOrWhiteSpace(classId) && !string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == admissionSettings.AdmissionSettingId && c.ExaminationStatus == int.Parse(examStatus))
                       .Include(c => c.AdmissionNotification)
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(classId) && string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                        .Where(c => c.Deleted != true && c.AdmissionSettingId == Guid.Parse(admissionSettingsId))
                        .Include(c => c.AdmissionNotification)
                        .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }

                    if (!string.IsNullOrWhiteSpace(classId) && !string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == Guid.Parse(admissionSettingsId) && c.ClassId == Guid.Parse(classId) && c.ExaminationStatus == int.Parse(examStatus))
                       .Include(c => c.AdmissionNotification)
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                    if (!string.IsNullOrWhiteSpace(classId) && string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == Guid.Parse(admissionSettingsId) && c.ClassId == Guid.Parse(classId))
                       .Include(c => c.AdmissionNotification)
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                    if (string.IsNullOrWhiteSpace(classId) && !string.IsNullOrWhiteSpace(examStatus))
                    {
                        var query = context.Admissions
                       .Where(c => c.Deleted != true && c.AdmissionSettingId == Guid.Parse(admissionSettingsId) && c.ExaminationStatus == int.Parse(examStatus))
                       .Include(c => c.AdmissionNotification)
                       .OrderByDescending(c => c.CreatedOn);
                        var totalRecord = query.Count();
                        var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                    }
                }

                

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }

}
