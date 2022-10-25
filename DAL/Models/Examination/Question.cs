using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Examination
{
    public class Question: CommonEntity
    {
        [Key]
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public Guid ExaminationId { get; set; }
        [ForeignKey("ExaminationId")]
        public Examination Examination { get; set; }
        public int Mark { get; set; }
        public string Options { get; set; }
        public string Answers { get; set; }
        public int QuestionType { get; set; }
        public int UserType { get; set; }
        public Guid ClientId { get; set; }
        public virtual CandidateAnswer CandidateAnswer { get; set; }
    }
}
