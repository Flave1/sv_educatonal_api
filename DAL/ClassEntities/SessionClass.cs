using DAL.SessionEntities;
using DAL.StudentInformation;
using DAL.TeachersInfor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.ClassEntities
{
    public class SessionClass : CommonEntity
    {
        [Key]
        public Guid SessionClassId { get; set; }
        public Guid SessionId { get; set; }
        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }
        public Guid ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual ClassLookup Class { get; set; }
        public Guid? FormTeacherId { get; set; }
        [ForeignKey("FormTeacherId")]
        public virtual Teacher Teacher { get; set; }
        public Guid? ClassCaptainId { get; set; } 
        public bool InSession { get; set; }
        public ICollection<StudentContact> Students { get; set; }
    }
}
