using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Class
{
    public class GetApplicationLookups
    { 
        public string LookupId { get; set; }
        public string Name { get; set; }
        public string GradeLevelId { get; set; }
        public bool IsActive { get; set; }
        public string GradeLevelName { get; set; }
    }
    public class ApplicationLookupCommand
    {
        public string LookupId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string GradeLevelId { get; set; }
    }

    public class GetSubjectTeacher
    {
        public bool subjectTeacherId { get; set; }
    }
}
