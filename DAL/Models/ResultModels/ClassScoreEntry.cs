using DAL.ClassEntities;
using DAL.SubjectModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.ResultModels
{
    public class ClassScoreEntry
    {
        [Key]
        public Guid ClassScoreEntryId { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public Guid SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        public ICollection<ScoreEntry> ScoreEntries { get; set; }
        
    }
}
