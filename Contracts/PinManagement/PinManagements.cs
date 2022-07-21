using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PinManagement
{
    public class PrintResultRequest
    {
        public string Pin { get; set; } 
        public string RegistractionNumber { get; set; }
        public string SessionClassid { get; set; }
        public string TermId { get; set; }
    }
    public class UploadPinRequest
    {
        public string Pin { get; set; }
        public int ExcelLineNumber { get; set; }
    }
        public class FwsPinValidationRequest
    {
        public string Pin { get; set; }
        public string StudentRegNo { get; set; }
        public string ClientId { get; set; }
    }

    public class Message
    {
        public string friendlyMessage { get; set; }
        public object technicalMessage { get; set; }
    }

    public class FwsResponseResult
{
        public string pin { get; set; }
        public string studentRegNo { get; set; }
        public object apiKey { get; set; }
        public string clientId { get; set; }
    }

    public class FwsResponse
    {
        public FwsResponseResult result { get; set; }
        public string status { get; set; }
        public Message message { get; set; }
    }

}
