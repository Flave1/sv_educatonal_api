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
    public class Candidate: CommonEntity
    {
        [Key]
        public Guid CandidateId { get; set; }
        public string CandidateExamId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PassportPhoto { get; set; }
        public int UserType { get; set; }
        public Guid ClientId { get; set; }
        public Guid CandidateCategoryId { get; set; }
        [ForeignKey("CandidateCategoryId")]
        public CandidateCategory CandidateCategory { get; set; }
        public virtual CandidateAnswer CandidateAnswer { get; set; }

    }
}
