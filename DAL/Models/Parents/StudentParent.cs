using DAL.StudentInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SMP.Contracts.Parents
{
    public class StudentParent
    {
        [Key]
        public string StudentAccountId { get; set; }
        public string ParentId { get; set; }
        public Parents Parents { get; set; }
        public ICollection<StudentContact> Students { get; set; }
    }
}