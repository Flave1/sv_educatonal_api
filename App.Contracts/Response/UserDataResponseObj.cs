using System;
using System.Collections.Generic;
using System.Text;

namespace App.Contracts.Response
{
    public class UserDataResponseObj
    {
        public int CompanyId { get; set; }
        public int StaffId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string StaffName { get; set; }
        public string CustomerName { get; set; }
        public int? BranchId { get; set; }
        public int? CountryId { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
