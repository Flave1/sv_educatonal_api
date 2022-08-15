using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.AssessmentEntities
{
    public class AssessmentScoreRecord
    {
        public Guid  AssessmentScoreRecordId { get; set; }
        public int AssessmentType { get; set; }
        public Guid? HomeAssessmentId { get; set; }
        [ForeignKey("HomeAssessmentId")]
        public HomeAssessment HomeAssessment { get; set; }
        public Guid? ClassAssessmentId { get; set; }
        [ForeignKey("ClassAssessmentId")]
        public ClassAssessment ClassAssessment { get; set; }

    }
}
