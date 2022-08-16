using DAL.ClassEntities;
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
        public string ListOfStudentIds { get; set; }
    }
}
