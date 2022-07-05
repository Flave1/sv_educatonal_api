using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.GradeModels
{
    public class AddGradeGroupModel
    {
        public string GradeGroupName { get; set; }
        public GradesModel[] Grades { get; set; }
        //public List<string> Classes { get; set; } 
    }

   
    public class GradesModel
    {
        public string GradeName { get; set; }
        public string Remark { get; set; }
        public int UpperLimit { get; set; }
        public int LowerLimit { get; set; }
    }

   
}