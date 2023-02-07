namespace SMP.DAL.Models
{
    public class FwsClientInformation
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
        public string UserId { get; set; }
    }
    public class RegNumber
    {
        public string Student { get; set; }
        public string Teacher { get; set; }
        public int StudentRegNoPosition { get; set; }
        public string Separator { get; set; }
    }
}
