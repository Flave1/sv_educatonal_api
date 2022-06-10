using DAL.SessionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.SessionEntities
{
    public class SessionTerm
    {
        [Key]
        public Guid SessionTermId { get; set; }
        public string TermName { get; set; }
        public bool IsActive { get; set; }
        public Guid SessionId { get; set; }
        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }
    }
}
