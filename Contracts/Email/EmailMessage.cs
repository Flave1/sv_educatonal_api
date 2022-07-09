using System.Collections.Generic;

namespace Contracts.Email
{
    public class EmailMessage
    {
        public string Content { get; set; }
        public string Subject { get; set; }
        public string SentBy { get; set; }
        public List<EmailAddress> ToAddresses { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> FromAddresses { get; set; } = new List<EmailAddress>();
    }

    public class EmailAddress
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
