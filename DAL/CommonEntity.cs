using System;

namespace DAL
{
    public class CommonEntity
    { 
        public bool Deleted { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string ClientId { get; set; }
    }
}
