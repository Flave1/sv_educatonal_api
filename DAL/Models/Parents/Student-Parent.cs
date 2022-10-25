
using DAL.StudentInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Parents
{
    public class Student_Parent
    {
        public string parentId { get; set; }
        public string studentAccountId { get; set; }
        public Parents parents { get; set; }
        public ICollection<StudentContact> Students { get; set; }
    }
}
