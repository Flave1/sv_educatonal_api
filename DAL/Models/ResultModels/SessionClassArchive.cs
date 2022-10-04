using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.ResultModels
{
    public class SessionClassArchive : CommonEntity
    {
        [Key]
        public Guid SessionClassArchiveId { get; set; }
        public Guid? SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public Guid? SessionTermId { get; set; }
        [ForeignKey("SessionTermId")]
        public SessionTerm SessionTerm { get; set; }
        public Guid? StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact StudentContact { get; set; }
        public bool HasPrintedResult { get; set; }
        public bool IsPublished { get; set; }
        public bool IsPromoted { get; set; }
    }
}
