using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Examination
{
    public class Examination: CommonEntity
    {
        [Key]
        public Guid ExaminationId { get; set; }
        public Guid? SubjectId { get; set; }
        public string ExamName { get; set; }
        public Guid? ClassId { get; set; }
        public Guid CandidateCategory { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Instruction { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool UseAsExamScore { get; set; }
        public bool UseAsAssessmentScore { get; set; }
        public string AsExamScoreSessionAndTerm { get; set; }
        public string AsAssessmentScoreSessionAndTerm { get; set; }
        public int UserType { get; set; }
        public int Status { get; set; }
        public Guid ClientId { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
