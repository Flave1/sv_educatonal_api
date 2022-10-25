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
    public class CandidateAnswer: CommonEntity
    {
        [Key]
        public Guid AnswerId { get; set; }
        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
        public Guid CandidateId { get; set; }
        [ForeignKey("CandidateId")]
        public Candidate Candidate { get; set; }
        public string Answers { get; set; }
        public Guid ClientId { get; set; }
    }
}
