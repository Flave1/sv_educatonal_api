using System;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.ResultModels
{
    public class ScoreEntryHistory
    {
        [Key]
        public Guid ScoreEntryHistoryId { get; set; }
        public string Score { get; set; }
        public string StudentId { get; set; }
        public string SessionTermId { get; set; }
        public string SessionClassId { get; set; }
        public string Subjectid { get; set; }
    }
}
