using DAL.ClassEntities;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.GradeEntities;
using SMP.DAL.Models.ResultModels;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.ResultModels
{
    public class GetClasses
    {
        public string SessionClass { get; set; }
        public string SessionClassId { get; set; }
        public GetClasses(SessionClass db)
        {
            SessionClass = db.Class.Name;
            SessionClassId = db.SessionClassId.ToString();
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
            SubjectName = db.Subject.Name;
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
        public string ClassScoreEntryId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int AssessmentScore { get; set; }
        public int ExamsScore { get; set; }
        public string SubjectTeacher { get; set; }
        public List<ScoreEntrySheet> ClassScoreEntries { get; set; } = new List<ScoreEntrySheet>();
        public GetClassScoreEntry(ClassScoreEntry db, string regNoFormat, SessionTerm term)
        {
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassId = db.SessionClassId.ToString();
            SubjectId = db.SubjectId.ToString();
            SubjectName = db.Subject.Name;
            ClassScoreEntryId = db.ClassScoreEntryId.ToString();
            AssessmentScore = db.SessionClass.AssessmentScore;
            ExamsScore = db.SessionClass.ExamScore;
            SubjectTeacher = db.SessionClass.Teacher.User.FirstName + " " + db.SessionClass.Teacher.User.LastName;
            if (db.SessionClass.Students != null && db.SessionClass.Students.Any())
            {
                
                foreach (var student in db.SessionClass.Students)
                {
                    var scoreEntrySheet = new ScoreEntrySheet();
                    var scoreEntrySheet1 = db.ScoreEntries.FirstOrDefault(f => f.StudentContactId == student.StudentContactId && f.SessionTermId == term.SessionTermId);
                    scoreEntrySheet.AssessmentScore = scoreEntrySheet1?.AssessmentScore ?? 0;
                    scoreEntrySheet.ExamsScore = scoreEntrySheet1?.ExamScore ?? 0;
                    scoreEntrySheet.RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
                    scoreEntrySheet.StudentContactId = student.StudentContactId.ToString();
                    scoreEntrySheet.StudentName = student.User.FirstName + " " + student.User.LastName + " " + student.User.MiddleName;
                    scoreEntrySheet.IsOffered = scoreEntrySheet1?.IsOffered ?? false;
                    scoreEntrySheet.IsSaved = scoreEntrySheet1?.IsSaved ?? false;
                    scoreEntrySheet.TotalScore = scoreEntrySheet1?.ExamScore??0 + scoreEntrySheet1?.AssessmentScore??0;
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
        public string SubjectName { get; set; }
        public int AssessmentScore { get; set; }
        public int ExamsScore { get; set; }
        public string SubjectTeacher { get; set; }
        public List<ScoreEntrySheet> ClassScoreEntries { get; set; } = new List<ScoreEntrySheet>();
        public PreviewClassScoreEntry(ClassScoreEntry db, string regNoFormat)
        {
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassId = db.SessionClassId.ToString();
            SubjectId = db.SubjectId.ToString();
            SubjectName = db.Subject.Name;
            AssessmentScore = db.SessionClass.AssessmentScore;
            ExamsScore = db.SessionClass.ExamScore;
            SubjectTeacher = db.SessionClass.Teacher.User.FirstName + " " + db.SessionClass.Teacher.User.LastName;
            if (db.ScoreEntries.Any())
            {
                ClassScoreEntries = db.ScoreEntries.Where(d => (bool)d.SessionTerm?.IsActive == true).Select(d => new ScoreEntrySheet(d, regNoFormat, db.SessionClass.Class.GradeLevel)).ToList();
            }
        }

        public PreviewClassScoreEntry(ClassScoreEntry db, string regNoFormat, Guid sessionTermId)
        {
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassId = db.SessionClassId.ToString();
            SubjectId = db.SubjectId.ToString();
            SubjectName = db.Subject.Name;
            AssessmentScore = db.SessionClass.AssessmentScore;
            ExamsScore = db.SessionClass.ExamScore;
            SubjectTeacher = db.SessionClass.Teacher.User.FirstName + " " + db.SessionClass.Teacher.User.LastName;
            if (db.ScoreEntries.Any())
            {
                ClassScoreEntries = db.ScoreEntries.Where(d => d.SessionTerm.SessionTermId == sessionTermId).Select(d => new ScoreEntrySheet(d, regNoFormat, db.SessionClass.Class.GradeLevel)).ToList();
            }
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
            StudentName = d.StudentContact.User.FirstName + " " + d.StudentContact.User.LastName + " " + d.StudentContact.User.MiddleName;
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