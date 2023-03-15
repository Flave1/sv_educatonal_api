using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Admissions
{
    public class CreateAdmissionCandidateCbt
    {
        public string CandidateCategory { get; set; }
        public string CategoryName { get; set; }
        public List<AdmissionCandidateList> AdmissionCandidateList { get; set; }
    }
    public class AdmissionCandidateList
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class SMPCbtCreateCandidateResponse
    {
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
    }
}
