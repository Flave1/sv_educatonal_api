using DAL.ClassEntities;
using DAL.TeachersInfor;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.SessionEntities
{
    public class Session : CommonEntity
    {
        [Key]
        public Guid SessionId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get; set; }
        public Guid? HeadTeacherId { get; set; }
        [ForeignKey("HeadTeacherId")]
        public Teacher HeadTeacher { get; set; }
        public virtual ICollection<SessionTerm> Terms { get; set; }
        public virtual ICollection<SessionClass> SessionClass { get; set; }
    }
}
