using DAL.ClassEntities;
using DAL.SubjectModels;
using SMP.DAL.Models.ClassEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
