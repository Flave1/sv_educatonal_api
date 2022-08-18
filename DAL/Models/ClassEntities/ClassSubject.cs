using DAL;
using DAL.ClassEntities;
using DAL.SubjectModels;
using DAL.TeachersInfor;
using SMP.DAL.Models.AssessmentEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.ClassEntities
{
    public class SessionClassSubject : CommonEntity
    {
        [Key]
        public Guid SessionClassSubjectId { get; set; }
        public Guid SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public Guid SubjectTeacherId { get; set; }
        [ForeignKey("SubjectTeacherId")]
        public virtual Teacher SubjectTeacher { get; set; }
        public int ExamScore { get; set; }
        public int AssessmentScore { get; set; }
        public virtual ICollection<ClassAssessment> ClassAssessments { get; set; }
        public virtual ICollection<HomeAssessment> HomeAssessments { get; set; }
    }
}
