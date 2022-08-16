using DAL;
using DAL.ClassEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.ClassEntities
{
    public class SessionClassGroup: CommonEntity
    {
        [Key]
        public Guid SessionClassGroupId { get; set; }
        public string GroupName { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public Guid SessionClassSubjectId { get; set; }
        [ForeignKey("SessionClassSubjectId")]
        public SessionClassSubject SessionClassSubject { get; set; }
        public string ListOfStudentContactIds { get; set; }
    }
}
