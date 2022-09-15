﻿using BLL;
using BLL.Utilities;
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
using SMP.Contracts.Routes;
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
        private readonly FwsConfigSettings fwsOptions;
        private readonly RegNumber regNumberOptions;
        private readonly IHttpContextAccessor accessor;
        public PinManagementService(DataContext context, IResultsService resultService, IWebRequestService webRequestService, IOptions<FwsConfigSettings> options, IOptions<RegNumber> regNoOptions, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.resultService = resultService;
            this.webRequestService = webRequestService;
            fwsOptions = options.Value;
            regNumberOptions = regNoOptions.Value;
            this.accessor = accessor;
        }
        async Task<APIResponse<PrintResult>> IPinManagementService.PrintResultAsync(PrintResultRequest request)
        {
            var res = new APIResponse<PrintResult>();
            try
            {
                var regNo = (this as IPinManagementService).GetStudentRealRegNumber(request.RegistractionNumber);
                var studentInfo = context.StudentContact.FirstOrDefault(x => x.RegistrationNumber.ToLower() == regNo.ToLower());
                if (studentInfo == null)
                {
                    res.Message.FriendlyMessage = "Invalid student registration number";
                    return res;
                }
                var studentClassArchive = context.SessionClassArchive.FirstOrDefault(s => s.SessionTermId == Guid.Parse(request.TermId) && s.StudentContactId == studentInfo.StudentContactId);
                if(studentClassArchive is null)
                {
                    res.Message.FriendlyMessage = "Republish Class result to capture this student result";
                    return res;
                }
                request.SessionClassid = studentClassArchive.SessionClassId.Value;
                var studentResult = await resultService.GetStudentResultForPrintingAsync(request.SessionClassid, Guid.Parse(request.TermId), studentInfo.StudentContactId);
                if (studentResult.Result != null)
                {
                    studentResult.Result.IsPrint = true;
                    studentResult.Result.IsPreview = false;
                    if (!studentResult.Result.isPublished)
                    {
                        res.Message.FriendlyMessage = "Result not published";
                        return res;
                    }
                    var pin = await context.UsedPin.Include(d => d.UploadedPin).Include(d => d.Sessionterm).ThenInclude(d => d.Session).Where(x => x.UploadedPin.Pin == request.Pin).ToListAsync();
                    if (pin.Any())
                    {
                        if (pin.Count >= 3)
                        {
                            res.Message.FriendlyMessage = "Pin can not be used more than 3 times";
                            return res;
                        }
                        else
                        {

                            if (pin.FirstOrDefault().SessionTermId != Guid.Parse(request.TermId))
                            {
                                res.Message.FriendlyMessage = $"Pin can only be used for {pin.FirstOrDefault().Sessionterm.TermName} " +
                                    $"term of {pin.FirstOrDefault().Sessionterm.Session.StartDate }/{pin.FirstOrDefault().Sessionterm.Session.EndDate } Session";
                                return res;
                            }
                            if (pin.FirstOrDefault().StudentContactId != studentInfo.StudentContactId)
                            {
                                res.Message.FriendlyMessage = $"Pin can only be used by one (1) student";
                                return res;
                            }
                            await AddPinAsUsedAsync(request, studentInfo.StudentContactId, pin.FirstOrDefault().UploadedPinId);
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

                        FwsPinValResponse fwsResponse = await webRequestService.PostAsync<FwsPinValResponse, FwsPinValidationRequest>($"{fwsOptions.FwsBaseUrl}{fwsRoutes.validatePin}", fwsPayload);

                        if (fwsResponse.status != "success")
                        {
                            res.Message.FriendlyMessage = fwsResponse.message.friendlyMessage;
                            return res;
                        }
                        var uploadedPin = await context.UploadedPin.FirstOrDefaultAsync(x => x.Pin == request.Pin);
                        if (uploadedPin == null)
                        {
                            res.Message.FriendlyMessage = "Pin not uploaded";
                            return res;
                        }
                        await resultService.UpdateStudentPrintStatusAsync(studentInfo.StudentContactId, Guid.Parse(request.TermId), true);
                        await AddPinAsUsedAsync(request, studentInfo.StudentContactId, uploadedPin.UploadedPinId);

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
            catch (ArgumentException ex)
            {
                res.Message.FriendlyMessage = ex.Message;
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
        private async Task AddPinAsUsedAsync(BatchPrintResultRequest2 request, Guid studentContactId, Guid UploadedPinId)
        {
            try
            {
                var newPin = new UsedPin();
                newPin.SessionClassId = request.SessionClassId;
                newPin.SessionTermId = request.TermId;
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

        string IPinManagementService.GetStudentRealRegNumber(string regNo)
        {
            try
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
            catch (Exception)
            {
                
                throw new ArgumentException("Please ensure registeration number is in correct format");
            }
        }
        async Task<APIResponse<UploadPinRequest>> IPinManagementService.UploadPinAsync(UploadPinRequest request)
        {
            var res = new APIResponse<UploadPinRequest>();

            try
            {
                List<UploadPinRequest> uploadedRecord = new List<UploadPinRequest>();
                var files = accessor.HttpContext.Request.Form.Files;

                if(files.Count() == 0)
                {
                    res.Message.FriendlyMessage = $"No File selected";
                    return res;
                }
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
                    var fwsResponse = await ValidatePinsOnUploadAsync(uploadedRecord);
                    if (fwsResponse == null)
                    { 
                        res.Message.FriendlyMessage = Messages.Unreachable;
                        return res;
                    }
                    if (fwsResponse.status != "success")
                    { 
                        res.Message.FriendlyMessage = fwsResponse.message.friendlyMessage;
                        return res;
                    }

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
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Pin uploaded successfully";
                    return res;
                }
               
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "File not found";
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
            return  await Task.Run(() => res);
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
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var res = new APIResponse<PinDetail>();
            res.Result = context.UsedPin
            .Include(x => x.UploadedPin)
            .Include(x => x.Student).ThenInclude(d => d.User)
            .Include(x => x.Sessionterm)
            .Include(x => x.SessionClass).ThenInclude(x => x.Session)
            .Where(x => x.UploadedPin.Pin == pin).AsEnumerable()
            .GroupBy(d => d.UploadedPinId).Select(grp => grp)
                .Select(f => new PinDetail(f, regNoFormat)).FirstOrDefault();
            res.IsSuccessful = true;
            return await Task.Run(() => res);
        }

        async Task<APIResponse<List<PrintResult>>> IPinManagementService.PrintBatchResultResultAsync(BatchPrintResultRequest2 request)
        {
            var res = new APIResponse<List<PrintResult>>();
            using (var trans = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var students = context.ScoreEntry.Include(x => x.ClassScoreEntry)
                        .Where(x => x.ClassScoreEntry.SessionClassId == request.SessionClassId && x.SessionTermId == request.TermId && x.IsOffered).Select(x => x.StudentContact);
                    //GetStudentId(request.SessionClassId, request.TermId);

                    var isArchived = IsResultArchived(request.SessionClassId, request.TermId, students.Select(x => x.StudentContactId).Distinct().ToList());
                    if (!isArchived)
                    {
                        await trans.DisposeAsync();
                        res.Message.FriendlyMessage = "Republish Class result to capture all student results";
                        return res;
                    }

                    var studentResults = await resultService.GetStudentResultForBatchPrintingAsync(request.SessionClassId, request.TermId);
                    if (studentResults.Result.Any())
                    {
                        var unUsedPins = GetUnusedPins(request.Students);
                        if (request.Students > unUsedPins.Count())
                        {
                            await trans.DisposeAsync();
                            res.Message.FriendlyMessage = "Insufficient pin to print students results";
                            return res;
                        }

                        var fwsResponse = await ValidatePinsAsync(unUsedPins, students.Select(x => x.RegistrationNumber).Distinct().ToList());
                        if (fwsResponse == null)
                        {
                            await trans.DisposeAsync();
                            res.Message.FriendlyMessage = Messages.Unreachable;
                            return res;
                        }
                        if (fwsResponse.status != "success")
                        {
                            await trans.DisposeAsync();
                            res.Message.FriendlyMessage = fwsResponse.message.friendlyMessage;
                            return res;
                        }

                        foreach(var pinResult in fwsResponse.result)
                        {

                            var pin = await context.UsedPin.Include(d => d.UploadedPin)
                                                       .Include(d => d.Sessionterm).ThenInclude(d => d.Session).Where(x => x.UploadedPin.Pin == pinResult.pin).ToListAsync();

                            var regNo = (this as IPinManagementService).GetStudentRealRegNumber(pinResult.studentRegNo);
                            var studentInfor = context.StudentContact.FirstOrDefault(x => x.RegistrationNumber == regNo);
                            if (pin.Any())
                            {
                                if (pin.Count >= 3)
                                {
                                    await trans.DisposeAsync();
                                    res.Message.FriendlyMessage = "Pin can not be used more than 3 times";
                                    return res;
                                }
                                else
                                {
                                    if (pin.FirstOrDefault().SessionTermId != request.TermId)
                                    {
                                        await trans.DisposeAsync();
                                        res.Message.FriendlyMessage = $"Pin can only be used for {pin.FirstOrDefault().Sessionterm.TermName} " +
                                            $"term of {pin.FirstOrDefault().Sessionterm.Session.StartDate }/{pin.FirstOrDefault().Sessionterm.Session.EndDate } Session";
                                        return res;
                                    }
                                   
                                    if (pin.FirstOrDefault().StudentContactId != studentInfor.StudentContactId)
                                    {
                                        await trans.DisposeAsync();
                                        res.Message.FriendlyMessage = $"Pin can only be used by one (1) student";
                                        return res;
                                    }
                                    await AddPinAsUsedAsync(request, studentInfor.StudentContactId, pin.FirstOrDefault().UploadedPinId); 
                                }
                            }
                            else
                            {
                                await resultService.UpdateStudentPrintStatusAsync(studentInfor.StudentContactId, request.TermId, true);
                                await AddPinAsUsedAsync(request, studentInfor.StudentContactId, context.UploadedPin.FirstOrDefault(x => x.Pin == pinResult.pin).UploadedPinId);
                            }
                        }
                        await trans.CommitAsync();
                        res.Result = studentResults.Result;
                        res.IsSuccessful = true;
                        res.Message.FriendlyMessage = Messages.GetSuccess;
                        return res;

                    }
                    else
                    {
                        await trans.DisposeAsync();
                        res.Message.FriendlyMessage = "Student Results does not exist";
                        return res;
                    }
                }
                catch (ArgumentException ex)
                {
                    await trans.DisposeAsync();
                    res.Message.FriendlyMessage = ex.Message;
                    return res;
                }
            }
           
        }


        List<UploadedPin> GetUnusedPins(int number) 
            => context.UploadedPin.Include(x => x.UsedPin).Where(d => d.Deleted == false && !d.UsedPin.Any()).Take(number).ToList(); 

       
        bool IsResultArchived(Guid classId, Guid termId, List<Guid> stdIds)
        {
            var stds = context.SessionClassArchive.Where(s => s.SessionTermId == termId && s.SessionClassId == classId && s.IsPublished).Select(x => x.StudentContactId).ToList();
            return stdIds.All(x => stds.Contains(x));
        }
        
        async Task<FwsMultiPinValResponse> ValidatePinsAsync(List<UploadedPin> pins, List<string> stds)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var stdsAndPins = stds.Zip(pins, (s, p) => new FwsPinValidationRequest
            {
               Pin = p.Pin,
               StudentRegNo = regNoFormat.Replace("%VALUE%", s),
               ClientId = fwsOptions.ClientId,
           }).ToList();
        
            return await webRequestService.PostAsync<FwsMultiPinValResponse, List<FwsPinValidationRequest>>($"{fwsOptions.FwsBaseUrl}{fwsRoutes.validateMultiPins}", stdsAndPins);

        }

        async Task<FwsMultiPinOnUploadValResponse> ValidatePinsOnUploadAsync(List<UploadPinRequest> pins)
        {
            var request = new PinsValOnUplaodRequest
            {
                ClientId = fwsOptions.ClientId,
                Pins = pins.Select(x => new PinObject { Pin = x.Pin, ExcelLine = x.ExcelLineNumber }).ToList()
            };

            return await webRequestService.PostAsync<FwsMultiPinOnUploadValResponse, PinsValOnUplaodRequest>($"{fwsOptions.FwsBaseUrl}{fwsRoutes.validateMultiPinsOnUpload}", request);

        }
    }
}