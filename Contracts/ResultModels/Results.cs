using DAL.ClassEntities;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
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

    public class GetClassScoreEntry
    {
        public string SessionClassName { get; set; }
        public string SessionClassId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int AssessmentScore { get; set; }
        public int ExamsScore { get; set; }
        public string SubjectTeacher { get; set; }
        public List<ScoreEntrySheet> ClassScoreEntries { get; set; } = new List<ScoreEntrySheet>();
        public GetClassScoreEntry(ClassScoreEntry db, string regNoFormat)
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
                ClassScoreEntries = db.ScoreEntries.Select(d => new ScoreEntrySheet
                {
                    AssessmentScore = d.AssessmentScore,
                    ExamsScore = d.ExamScore,
                    RegistrationNumber = regNoFormat.Replace("%VALUE%", d.StudentContact.RegistrationNumber),
                    ScoreEntryId = d.ScoreEntryId.ToString(),
                    StudentName = d.StudentContact.User.FirstName + " " + d.StudentContact.User.LastName,
                }).ToList();
            }
        }
    }

    public class ScoreEntrySheet
        {
            public string ScoreEntryId { get; set; }
            public string StudentName { get; set; }
            public string RegistrationNumber { get; set; }
            public int AssessmentScore { get; set; }
            public int ExamsScore { get; set; }
        }
    }