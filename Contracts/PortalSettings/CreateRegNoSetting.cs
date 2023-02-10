using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PortalSettings
{
    public class CreateRegNoSetting
    {
        public string StudentRegNoPrefix { get; set; }
        public string StudentRegNoSufix { get; set; }
        public string TeacherRegNoPrefix { get; set; }
        public string TeacherRegNoSufix { get; set; }
        public int RegNoPosition { get; set; }
        public string RegNoSeperator { get; set; }
    }
}
