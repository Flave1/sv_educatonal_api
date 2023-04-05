using BLL;
using BLL.Filter;
using BLL.LoggerService;
using BLL.Wrappers;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NLog.Filters;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.Contracts.Admissions;
using SMP.Contracts.AdmissionSettings;
using SMP.DAL.Migrations;
using SMP.DAL.Models.Admission;
using SMP.DAL.Models.Parents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SMP.BLL.Services.AdmissionServices
{
    public class AdmissionSettingService : IAdmissionSettingService
    {
        private readonly DataContext context;
        private readonly IPaginationService paginationService;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        private readonly IHttpContextAccessor accessor;

        public AdmissionSettingService(DataContext context, IPaginationService paginationService, ILoggerService loggerService, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.paginationService = paginationService;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.accessor = accessor;
        }
        public async Task<APIResponse<CreateAdmissionSettings>> CreateSettings(CreateAdmissionSettings request)
        {
            var res = new APIResponse<CreateAdmissionSettings>();

            try
            {
                var setting = await context.AdmissionSettings
                    .Where(d => d.Deleted != true && d.AdmissionStatus == true && smsClientId == d.ClientId).FirstOrDefaultAsync();

                if(setting != null && request.AdmissionStatus == true)
                {
                    res.Message.FriendlyMessage = $"New admission cannot be created, {setting.AdmissionSettingName} is currently in progress.";
                    res.IsSuccessful = false;
                    return res;
                }


                var newSetting = new AdmissionSetting
                {
                    AdmissionSettingName = request.AdmissionSettingName,
                    Classes = string.Join(",", request.Classes),
                    AdmissionStatus = request.AdmissionStatus,
                    PassedExamEmail = request.PassedExamEmail,
                    FailedExamEmail = request.FailedExamEmail,
                    ScreeningEmail = request.ScreeningEmail,
                    RegistrationFee = request.RegistrationFee
                };
                context.AdmissionSettings.Add(newSetting);

                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Saved;
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

        public async Task<APIResponse<UpdateAdmissionSettings>> UpdateSettings(UpdateAdmissionSettings request)
        {
            var res = new APIResponse<UpdateAdmissionSettings>();
            try
            {
                var admissionSettings = await context.AdmissionSettings
                    .Where(d => d.Deleted != true && d.AdmissionSettingId == Guid.Parse(request.AdmissionSettingsId) 
                    && smsClientId == d.ClientId).FirstOrDefaultAsync();
                if (admissionSettings == null)
                {
                    res.Message.FriendlyMessage = "Admission Settings Id does not exist";
                    res.IsSuccessful = false;
                    return res;
                }

                var setting = await context.AdmissionSettings
                    .Where(d => d.Deleted != true && d.AdmissionStatus == true && smsClientId == d.ClientId).FirstOrDefaultAsync();

                if (setting != null && request.AdmissionStatus == true && admissionSettings.AdmissionStatus != true)
                {
                    res.Message.FriendlyMessage = $"New admission cannot be created {setting.AdmissionSettingName} is currently in progress";
                    res.IsSuccessful = false;
                    return res;
                }

                admissionSettings.AdmissionSettingName = request.AdmissionSettingName;
                admissionSettings.Classes = string.Join(",", request.Classes);
                admissionSettings.AdmissionStatus = request.AdmissionStatus;
                admissionSettings.PassedExamEmail = request.PassedExamEmail;
                admissionSettings.FailedExamEmail = request.FailedExamEmail;
                admissionSettings.ScreeningEmail = request.ScreeningEmail;
                admissionSettings.RegistrationFee = request.RegistrationFee;

                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Updated;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<bool>> DeleteSettings(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var settings = await context.AdmissionSettings.Where(d => d.Deleted != true && d.AdmissionSettingId == Guid.Parse(request.Item) && d.ClientId == smsClientId).FirstOrDefaultAsync();
                if (settings == null)
                {
                    res.Message.FriendlyMessage = "Admission Settings Id does not exist";
                    res.IsSuccessful = false;
                    return res;
                }

                settings.Deleted = true;
                await context.SaveChangesAsync();

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
                res.Result = true;
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

        public async Task<APIResponse<PagedResponse<List<SelectAdmissionSettings>>>> GetAllSettings(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<SelectAdmissionSettings>>>();
            try
            {
                var classes =  context.AdmissionSettings?
                    .Where(d => d.Deleted != true && d.ClientId == smsClientId)?
                    .FirstOrDefault()?.Classes?.Split(',', StringSplitOptions.None).ToList();

                var parentId = accessor.HttpContext.User.FindFirst(x => x.Type == "parentId")?.Value;
                var parentAdmissions = new List<Guid>();
                if (!string.IsNullOrEmpty(parentId))
                {
                    parentAdmissions = context.Admissions
                    .Where(x => x.ClientId == smsClientId && x.Deleted == false && x.ParentId == Guid.Parse(parentId))
                    .Select(c => c.AdmissionSettingId).Distinct().ToList();
                }

                if (classes is null)
                {
                    res.Result = new PagedResponse<List<SelectAdmissionSettings>>();
                    res.IsSuccessful = true;
                    return res;
                }
                IQueryable<AdmissionSetting> query = null;

                if (parentAdmissions.Any())
                {
                    query = context.AdmissionSettings
                    .Where(d => d.Deleted != true && smsClientId == d.ClientId && parentAdmissions.Contains(d.AdmissionSettingId) && d.AdmissionStatus == true).OrderBy(x => x.CreatedOn);
                }

                if(query is null)
                {
                    res.Result = new PagedResponse<List<SelectAdmissionSettings>>();
                    res.IsSuccessful = true;
                    return res;
                }

                var totalRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(db => new SelectAdmissionSettings(db, context.ClassLookUp.Where(x => classes.Contains(x.ClassLookupId.ToString())).ToList())).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);

                res.Message.FriendlyMessage = Messages.GetSuccess;
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

        public async Task<APIResponse<SelectAdmissionSettings>> GetSettingsById(string admissionSettingsId)
        {
            var res = new APIResponse<SelectAdmissionSettings>();
            try
            {
                var classes = context.AdmissionSettings?.Where(d => d.Deleted != true && smsClientId == d.ClientId)?.FirstOrDefault()?.Classes?.Split(',', StringSplitOptions.None).ToList();
                var result = await context.AdmissionSettings
                    .Where(d => d.Deleted != true && smsClientId == d.ClientId && d.AdmissionSettingId == Guid.Parse(admissionSettingsId))
                    .Select(db => new SelectAdmissionSettings(db, context.ClassLookUp.Where(x => classes.Contains(x.ClassLookupId.ToString())).ToList()))
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    res.Message.FriendlyMessage = "No item found";
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
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }
}
