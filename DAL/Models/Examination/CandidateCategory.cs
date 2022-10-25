using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Examination
{
    public class CandidateCategory: CommonEntity
    {
        [Key]
        public Guid CandidateCategoryId { get; set; }
        public string Name { get; set; }
        public int UserType { get; set; }
        public Guid ClientId { get; set; }

        public ICollection<Candidate> Candidates { get; set; }
    }
}
