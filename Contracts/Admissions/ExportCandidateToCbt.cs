using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Admissions
{
    public class ExportCandidateToCbt
    {
        public string ClassId { get; set; }
        public string CategoryName { get; set; }
        public string CandidateCategory { get; set; }
    }
}
