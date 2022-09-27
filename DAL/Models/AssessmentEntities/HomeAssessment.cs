using DAL;
using DAL.ClassEntities;
using DAL.TeachersInfor;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.AssessmentEntities
{
    public class HomeAssessment : CommonEntity
    {
        [Key]
        public Guid HomeAssessmentId { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Comment { get; set; }
        public int Status { get; set; }
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
        public SessionTerm SessionTerm { get; set; }
        public string DateDeadLine { get; set; }
        public string TimeDeadLine { get; set; }
        public Guid? TeacherId { get; set; }
        public virtual ICollection<HomeAssessmentFeedBack> HomeAssessmentFeedBacks { get; set; }
        public virtual ICollection<AssessmentScoreRecord> AssessmentScoreRecord { get; set; }
    }
}
