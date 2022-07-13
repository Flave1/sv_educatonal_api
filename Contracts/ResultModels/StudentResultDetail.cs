using DAL.StudentInformation;
using SMP.DAL.Models.GradeEntities;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMP.Contracts.ResultModels
{
    public class StudentResult
    {
        public bool IsPublished { get; set; }
        public List<StudentResultDetail> PublishResult { get; set; } = new List<StudentResultDetail>();
        public StudentResult(ICollection<StudentContact> s, string regNoFormat, Guid sessionClassId, Guid termId)
        {
            if (s.Any())
                PublishResult = s.Select(x => new StudentResultDetail(x, regNoFormat, sessionClassId, termId)).ToList();
        }
        public StudentResult(StudentContact student, string regNoFormat, Guid sessionClassId, Guid termId)
        {
            if (student != null)
            {
                var s = new StudentResultDetail();
                s.StudentName = student.User?.FirstName + " " + student.User?.LastName;
                s.RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
                s.Position = "";
                var studentsSubjects = student.ScoreEntries.Where(d => d.ClassScoreEntry.SessionClassId == sessionClassId && d.SessionTermId == termId);
                s.TotalSubjects = studentsSubjects.Count();
                s.TotalExamScore = studentsSubjects.Sum(d => d.ExamScore);
                s.TotalAssessmentScore = studentsSubjects.Sum(d => d.AssessmentScore);
                decimal total = s.TotalExamScore + s.TotalAssessmentScore;
                s.AverageScore = Math.Round(total / s.TotalSubjects, 2);

                s.Status = student.SessionClass.PassMark > s.AverageScore ? "FAILED" : "PASSED";
                PublishResult.Add(s);
            }
        }
    }


    public class StudentResultDetail
    {
        public string StudentName { get; set; }
        public string StudentContactId { get; set; }
        public string RegistrationNumber { get; set; }
        public string Position { get; set; }
        public decimal AverageScore { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalExamScore { get; set; }
        public int TotalAssessmentScore { get; set; }
        public string Status { get; set; }
        public StudentResultDetail() { }
        public StudentResultDetail(StudentContact student, string regNoFormat, Guid sessionClassId, Guid termId)
        {
            StudentName = student.User?.FirstName + " " + student.User?.LastName;
            StudentContactId = student.StudentContactId.ToString();
            RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
            Position = "1";
            var studentsSubjects = student.ScoreEntries.Where(d => d.ClassScoreEntry.SessionClassId == sessionClassId && d.SessionTermId == termId);
            TotalSubjects = studentsSubjects.Count();
            TotalExamScore = studentsSubjects.Sum(d => d.ExamScore);
            TotalAssessmentScore = studentsSubjects.Sum(d => d.AssessmentScore);
            decimal total = TotalExamScore + TotalAssessmentScore;
            AverageScore = Math.Round(TotalSubjects > 0 ? total / TotalSubjects : 0, 2);

            Status = student.SessionClass.PassMark > AverageScore ? "FAILED" : "PASSED";
        }
    }

    public class PublishResultRequest
    {
        public Guid SessionClassId { get; set; }
        public Guid SessionTermId { get; set; }
        public bool Publish { get; set; }
    }

    public class StudentCoreEntry
    {
        public string StudentName { get; set; }
        public string StudentContactId { get; set; }
        public string RegistrationNumber { get; set; }
        public string SessionClassName { get; set; }
        public bool IsPublished { get; set; }
        public List<StudentSubjectEntries> StudentSubjectEntries { get; set; }
        public StudentCoreEntry() { }
        public StudentCoreEntry(StudentContact student, string regNoFormat, Guid termId)
        {
            if(student != null)
            {
                StudentName = student.User?.FirstName + " " + student.User?.LastName;
                StudentContactId = student.StudentContactId.ToString();
                RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
                IsPublished = student.SessionClass?.PublishStatus?.IsPublished ?? false;
                SessionClassName = student.SessionClass.Class.Name;
                if (student.ScoreEntries.Any())
                {
                    StudentSubjectEntries = student.ScoreEntries.Where(x => x.SessionTermId == termId).Select(e => new StudentSubjectEntries(e, e.ClassScoreEntry.SessionClass.Class.GradeLevel)).ToList();
                }
            }
            
        }
    }

    public class StudentSubjectEntries
    {
        public string SessionClassId { get; set; }
        public string SubjectId { get; set; }
        public string SibjectName { get; set; }
        public int ExamScore { get; set; }
        public string ClassScoreEntryId { get; set; }
        public int AssessmentScore { get; set; }
        public int TotalScore { get; set; }
        public string Grade { get; set; }
        public string Remark { get; set; }
        public StudentSubjectEntries(ScoreEntry e, GradeGroup level)
        {
            SessionClassId = e.ClassScoreEntry.SessionClassId.ToString();
            ClassScoreEntryId = e.ClassScoreEntry.ClassScoreEntryId.ToString();
            SubjectId = e.ClassScoreEntry.SubjectId.ToString();
            SibjectName = e.ClassScoreEntry.Subject.Name;
            ExamScore = e.ExamScore;
            AssessmentScore = e.AssessmentScore;
            TotalScore = ExamScore + AssessmentScore;
            if (level != null)
            {
                var grade = level.Grades.FirstOrDefault(d => d.LowerLimit <= TotalScore && TotalScore <= d.UpperLimit);
                if (grade != null)
                {
                    Grade = grade.GradeName;
                    Remark = grade.Remark;
                }
            }
        }
    }

    public class CognitiveBehaviour
    {
        public string behaviour { get; set; }
        public string remark { get; set; }
    }

    public class GradeSetting
    {
        public string limit { get; set; }
        public string grade { get; set; }
        public GradeSetting(Grade gradeSetting)
        {
            grade = gradeSetting.GradeName;
            limit = gradeSetting.UpperLimit + "-" + gradeSetting.LowerLimit;
        }
    }

    public class PreviewResult
    {
        public string studentName { get; set; }
        public Guid studentContactId { get; set; }
        public string registrationNumber { get; set; }
        public string sessionClassName { get; set; }
        public string session { get; set; }
        public string term { get; set; }
        public string position { get; set; }
        public string remark { get; set; }
        public int noOfStudents { get; set; }
        public decimal total { get; set; }
        public int totalScores { get; set; }
        public int totalSubjects { get; set; }
        public int totalExamScore { get; set; }
        public int totalAssessmentScore { get;set; }
        public decimal average { get; set; }
        public bool isPublished { get; set; }
        public List<StudentSubjectEntry> studentSubjectEntries { get; set; }
        public List<GradeSetting> gradeSetting { get; set; }
        public List<CognitiveBehaviour> cognitiveBehaviour { get; set; }
        public PreviewResult(StudentContact student, string regNoFormat, Guid sessionClassId, Guid termId, Guid StudentContactId)
        {
            var studentsSubjects = student.ScoreEntries.Where(d => d.ClassScoreEntry.SessionClassId == sessionClassId && d.SessionTermId == termId);
            totalExamScore = studentsSubjects.Sum(d => d.ExamScore);
            totalAssessmentScore = studentsSubjects.Sum(d => d.AssessmentScore);
            total = totalExamScore + totalAssessmentScore;
            average = Math.Round(totalSubjects > 0 ? total / totalSubjects : 0, 2);
            if (StudentContactId == studentContactId)
            {
                totalSubjects = studentsSubjects.Count();
                remark = student.SessionClass.PassMark > average ? "FAILED" : "PASSED";
                position = "1";
                registrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
                session = student.SessionClass.Session.StartDate + " / " + student.SessionClass.Session.StartDate;
                studentName = student.User?.FirstName + " " + student.User?.LastName;
                studentContactId = student.StudentContactId;
                sessionClassName = student.SessionClass.Class.Name;
                term = studentsSubjects.FirstOrDefault().SessionTerm.TermName;
                noOfStudents = student.SessionClass.Students.Where(d => d.EnrollmentStatus == 1).Count();
                isPublished = student.SessionClass.PublishStatus.IsPublished;
                studentSubjectEntries = studentsSubjects.Select(e => new StudentSubjectEntry(e, student.SessionClass.Class.GradeLevel, sessionClassId)).ToList();
                gradeSetting = student.SessionClass.Class.GradeLevel.Grades.Select(x => new GradeSetting(x)).ToList();
            }
        }
    }

    public class StudentSubjectEntry
    {
        public string sessionClassId { get; set; }
        public string subjectId { get; set; }
        public string sibjectName { get; set; }
        public int examScore { get; set; }
        public string classScoreEntryId { get; set; }
        public int assessmentScore { get; set; }
        public int totalScore { get; set; }
        public string grade { get; set; }
        public string remark { get; set; }
        public StudentSubjectEntry() { }
        public StudentSubjectEntry(ScoreEntry d, GradeGroup level, Guid SessionClassId)
        {
            sessionClassId = SessionClassId.ToString();
            subjectId = d.ClassScoreEntry.SubjectId.ToString();
            sibjectName = d.ClassScoreEntry.Subject.Name;
            examScore = d.ExamScore;
            classScoreEntryId = d.ClassScoreEntryId.ToString();
            assessmentScore = d.AssessmentScore;
            totalScore = examScore + assessmentScore;
            var gradeSetup = level.Grades.FirstOrDefault(d => d.LowerLimit <= totalScore && totalScore <= d.UpperLimit);
            if (gradeSetup != null)
            {
                grade = gradeSetup.GradeName;
                remark = gradeSetup.Remark;
            }
        }
    }
}
