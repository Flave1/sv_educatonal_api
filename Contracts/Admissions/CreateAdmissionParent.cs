using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Admissions
{
    public class CreateAdmissionParent
    {
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Relationship { get; set; }
        public string PhoneNumber { get; set; }
        public string SchoolUrl { get; set; }
    }
}
