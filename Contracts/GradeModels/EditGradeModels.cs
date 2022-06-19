using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.GradeModels
{
    public class EditGradeGroupModel
    {
        public Guid GradeGroupId { get; set; }
        public string GradeGroupName { get; set; }
        public GradesModel[] Grades { get; set; }
        //public List<string> Classes { get; set; }
    }



   
}