using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore;
using System;
using SMP.BLL.Helper.Model;
using SMP.BLL.Helper;

namespace BLL.Helper
{ 
    public class ClientService : IClientService
    {
        private readonly IClientBaseService provider; 
        public ClientService(IClientBaseService provider)
        { 
            this.provider = provider;
        }
        async Task<APIResponse<GetSmsRequest>> IClientService.GetBy(GetSmsRequest request)
        {
            try
            {
                 
                var response = await provider.Get<APIResponse<GetSmsRequest>>("sms/validate-pin" + request);
                if (response != null)
                {
                    response.IsSuccessful = true;
                    response.Result = request; 
                }
                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
