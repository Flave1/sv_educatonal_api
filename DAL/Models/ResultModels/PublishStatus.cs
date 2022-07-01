using DAL;
using DAL.ClassEntities;
using SMP.DAL.Models.SessionEntities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.ResultModels
{
    public class PublishStatus: CommonEntity
    {
        [Key]
        public Guid PublishStatusId { get; set; }
        public bool IsPublished { get; set; }
        public Guid SessionTermId { get; set; }
        [ForeignKey("SessionTermId")]
        public SessionTerm SessionTerm { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
    }
}
