using BLL;
using Contracts.Common;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.Contracts.AdmissionSettings;
using SMP.DAL.Migrations;
using SMP.DAL.Models.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AdmissionServices
{
    public class AdmissionSettingService : IAdmissionSettingService
    {
        private readonly DataContext context;

        public AdmissionSettingService(DataContext context)
        {
            this.context = context;
        }
        public async Task<APIResponse<CreateAdmissionSettings>> CreateSettings(CreateAdmissionSettings request)
        {
            var res = new APIResponse<CreateAdmissionSettings>();

            try
            {
                var setting = await context.AdmissionSettings
                    .Where(d => d.Deleted != true).FirstOrDefaultAsync();

                if (setting == null)
                {
                    var newSetting = new AdmissionSetting
                    {
                        Classes = string.Join(",", request.Classes),
                        AdmissionStatus = request.AdmissionStatus,
                        PassedExamEmail = request.PassedExamEmail,
                        FailedExamEmail = request.FailedExamEmail,
                        ScreeningEmail = request.ScreeningEmail,
                        RegistrationFee = request.RegistrationFee
                    };
                    context.AdmissionSettings.Add(newSetting);
                }
                else
                {
                    setting.Classes = string.Join(",", request.Classes);
                    setting.AdmissionStatus = request.AdmissionStatus;
                    setting.PassedExamEmail = request.PassedExamEmail;
                    setting.FailedExamEmail = request.FailedExamEmail;
                    setting.ScreeningEmail = request.ScreeningEmail;
                    setting.RegistrationFee = request.RegistrationFee;
                }

                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Saved;
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

        public async Task<APIResponse<bool>> DeleteSettings(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var settings = await context.AdmissionSettings.Where(d => d.Deleted != true && d.AdmissionSettingId == Guid.Parse(request.Item)).FirstOrDefaultAsync();
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
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<SelectAdmissionSettings>> GetSettings()
        {
            var res = new APIResponse<SelectAdmissionSettings>();
            try
            {
                var classes =  context.AdmissionSettings?.Where(d => d.Deleted != true)?.FirstOrDefault()?.Classes?.Split(',').ToList();
                var result = await context.AdmissionSettings
                    .Where(d => d.Deleted != true)
                    .Select(db => new SelectAdmissionSettings(db, context.ClassLookUp.Where(x=> classes.Contains(x.ClassLookupId.ToString())).ToList()))
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
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }
}
