using BLL;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using SMP.BLL.Constants;
using SMP.BLL.Services.ResultServices;
using SMP.BLL.Services.WebRequestServices;
using SMP.Contracts.Options;
using SMP.Contracts.PinManagement;
using SMP.Contracts.ResultModels;
using SMP.DAL.Models.PinManagement;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IHttpContextAccessor accessor;
        public PinManagementService(DataContext context, IResultsService resultService, IWebRequestService webRequestService, IOptions<FwsConfigSeetings> options, IOptions<RegNumber> regNoOptions, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.resultService = resultService;
            this.webRequestService = webRequestService;
            fwsOptions = options.Value;
            regNumberOptions = regNoOptions.Value;
            this.accessor = accessor;
        }
        async Task<APIResponse<PreviewResult>> IPinManagementService.PrintResultAsync(PrintResultRequest request)
        {
            var res = new APIResponse<PreviewResult>();
            var regNo = GetStudentRealRegNumber(request.RegistractionNumber);
            var student = context.StudentContact.FirstOrDefault(x => x.RegistrationNumber.ToLower() == regNo.ToLower());
            if (student == null)
            {
                res.Message.FriendlyMessage = "Invalid student registration number";
                return res;
            }
            request.SessionClassid = student.SessionClassId;
            var studentResult = await resultService.GetStudentResultAsync(student.SessionClassId, Guid.Parse(request.TermId), student.StudentContactId);
            if (studentResult.Result != null)
            {
                var pin = await context.UsedPin.Include(d => d.UploadedPin).Include(d => d.Sessionterm).Where(x => x.UploadedPin.Pin == request.Pin).ToListAsync();
                if (pin.Any())
                {
                    if (pin.Count >= 3)
                    {
                        res.Message.FriendlyMessage = "Pin can not be used more than 3 times";
                        return res;
                    }
                    else
                    {
                       
                        if(pin.FirstOrDefault().SessionTermId != Guid.Parse(request.TermId))
                        {
                            res.Message.FriendlyMessage = $"Pin can only be used for {pin.FirstOrDefault().Sessionterm.TermName} term";
                            return res;
                        }
                        if (pin.FirstOrDefault().StudentContactId != student.StudentContactId)
                        {
                            res.Message.FriendlyMessage = $"Pin can only be used by one (1) student";
                            return res;
                        }
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
                    //FwsResponse fwsResponse = await webRequestService.PostAsync<FwsResponse, FwsPinValidationRequest>($"{fwsOptions.FwsBaseUrl}validate-pin", fwsPayload);
                    if (fwsResponse.status != "success")
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
                newPin.SessionClassId = request.SessionClassid;
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
        async Task<APIResponse<UploadPinRequest>> IPinManagementService.UploadPinAsync(UploadPinRequest request)
        {
            var res = new APIResponse<UploadPinRequest>();

            try
            {
                List<UploadPinRequest> uploadedRecord = new List<UploadPinRequest>();
                var files = accessor.HttpContext.Request.Form.Files;
                var byteList = new List<byte[]>();
                foreach (var fileBit in files)
                {
                    if (fileBit.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await fileBit.CopyToAsync(ms);
                            byteList.Add(ms.ToArray());
                        }
                    }
                }

                if (byteList.Count() > 0)
                {
                    foreach (var item in byteList)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (MemoryStream stream = new MemoryStream(item))
                        using (ExcelPackage excelPackage = new ExcelPackage(stream))
                        {
                            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                            int totalRows = workSheet.Dimension.Rows;
                            int totalColumns = workSheet.Dimension.Columns;
                            if (totalColumns != 2)
                            {
                                res.Message.FriendlyMessage = $"Two (2) Columns Expected";
                                return res;
                            }
                            for (int i = 2; i <= totalRows; i++)
                            {
                                uploadedRecord.Add(new UploadPinRequest
                                {
                                    ExcelLineNumber = i,
                                    Serial = workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : null,
                                    Pin = workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : null,

                                });
                            }
                        }
                    }
                }

                if (uploadedRecord.Count > 0)
                {
                    foreach (var item in uploadedRecord)
                    {
                        if (string.IsNullOrEmpty(item.Pin))
                        {
                            res.Message.FriendlyMessage = $"Pin cannot be empty detected on line {item.ExcelLineNumber}";
                            return res;
                        }
                        var current_item = context.UploadedPin.FirstOrDefault(e => e.Pin == item.Pin);
                        if (current_item == null)
                        {
                            current_item = new UploadedPin();
                            current_item.Pin = item.Pin;
                            current_item.Serial = item.Serial;
                            await context.UploadedPin.AddAsync(current_item);
                        }
                        else
                        {
                            res.Message.FriendlyMessage = $"{item.Pin} already uploaded detected on line {item.ExcelLineNumber}";
                            return res;
                        }
                    }
                    await context.SaveChangesAsync();
                }
               
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Pin uploaded successfully";
                return res;

            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = $" {ex?.Message}";
                return res;
            }
        }
       
        async Task<APIResponse<List<GetPins>>> IPinManagementService.GetAllUnusedPinsAsync()
        {
            var res = new APIResponse<List<GetPins>>();
            var result = res.Result;
            var allPins = context.UploadedPin.Where(d => d.Deleted == false);
            var usedPinIds = context.UsedPin.Where(d => d.Deleted == false).Select(x => x.UploadedPinId);

            res.Result = await allPins.OrderByDescending(x => x.CreatedOn).Where(d => !usedPinIds.Contains(d.UploadedPinId)).Select(f => new GetPins(f)).ToListAsync();
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetPins>>> IPinManagementService.GetAllUsedPinsAsync()
        {

            

            var res = new APIResponse<List<GetPins>>();
            res.Result =  context.UsedPin.Where(d => d.Deleted == false)
                .Include(x => x.UploadedPin)
                .Include(x => x.Student).ThenInclude(d => d.User)
                .Include(x => x.Sessionterm)
                .OrderByDescending(x => x.CreatedOn)
                .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                .AsEnumerable()
                .GroupBy(d => d.UploadedPinId).Select(grp => grp)
                .Select(f => new GetPins(f)).ToList();

            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PinDetail>> IPinManagementService.GetUnusedPinDetailAsync(string pin)
        {
            var res = new APIResponse<PinDetail>();
            res.Result = await context.UploadedPin.Where(x => x.Pin == pin).Select(e => new PinDetail(e)).FirstOrDefaultAsync();
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PinDetail>> IPinManagementService.GetUsedPinDetailAsync(string pin)
        {
            var res = new APIResponse<PinDetail>();
            res.Result = context.UsedPin
            .Include(x => x.UploadedPin)
            .Include(x => x.Student).ThenInclude(d => d.User)
            .Include(x => x.Sessionterm)
            .Include(x => x.SessionClass).ThenInclude(x => x.Session)
            .Where(x => x.UploadedPin.Pin == pin).AsEnumerable()
            .GroupBy(d => d.UploadedPinId).Select(grp => grp)
                .Select(f => new PinDetail(f)).FirstOrDefault();
            res.IsSuccessful = true;
            return res;
        }
    }
}