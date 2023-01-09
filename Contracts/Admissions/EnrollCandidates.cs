using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Admissions
{
    public class EnrollCandidates
    {
        public List<string> AdmissionIds { get; set; }
        public string SessionClassId { get; set; }
    }
}
