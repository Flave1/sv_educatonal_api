using DAL;
using DAL.SessionEntities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.SessionEntities
{
    public class SessionTerm : CommonEntity
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
