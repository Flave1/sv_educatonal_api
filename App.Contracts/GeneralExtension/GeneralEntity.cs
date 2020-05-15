using System;
using System.Collections.Generic;
using System.Text;

namespace App.Contracts.GeneralExtension
{
    public class GeneralEntity
    {
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int CompanyId { get; set; }
    }
}
