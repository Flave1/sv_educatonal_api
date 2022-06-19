using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using SMP.DAL.Models.ClassEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
