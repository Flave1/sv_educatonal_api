using BLL;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SMP.BLL.Constants;
using SMP.BLL.Services.ResultServices;
using SMP.BLL.Services.WebRequestServices;
using SMP.Contracts.Options;
using SMP.Contracts.PinManagement;
using SMP.Contracts.ResultModels;
using SMP.DAL.Models.PinManagement;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PinManagementService
{
    public class PinManagementService : IPinManagementService
    {
        private readonly DataContext context;
        private readonly IResultsService resultService;
        private readonly IWebRequestService webRequestService;
        private readonly FwsConfigSeetings fwsOptions;
        private readonly RegNumber regNumberOptions;
        public PinManagementService(DataContext context, IResultsService resultService, IWebRequestService webRequestService, IOptions<FwsConfigSeetings> options, IOptions<RegNumber> regNoOptions)
        {
            this.context = context;
            this.resultService = resultService;
            this.webRequestService = webRequestService;
            fwsOptions = options.Value;
            regNumberOptions = regNoOptions.Value;
        }
        async Task<APIResponse<PreviewResult>> IPinManagementService.PrintResultAsync(PrintResultRequest request)
        {
            var res = new APIResponse<PreviewResult>();
            var regNo = GetStudentRealRegNumber(request.RegistractionNumber);
            var student = context.StudentContact.FirstOrDefault(x => x.RegistrationNumber.ToLower() == regNo.ToLower());
            if(student == null)
            {
                res.Message.FriendlyMessage = "Invalid student registration number";
                return res;
            }
            var studentResult = await resultService.GetStudentResultAsync(Guid.Parse(request.SessionClassid), Guid.Parse(request.TermId), student.StudentContactId);
            if (studentResult.Result != null)
            {  
                var pin = await context.UsedPin.Include(d => d.UploadedPin).Where(x => x.UploadedPin.Pin == request.Pin).ToListAsync();
                if (pin.Any())
                {
                    if (pin.Count >= 3)
                    {
                        res.Message.FriendlyMessage = "Pin can not be used more than 3 times";
                        return res;
                    }
                    else
                    {
                        await AddPinAsUsedAsync(request, student.StudentContactId, pin.FirstOrDefault().UploadedPinId);
                        res.Result = studentResult.Result;
                        res.IsSuccessful = true;
                        res.Message.FriendlyMessage = Messages.GetSuccess;
                        return res;
                    }
                }
                else
                {
                    var fwsPayload = new FwsPinValidationRequest
                    {
                        ClientId = fwsOptions.ClientId,
                        Pin = request.Pin,
                        StudentRegNo = regNo,
                    };

                    FwsResponse fwsResponse = await webRequestService.PostAsync<FwsResponse, FwsPinValidationRequest>($"{ fwsOptions.FwsBaseUrl}sms/validate-pin", fwsPayload);
                    if(fwsResponse.status != "success")
                    {
                        res.Message.FriendlyMessage = fwsResponse.message.friendlyMessage;
                        return res;
                    }
                    var uploadedPin = await context.UploadedPin.FirstOrDefaultAsync(x => x.Pin == request.Pin);
                    if(uploadedPin == null)
                    {
                        res.Message.FriendlyMessage = "Pin not uploaded";
                        return res;
                    }
                    await AddPinAsUsedAsync(request, student.StudentContactId, uploadedPin.UploadedPinId);

                    res.Result = studentResult.Result;
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
            }
            else
            { 
                res.Message.FriendlyMessage = "Student Result does not exist";
                return res;
            }
        }


        private async Task AddPinAsUsedAsync(PrintResultRequest request, Guid studentContactId, Guid UploadedPinId)
        {
            try
            {
                var newPin = new UsedPin();
                newPin.SessionClassId = Guid.Parse(request.SessionClassid);
                newPin.SessionTermId = Guid.Parse(request.TermId);
                newPin.StudentContactId = studentContactId;
                newPin.DateUsed = DateTime.UtcNow;
                newPin.UploadedPinId = UploadedPinId;
                await context.UsedPin.AddAsync(newPin);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetStudentRealRegNumber(string regNo)
        {
            var splited = regNo.Split('/');
            if (regNumberOptions.StudentRegNoPosition == 3)
            {
                return splited[2];
            }
            if (regNumberOptions.StudentRegNoPosition == 2)
            {
                return splited[1];
            }
            if (regNumberOptions.StudentRegNoPosition == 1)
            {
                return splited[0];
            }
            return regNo;
        }
    }
}
