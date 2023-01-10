using SMP.Contracts.PinManagement;

namespace SMP.Contracts.Authentication
{
    public class SmsClientInformation
    {
        public SmsClientInformationResult Result { get; set; }
        public string Status { get; set; }
        public Message Message { get; set; }
    }
    public class SmsClientInformationResult
    {
        public string ClientId { get; set; }
        public string SchoolName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string IpAddress { get; set; }
        public string SmsapI_KEY { get; set; }
        public string BaseUrlAppendix { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
