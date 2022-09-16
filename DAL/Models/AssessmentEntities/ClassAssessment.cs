using DAL.ClassEntities;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.SessionEntities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.AssessmentEntities
{
    public class ClassAssessment
    {
        [Key]
        public Guid ClassAssessmentId { get; set; }
        public string Description { get; set; }
        public decimal AssessmentScore { get; set; }
        public Guid Scorer { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public Guid? SessionClassSubjectId { get; set; }
        [ForeignKey("SessionClassSubjectId")]
        public SessionClassSubject SessionClassSubject { get; set; }
        public string ListOfStudentIds { get; set; }
        public Guid? SessionTermId { get; set; }
        [ForeignKey("SessionTermId")]
        public SessionTerm SessionTerm { get; set; }
    }


}
