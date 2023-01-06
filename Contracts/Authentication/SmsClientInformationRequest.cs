using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Authentication
{
    public class SmsClientInformationRequest
    {
        public string ClientId { get; set; }
        public string ApiKey { get; set; }
    }
}
