using DAL.ClassEntities;
using SMP.DAL.Models.ClassEntities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.AssessmentEntities
{
    public class ClassAssessment
    {
        public Guid ClassAssessmentId { get; set; }
        public string Description { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public Guid? SessionClassSubjectId { get; set; }
        [ForeignKey("SessionClassSubjectId")]
        public SessionClassSubject SessionClassSubject { get; set; }
        public string ListOfStudentIds { get; set; }
    }
}
