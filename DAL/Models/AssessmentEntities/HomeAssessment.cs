using DAL.ClassEntities;
using SMP.DAL.Models.ClassEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.AssessmentEntities
{
    public class HomeAssessment
    {
        [Key]
        public Guid HomeAssessmentId { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int AssessmentScore { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public Guid SessionClassSubjectId { get; set; }
        [ForeignKey("SessionClassSubjectId")]
        public SessionClassSubject SessionClassSubject { get; set; }
        public Guid? SessionClassGroupId { get; set; }
        [ForeignKey("SessionClassGroupId")]
        public SessionClassGroup SessionClassGroup { get; set; }
        public Guid SessionTermId { get; set; }
        [ForeignKey("SessionTermId")]
        public SessionClass SessionTerm{ get; set; }
        public virtual ICollection<HomeAssessmentFeedBack> HomeAssessmentFeedBacks { get; set; }
    }
}
