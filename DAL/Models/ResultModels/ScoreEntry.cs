using DAL;
using DAL.StudentInformation;
using SMP.DAL.Models.SessionEntities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.ResultModels
{
    public class ScoreEntry: CommonEntity
    {
        [Key]
        public Guid ScoreEntryId { get; set; }
        public int AssessmentScore { get; set; }
        public int ExamScore { get; set; }
        public bool IsOffered { get; set; }
        public bool IsSaved { get; set; }
        public Guid StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact StudentContact { get; set; }
        public Guid ClassScoreEntryId { get; set; }
        [ForeignKey("ClassScoreEntryId")]
        public ClassScoreEntry ClassScoreEntry { get; set; }
        public Guid? SessionTermId { get; set; }
        [ForeignKey("SessionTermId")]
        public SessionTerm SessionTerm { get; set; }

    }
}
