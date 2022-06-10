using DAL.ClassEntities;
using SMP.DAL.Models.GradeEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.GradeModels
{


    public class GetGradeGroupModel 
    {
        public Guid GradeGroupId { get; set; }
        public string GradeGroupName { get; set; }
        public List<GetGradesModel> Grades { get; set; } = new List<GetGradesModel>();
        public List<GetClassGradeModel> Classes { get; set; } = new List<GetClassGradeModel>();
        public GetGradeGroupModel(GradeGroup gg)
        {
            GradeGroupId = gg.GradeGroupId;
            GradeGroupName = gg.GradeGroupName;
            if (gg.ClassGrades.Any())
            {
                Classes = gg.ClassGrades.OrderBy(s => s.SessionClass.Class.Name).Select(cg => new GetClassGradeModel
                {
                    ClassGradeId = cg.ClassGradeId,
                    SessionClassId = cg.SessionClassId,
                    ClassName = cg.SessionClass.Class.Name,
                }).ToList();
            }
            if (gg.Grades.Any())
            {
                Grades = gg.Grades.OrderBy(d => d.GradeName).Select(g => new GetGradesModel
                {
                    GradeId = g.GradeId,
                    GradeName = g.GradeName,
                    LowerLimit = g.LowerLimit,
                    Remark = g.Remark,
                    UpperLimit = g.UpperLimit
                }).ToList();
            }
        }
    }

    public class GetGradesModel
    {
        public Guid GradeId { get; set; }
        public string GradeName { get; set; }
        public string Remark { get; set; }
        public int UpperLimit { get; set; }
        public int LowerLimit { get; set; }
    }

    public class GetClassGradeModel
    {
        public Guid ClassGradeId { get; set; }
        public Guid SessionClassId { get; set; }
        public string ClassName { get; set; }
    }

    public class GetSessionClass
    {
        public string SessionClassId { get; set; }
        public string ClassName { get; set; }

        public GetSessionClass(SessionClass sClass)
        {
            SessionClassId = sClass.SessionClassId.ToString();
            ClassName = sClass.Class.Name;
           
        }
    }

}
