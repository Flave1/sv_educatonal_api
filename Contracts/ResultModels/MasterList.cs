﻿using DAL.ClassEntities;
using DAL.StudentInformation;
using DAL.SubjectModels;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.ResultModels;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.ResultModels
{
    public class MasterList
    {
        public string Session { get; set; }
        public string SessionClass { get; set; }
        public string FormTeacher { get; set; }
        public string TermName { get; set; }
        public List<MasterListResult> ResultList { get;set; }
  
        public MasterList(SessionClass db, SessionTerm term, string regNoFormat)
        {
            Session = db.Session.StartDate + " / " + db.Session.EndDate;
            SessionClass = db.Class.Name;
            FormTeacher = db.Teacher.User.FirstName + " " + db.Teacher.User.LastName;
            TermName = term.TermName;
            if (db.Students.Any())
            {
                ResultList = db.Students.Select(e => new MasterListResult(e, regNoFormat, term.SessionTermId)).ToList();
            }
            
        }

    }

    public class MasterListResult
    {
        public string StudentName { get; set; }
        public string RegistrationNumber { get; set; }
        public string Position { get; set; }
        public int TotalSubjects { get; set; }
        public decimal TotalScore { get; set; }
        public decimal AverageScore { get; set; }
        public int TotalExamScore { get; set; }
        public int TotalAssessmentScore { get; set; }
        public string Status { get; set; }
        public List<MasterListResultSubjects> Subjects { get; set; }
        public MasterListResult(StudentContact se, string regNoFormat, Guid sessionTermId)
        {
            StudentName = se?.User?.FirstName + " " + se?.User?.LastName + " " + se?.User?.MiddleName;
            RegistrationNumber = regNoFormat?.Replace("%VALUE%", se?.RegistrationNumber);
            Position = "1";
            TotalSubjects = se?.ScoreEntries.Where(d => d.SessionTermId == sessionTermId && d.IsOffered == true)?.Count(d => d.IsOffered == true)??0;

            TotalExamScore = se?.ScoreEntries.Where(d => d.SessionTermId == sessionTermId && d.IsOffered == true)?.Sum(d => d.ExamScore)??0;
            TotalAssessmentScore = se?.ScoreEntries.Where(d => d.SessionTermId == sessionTermId && d.IsOffered == true)?.Sum(d => d.AssessmentScore)??0;

            TotalScore = TotalExamScore + TotalAssessmentScore;

            AverageScore = Math.Round(TotalSubjects > 0 ? TotalScore / TotalSubjects : 0, 2);
            Status = se?.SessionClass?.PassMark > AverageScore ? "FAILED" : "PASSED";

            Subjects = se?.ScoreEntries.Where(d => d.SessionTermId == sessionTermId && d.IsOffered == true).Select(s => s.ClassScoreEntry)
                .Select(d => new MasterListResultSubjects(d.Subject, se.ScoreEntries.Where(d => d.SessionTermId == sessionTermId && d.IsOffered == true).ToList())).ToList();

        }
    }

    public class MasterListResultSubjects
    {
        public string SubjectName { get; set; }
        public int AssessmentScore { get; set; }
        public int ExamScore { get; set; }
        public int Total { get; set; }
        public MasterListResultSubjects(Subject cs, List<ScoreEntry> scores)
        {
            AssessmentScore = scores.FirstOrDefault(e => cs.SubjectId == e.ClassScoreEntry.SubjectId)?.AssessmentScore ?? 0;

            ExamScore = scores.FirstOrDefault(e => cs.SubjectId == e.ClassScoreEntry.SubjectId)?.ExamScore ?? 0;

            Total = AssessmentScore + ExamScore;

            SubjectName = cs.Name;
        }
    }

    public class CumulativeMasterList
    {
        public string Session { get; set; }
        public string SessionClass { get; set; }
        public string FormTeacher { get; set; }
        public List<CumulativeMasterListResult> ResultList { get; set; }

        public CumulativeMasterList(SessionClass db, string regNoFormat)
        {
            Session = db.Session.StartDate + " / " + db.Session.EndDate;
            SessionClass = db.Class.Name;
            FormTeacher = db.Teacher.User.FirstName + " " + db.Teacher.User.LastName;
            if (db.Students.Any())
                ResultList = db.Students.Select(e => new CumulativeMasterListResult(e, regNoFormat)).ToList();

        } 
    }

    public class CumulativeMasterListResult
    {
        public string StudentName { get; set; }
        public string RegistrationNumber { get; set; }
        public string Position { get; set; }
        public int TotalSubjects { get; set; }
        public decimal TotalScore { get; set; }
        public decimal AverageScore { get; set; }
        public int TotalExamScore { get; set; }
        public int TotalAssessmentScore { get; set; }
        public string Status { get; set; }
        public List<CumulativeTermAvgScore> CumulativeTermAvgScore { get; set; }
        public List<CumulativeMasterListResultSubjects> Subjects { get; set; }
        public CumulativeMasterListResult(StudentContact se, string regNoFormat)
        {
            StudentName = se?.User?.FirstName + " " + se?.User?.LastName + " " + se?.User?.MiddleName;
            RegistrationNumber = regNoFormat?.Replace("%VALUE%", se?.RegistrationNumber);
            Position = "1";
            TotalSubjects = se?.ScoreEntries.Where(d => d.IsOffered == true)?.Count(d => d.IsOffered == true) ?? 0;

            TotalExamScore = se?.ScoreEntries.Where(d => d.IsOffered == true)?.Sum(d => d.ExamScore) ?? 0;
            TotalAssessmentScore = se?.ScoreEntries.Where(d => d.IsOffered == true)?.Sum(d => d.AssessmentScore) ?? 0;

            TotalScore = TotalExamScore + TotalAssessmentScore;

            AverageScore = Math.Round(TotalSubjects > 0 ? TotalScore / TotalSubjects : 0, 2);
            Status = se?.SessionClass?.PassMark > AverageScore ? "FAILED" : "PASSED";

            var scores = se.ScoreEntries.Where(d => d.IsOffered == true).ToList();


            var grouped = se?.ScoreEntries.Where(d => d.IsOffered == true)
                .GroupBy(u => u.SessionTermId)
                .Select(grp => grp.ToList())
                .ToList();

            CumulativeTermAvgScore = grouped.Select(d => new CumulativeTermAvgScore(d)).ToList();
            //CumulativeTermAvgScore = se?.ScoreEntries.GroupBy(d => new {

            //    SessionTermid = d.SessionTermId,
            //    TermName = d.t.Where(a => a.IsOffered == true).FirstOrDefault()?.SessionTerm?.TermName

            //}).Select(x => new CumulativeTermAvgScore(x)).ToList();


            Subjects = se?.ScoreEntries.Where(d => d.IsOffered == true).GroupBy(x => x.ClassScoreEntryId).Select(d => new CumulativeMasterListResultSubjects(d)).ToList();

        }
    }
    public class CumulativeMasterListResultSubjects
    {
        public string SubjectName { get; set; }
        public int AssessmentScore { get; set; }
        public int ExamScore { get; set; }
        public int Total { get; set; }
        public List<CumulativeTermAvgScore> CumulativeTermAvgScore { get; set; }


        public CumulativeMasterListResultSubjects(IGrouping<Guid, ScoreEntry> d)
        {
            AssessmentScore = d.Sum(s => s.AssessmentScore);
            ExamScore = d.Sum(s => s.ExamScore);
            Total = AssessmentScore + ExamScore;
            CumulativeTermAvgScore = d.Select(x => new CumulativeTermAvgScore(x)).ToList();

            SubjectName = d.FirstOrDefault().ClassScoreEntry.Subject.Name;
        }
    }

    public class CumulativeTermAvgScore
    {
        public string TermName { get; set; }
        public string TermId { get; set; }
        public int TermCumalativeScore { get; set; }
        public CumulativeTermAvgScore() { }
        public CumulativeTermAvgScore(ScoreEntry score)
        {
            TermName = score?.SessionTerm?.TermName;
            TermId = score?.SessionTerm?.SessionTermId.ToString();
            TermCumalativeScore = score.ExamScore + score.AssessmentScore;
        }
        public CumulativeTermAvgScore(IGrouping<string, ScoreEntry> x)
        {
            TermName = x.Where(d => d.IsOffered == true).FirstOrDefault()?.SessionTerm?.TermName;
            TermId = x.Where(d => d.IsOffered == true).FirstOrDefault()?.SessionTerm?.SessionTermId.ToString();
            var exam = x?.Where(d => d.IsOffered == true).Sum(r => r.ExamScore) ?? 0;
            var ass = x?.Where(d => d.IsOffered == true).Sum(r => r.AssessmentScore) ?? 0;
            var count =  x.Where(d => d.IsOffered == true).Count();
            TermCumalativeScore =  exam + ass / count;
        }
        public CumulativeTermAvgScore(List<ScoreEntry> x)
        {
            TermName = x.FirstOrDefault()?.SessionTerm?.TermName;
            TermId = x.FirstOrDefault()?.SessionTerm?.SessionTermId.ToString();
            var exam = x?.Sum(r => r.ExamScore) ?? 0;
            var ass = x?.Sum(r => r.AssessmentScore) ?? 0;
            var count = x?.Count();
            TermCumalativeScore = exam + ass / count ?? 0;
        }

    }

}