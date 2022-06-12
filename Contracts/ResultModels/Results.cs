using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.ResultModels
{
    public class GetClasses
    {
        public string SessionClass { get; set; }
        public string SessionClassId { get; set; }
    }

    public class GetClassSubjects
    {
        public string SessionClass { get; set; }
        public string SessionClassId { get; set; }
    }
}
