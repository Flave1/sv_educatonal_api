using DAL;
using DAL.StudentInformation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.AssessmentEntities
{
    public class HomeAssessmentFeedBack : CommonEntity
    {
        [Key]
        public Guid HomeAssessmentFeedBackId { get; set; }
        public string Content { get; set; }
        public string AttachmentUrls { get; set; }
        public decimal Mark { get; set; }
        public Guid? HomeAssessmentId { get; set; }
        [ForeignKey("HomeAssessmentId")]
        public HomeAssessment HomeAssessment { get; set; }
        public Guid? StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact StudentContact { get; set; }
        public int Status { get; set; }
        public bool Included { get; set; }
        public string Comment { get; set; }
        public decimal IncludedScore { get; set; }
    }
}
