using DAL.ClassEntities;
using DAL.StudentInformation;
using DAL.SubjectModels;
using DAL.TeachersInfor;
using SMP.DAL.Models.PinManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Dashboard
{
    public class GetDashboard
    { 
        public long TotalEnrolledStudent { get; set; }
        public long TotalClass { get; set; }
        public long TotalStaff { get; set; }
        public long TotalPins { get; set; }
        public long TotalSubjects { get; set; }
    }
}
