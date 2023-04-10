using SMP.DAL.Models.SessionEntities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.Contracts.Session
{
    public class SessionTermDto
    {
        public Guid SessionTermId { get; set; }
        public string TermName { get; set; }
        public bool IsActive { get; set; }
        public Guid SessionId { get; set; }
        //public virtual SessionDto Session { get; set; }
        public SessionTermDto(SessionTerm db)
        {
            SessionTermId = db.SessionTermId;
            TermName = db.TermName;
            IsActive = db.IsActive;
            SessionId = db.SessionId;
        }
    }

    public class SessionDto
    {
        public Guid SessionId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get; set; }
        public Guid? HeadTeacherId { get; set; }
    }
    
}
