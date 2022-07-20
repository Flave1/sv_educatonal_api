using BLL;
using SMP.BLL.Helper.Model;
using SMP.Contracts.PinManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Helper
{
    public interface IClientService
    {

        Task<APIResponse<GetSmsRequest>> GetBy(GetSmsRequest request);
    }
    public class GetSmsRequest
    {
        public string ClientId { get; set; }
        public string StudentRegNo { get; set; }
        public string Pin { get; set; }
    }
}
