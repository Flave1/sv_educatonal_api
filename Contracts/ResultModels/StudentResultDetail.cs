using DAL.StudentInformation;
using SMP.DAL.Models.GradeEntities;
using SMP.DAL.Models.ResultModels;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMP.Contracts.ResultModels
{
    public class StudentResult
    {
        public bool IsPublished { get; set; }
        public List<StudentResultDetail> PublishResult { get; set; } = new List<StudentResultDetail>();
        public StudentResult() { }
        public StudentResult(IGrouping<Guid, ScoreEntry> entries, string regNoFormat, Guid sessionClassId, Guid termId)
        {
            if (entries.Any())
            {
                //IsPublished = entries.FirstOrDefault().ClassScoreEntry?.SessionClass?.PublishStatus?.IsPublished ?? false;
                PublishResult = entries.Select(x => new StudentResultDetail
                {
                    StudentName = entries.FirstOrDefault()?.StudentContact.User?.FirstName + " " + entries.FirstOrDefault()?.StudentContact.User?.LastName,
                    RegistrationNumber = regNoFormat.Replace("%VALUE%", entries.FirstOrDefault()?.StudentContact?.RegistrationNumber),
                    Position = "",
                    TotalSubjects = entries.Count(),
                    TotalExamScore = entries.Sum(d => d.ExamScore),
                    TotalAssessmentScore = entries.Sum(d => d.AssessmentScore),
                    AverageScore = Math.Round(Convert.ToDecimal(entries.Sum(d => d.ExamScore) + entries.Sum(d => d.AssessmentScore)) / entries.Count(), 2),
                    Status = entries.FirstOrDefault().ClassScoreEntry.SessionClass.PassMark >
                    Math.Round(Convert.ToDecimal(entries.Sum(d => d.ExamScore) + entries.Sum(d => d.AssessmentScore)) / entries.Count(), 2) ? "FAILED" : "PASSED"
                }).ToList();
            }
        }
        public StudentResult(ICollection<StudentContact> s, string regNoFormat, Guid sessionClassId, Guid termId)
        {
            if (s.Any())
            {
                //IsPublished = s.FirstOrDefault()?.SessionClass?.PublishStatus?.IsPublished ?? false;
                PublishResult = s.Select(x => new StudentResultDetail(x, regNoFormat, sessionClassId, termId)).ToList();
            }
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
                //IsPublished = student.SessionClass?.PublishStatus?.IsPublished ?? false;
                SessionClassName = student.SessionClass.Class.Name;
                if (student.ScoreEntries.Any())
                {
                    StudentSubjectEntries = student.ScoreEntries.Where(x => x.SessionTermId == termId).Select(e => new StudentSubjectEntries(e, e.ClassScoreEntry.SessionClass.Class.GradeLevel)).ToList();
                }
            }
            
        }

        public StudentCoreEntry(StudentContact student, string regNoFormat)
        {
            StudentName = student.User?.FirstName + " " + student.User?.LastName;
            StudentContactId = student.StudentContactId.ToString();
            RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
            //SessionClassName = student.SessionClass.Class.Name;
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
        public bool IsPreview { get; set; } = true;
        public bool IsPrint { get; set; } = true;
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
            studentContactId = student.StudentContactId;
            IsPrint = false;
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
                term = studentsSubjects?.FirstOrDefault()?.SessionTerm?.TermName ??"";
                noOfStudents = student.SessionClass.Students.Where(d => d.EnrollmentStatus == 1).Count();
                //isPublished = student.SessionClass?.PublishStatus?.IsPublished ?? false;
                studentSubjectEntries = studentsSubjects.Select(e => new StudentSubjectEntry(e, student.SessionClass.Class.GradeLevel, sessionClassId)).ToList();
                gradeSetting = student.SessionClass.Class.GradeLevel.Grades.Select(x => new GradeSetting(x)).ToList();
            }
            cognitiveBehaviour = new List<CognitiveBehaviour>
            {
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"},
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"},
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"},
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"},
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"}
            };
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

    public class StudentResultRecord
    {
        public int AssessmentScore { get; set; }
        public int ExamScore { get; set; }
        public decimal TotalScore { get; set; }
        public decimal AverageScore { get; set; }
        public bool ShouldPromoteStudent { get; set; }
        public int PassedStudents { get; set; } = 0;
        public int FailedStudents { get; set; } = 0;
        public string StudentContactId { get; set; }
        public StudentResultRecord() { }
        public StudentResultRecord(StudentContact st, Guid termId)
        {
            var studentScoreEntries = st.ScoreEntries.Where(x => x.IsOffered && x.SessionTermId == termId);
            AssessmentScore = studentScoreEntries.Sum(d => d.AssessmentScore);
            ExamScore = studentScoreEntries.Sum(d => d.ExamScore);
            TotalScore = AssessmentScore + ExamScore;
            var totalSubjects = studentScoreEntries.Count();
            AverageScore = Math.Round(totalSubjects > 0 ? TotalScore / totalSubjects : 0, 2);
            ShouldPromoteStudent = AverageScore > st.SessionClass.PassMark;
            StudentContactId = st.StudentContactId.ToString();
        }

        public StudentResultRecord(ICollection<StudentContact> sts, Guid termId)
        {
            foreach(var st in sts)
            {
                var studentScoreEntries = st.ScoreEntries.Where(x => x.IsOffered && x.SessionTermId == termId);
                AssessmentScore = studentScoreEntries.Sum(d => d.AssessmentScore);
                ExamScore = studentScoreEntries.Sum(d => d.ExamScore);
                TotalScore = AssessmentScore + ExamScore;
                var totalSubjects = studentScoreEntries.Count();
                AverageScore = Math.Round(totalSubjects > 0 ? TotalScore / totalSubjects : 0, 2);
                ShouldPromoteStudent = AverageScore > st.SessionClass.PassMark;
                PassedStudents = ShouldPromoteStudent ? PassedStudents + 1 : PassedStudents + 0;
                FailedStudents = !ShouldPromoteStudent ? FailedStudents + 1 : FailedStudents + 0;
            }
            

        }
    }

    public class PrintResult
    {
        public bool IsPreview { get; set; } = true;
        public bool IsPrint { get; set; } = true;
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
        public int totalAssessmentScore { get; set; }
        public decimal average { get; set; }
        public bool isPublished { get; set; }
        public List<StudentSubjectEntry> studentSubjectEntries { get; set; }
        public List<GradeSetting> gradeSetting { get; set; }
        public List<CognitiveBehaviour> cognitiveBehaviour { get; set; }
        public PrintResult(ICollection<ScoreEntry> ScoreEntries, string regNoFormat, SessionTerm Term, Guid studentId)
        {
            var clas = ScoreEntries.FirstOrDefault().ClassScoreEntry.SessionClass;
            var std = ScoreEntries.FirstOrDefault().StudentContact;
            totalExamScore = ScoreEntries.Sum(d => d.ExamScore);
            totalAssessmentScore = ScoreEntries.Sum(d => d.AssessmentScore);
            total = totalExamScore + totalAssessmentScore;
            studentContactId = ScoreEntries.FirstOrDefault().StudentContactId;
            IsPrint = false;
            average = Math.Round(totalSubjects > 0 ? total / totalSubjects : 0, 2);
            if (studentId == studentContactId)
            {
                term = Term.TermName;
                totalSubjects = ScoreEntries.Count();
                remark = ScoreEntries.FirstOrDefault().ClassScoreEntry.SessionClass.PassMark > average ? "FAILED" : "PASSED";
                position = "1";
                registrationNumber = regNoFormat.Replace("%VALUE%", std.RegistrationNumber);
                session = clas.Session.StartDate + " / " + clas.Session.StartDate;
                studentName = std.User?.FirstName + " " + std.User?.LastName;
                studentContactId = std.StudentContactId;
                sessionClassName = clas.Class.Name;
                //isPublished = clas.PublishStatus?.IsPublished ?? false;
                studentSubjectEntries = ScoreEntries.Select(e => new StudentSubjectEntry(e, clas.Class.GradeLevel, clas.SessionClassId)).ToList();
                gradeSetting = clas.Class.GradeLevel.Grades.Select(x => new GradeSetting(x)).ToList();
            }
            cognitiveBehaviour = new List<CognitiveBehaviour>
            {
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"},
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"},
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"},
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"},
                new CognitiveBehaviour{ behaviour = "Play", remark = "Play"}
            };
        }
    }

}
