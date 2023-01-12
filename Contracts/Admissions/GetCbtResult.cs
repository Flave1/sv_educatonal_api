using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Admissions
{
    public class GetCbtResult
    {
        public string CandidateName { get; set; }
        public string CandidateId { get; set; }
        public string CandidateEmail { get; set; }
        public string ExaminationName { get; set; }
        public int TotalScore { get; set; }
        public string Status { get; set; }
    }
}
