using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.ParentModels
{
    public class ParentDashboardCount
    {
        public int TotalWards { get; set; }
        public int TotalAssessment { get; set; }
        public int TeachersNote { get; set; }
        public int WardsNote { get; set; }
    }
}
