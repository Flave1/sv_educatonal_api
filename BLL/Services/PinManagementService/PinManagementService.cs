using BLL;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SMP.BLL.Constants;
using SMP.BLL.Helper;
using SMP.BLL.Services.ResultServices;
using SMP.Contracts.PinManagement;
using SMP.DAL.Models.PinManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PinManagementService
{
    public class PinManagementService : IPinManagementService
    {
        private readonly DataContext context;
        private readonly IResultsService service;
        private readonly IClientService client;
        public static IConfiguration config;
        public PinManagementService(DataContext context, IResultsService service, IClientService client, IConfiguration configuration)
        {
            this.context = context;
            this.service = service;
            this.client = client;
            config = configuration;
        }
        async Task<APIResponse<UploadedPins>> IPinManagementService.PrintResultAsync(UploadedPins request)
        {
            var res = new APIResponse<UploadedPins>();
            var result = service.GetStudentResultAsync(request.SessionClassid, request.TermId, request.StudentContactId);
            if (result != null)
            {  
                var usedPin = await context.UsedPin.Where(x => x.UploadedPinId == request.Pin).ToListAsync();
                if (usedPin != null)
                {
                    if (usedPin.Count >= 3)
                    {
                        res.Message.FriendlyMessage = "Pin can not be used more than 3 times";
                        return res;
                    }
                    else
                    {

                        foreach (var pin in usedPin)
                        {

                            var used  = new UsedPin()
                            {
                                UploadedPinId = pin.UploadedPinId,
                                SessionId = pin.SessionId,
                                SessionTermId = pin.SessionTermId,
                                StudentContactId = pin.StudentContactId,
                                DateUsed = pin.DateUsed,

                            };
                            await context.UsedPin.AddAsync(used);
                            await context.SaveChangesAsync();

                        }

                        res.Message.FriendlyMessage = Messages.Saved;
                        res.IsSuccessful = true;
                        return res;
                    }
                }
                else
                {
                    //todo:
                    await client.GetBy(new GetSmsRequest() { ClientId = config.GetSection("clientId:D7F710A5-592F-4390-068C-08DA686DA23E").Value, Pin = request.Pin.ToString(), StudentRegNo = request.RegistractionNumber});
                    res.Message.FriendlyMessage = "Pin does not exist";
                    res.IsSuccessful = true;
                    return res;
                }

            }
            else
            { 
                res.Message.FriendlyMessage = "Student Result does not exist";
                return res;
            } 
            res.Result = request;
            return res;
        }
    }
}
