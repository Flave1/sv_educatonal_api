using SMP.DAL.Models.ResultModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.SubjectModels
{
    public class Subject : CommonEntity
    {
        [Key]
        public Guid SubjectId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ScoreEntry ScoreEntry { get; set; }
    }
}
