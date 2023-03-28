using DAL.ClassEntities;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.GradeEntities;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SMP.Contracts.ResultModels
{
    public class GetClasses
    {
        public string SessionClass { get; set; }
        public string SessionClassId { get; set; }
        public string ClassId { get; set; }
        public GetClasses(SessionClass db)
        {
            SessionClass = db.Class.Name;
            SessionClassId = db.SessionClassId.ToString();
            ClassId = db.ClassId.ToString();
        }
    }

    public class GetClassSubjects
    {
        public string SessionClassId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public GetClassSubjects(SessionClassSubject db)
        {
            SessionClassId = db.SessionClassId.ToString();
            SubjectId = db.SubjectId.ToString();
            SubjectName = db?.Subject?.Name;
        }
    }

    public class UpdateScore
    {
        [Required]
        public string StudentContactId { get; set; }
        [Required]
        public int Score { get; set; }
        [Required]
        public string SubjectId { get; set; }
        [Required]
        public string ClassScoreEntryId { get; set; }
        [Required]
        public string TermId { get; set; }
    }

    public class UpdateOtherSessionScore
    {
        [Required]
        public string StudentContactId { get; set; }
        [Required]
        public int Score { get; set; }
        [Required]
        public string SubjectId { get; set; }
        [Required]
        public string ClassScoreEntryId { get; set; }
        [Required]
        public string SessionTermId { get; set; }
    }

    public class GetClassScoreEntry
    {
        public string SessionClassName { get; set; }
        public string SessionClassId { get; set; }
        public Guid ClassScoreEntryId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int AssessmentScore { get; set; }
        public int ExamsScore { get; set; }
        public string SubjectTeacher { get; set; }
        public List<ScoreEntrySheet> ClassScoreEntries { get; set; } = new List<ScoreEntrySheet>();
        public GetClassScoreEntry(ClassScoreEntry db, string regNoFormat)
        {
            //var subject = db?.SessionClass?.SessionClassSubjects?.FirstOrDefault(x => x.SubjectId == db.SubjectId);
            SessionClassName = db.SessionClass?.Class?.Name;
            SessionClassId = db.SessionClassId.ToString();
            SubjectId = db.SubjectId.ToString();
            SubjectName = db.Subject.Name;
            ClassScoreEntryId = db.ClassScoreEntryId;
            //AssessmentScore = subject?.AssessmentScore ?? 0;
            //ExamsScore = subject?.ExamScore ?? 0;
            //SubjectTeacher = subject?.SubjectTeacher?.FirstName + " " + subject?.SubjectTeacher?.LastName;

        }
        public GetClassScoreEntry(ClassScoreEntry db, string regNoFormat, Guid term)
        {
            //var subject = db?.SessionClass?.SessionClassSubjects?.FirstOrDefault(x => x.SubjectId == db.SubjectId);
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassId = db.SessionClassId.ToString();
            SubjectId = db.SubjectId.ToString();
            SubjectName = db.Subject.Name;
            ClassScoreEntryId = db.ClassScoreEntryId;
            //AssessmentScore = db.SessionClass.AssessmentScore;
            //ExamsScore = db.SessionClass.ExamScore;
            //SubjectTeacher = subject?.SubjectTeacher?.FirstName + " " + subject?.SubjectTeacher?.LastName;
            if (db.ScoreEntries.Any())
            {

                foreach (var entry in db.ScoreEntries)
                {
                    var scoreEntrySheet = new ScoreEntrySheet();
                    var scoreEntrySheet1 = db.ScoreEntries.FirstOrDefault(f => f.StudentContactId == entry.StudentContactId && f.SessionTermId == term);
                    scoreEntrySheet.AssessmentScore = scoreEntrySheet1?.AssessmentScore ?? 0;
                    scoreEntrySheet.ExamsScore = scoreEntrySheet1?.ExamScore ?? 0;
                    scoreEntrySheet.RegistrationNumber = regNoFormat.Replace("%VALUE%", entry.StudentContact.RegistrationNumber);
                    scoreEntrySheet.StudentContactId = entry.StudentContactId.ToString();
                    scoreEntrySheet.StudentName = entry.StudentContact.FirstName + " " + entry.StudentContact.LastName + " " + entry.StudentContact.MiddleName;
                    scoreEntrySheet.IsOffered = scoreEntrySheet1?.IsOffered ?? false;
                    scoreEntrySheet.IsSaved = scoreEntrySheet1?.IsSaved ?? false;
                    scoreEntrySheet.TotalScore = scoreEntrySheet1?.ExamScore ?? 0 + scoreEntrySheet1?.AssessmentScore ?? 0;
                    ClassScoreEntries.Add(scoreEntrySheet);
                }
            }
        }
    }
  
    public class PreviewClassScoreEntry
    {
        public string SessionClassName { get; set; }
        public string SessionClassId { get; set; }
        public string SubjectId { get; set; }
        public Guid ClassLookupId { get; set; }
        public string SubjectName { get; set; }
        public int AssessmentScore { get; set; }
        public int ExamsScore { get; set; }
        public string SubjectTeacher { get; set; }
        public Guid ClassScoreEntryId { get; set; }
        public List<ScoreEntrySheet> ClassScoreEntries { get; set; } = new List<ScoreEntrySheet>();
        public PreviewClassScoreEntry(ClassScoreEntry db, string regNoFormat)
        {
            SessionClassName = db.SessionClass.Class.Name;
            ClassLookupId = db.SessionClass.ClassId;
            SessionClassId = db.SessionClassId.ToString();
            SubjectId = db.SubjectId.ToString();
            SubjectName = db.Subject.Name;
            AssessmentScore = db.SessionClass.AssessmentScore;
            ExamsScore = db.SessionClass.ExamScore;
            ClassScoreEntryId = db.ClassScoreEntryId;
        }
    }


    public class ScoreEntrySheet
    {
        public string StudentContactId { get; set; }
        public string StudentName { get; set; }
        public string RegistrationNumber { get; set; }
        public int AssessmentScore { get; set; }
        public int ExamsScore { get; set; }
        public int TotalScore { get; set; }
        public bool IsOffered { get; set; }
        public bool IsSaved { get; set; }
        public string Grade { get; set; }
        public string Remark { get; set; }
        public ScoreEntrySheet() { }
        public ScoreEntrySheet(ScoreEntry d, string regNoFormat, GradeGroup level = null)
        {
            AssessmentScore = d.AssessmentScore;
            ExamsScore = d.ExamScore;
            RegistrationNumber = regNoFormat.Replace("%VALUE%", d.StudentContact.RegistrationNumber);
            StudentContactId = d.StudentContactId.ToString();
            StudentName = d.StudentContact.FirstName + " " + d.StudentContact.LastName + " " + d.StudentContact.MiddleName;
            IsOffered = d.IsOffered;
            IsSaved = d.IsSaved;
            TotalScore = ExamsScore + AssessmentScore;
            if (level != null)
            {
                var grade = level.Grades.FirstOrDefault(d => d.LowerLimit <= TotalScore && TotalScore <=  d.UpperLimit);
                if (grade != null)
                {
                    Grade = grade.GradeName;
                    Remark = grade.Remark;
                }
            }
        }
    }

    

}