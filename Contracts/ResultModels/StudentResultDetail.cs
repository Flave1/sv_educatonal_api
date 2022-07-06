﻿using DAL.StudentInformation;
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
        public StudentResult(ICollection<StudentContact> s, string regNoFormat, Guid sessionClassId)
        {
            if (s.Any())
                PublishResult = s.Select(x => new StudentResultDetail(x, regNoFormat, sessionClassId)).ToList();
        }
        public StudentResult(StudentContact student, string regNoFormat, Guid sessionClassId, Guid termId)
        {
            if (student != null)
            {
                var s = new StudentResultDetail();
                s.StudentName = student.User?.FirstName + " " + student.User?.LastName;
                s.RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
                s.Position = "";
                s.TotalSubjects = student.SessionClass.ClassScoreEntries.Select(d => d.ScoreEntries).Count();

                s.TotalExamScore = student.ScoreEntries.Where(d => d.ClassScoreEntry.SessionClass.SessionClassId == sessionClassId && d.SessionTermId == termId).Sum(d => d.ExamScore);
                s.TotalAssessmentScore = student.ScoreEntries.Where(d => d.ClassScoreEntry.SessionClass.SessionClassId == sessionClassId && d.SessionTermId == termId).Sum(d => d.AssessmentScore);

                s.AverageScore = s.TotalExamScore + s.TotalAssessmentScore / s.TotalSubjects;

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
        public double AverageScore { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalExamScore { get; set; }
        public int TotalAssessmentScore { get; set; }
        public string Status { get; set; }
        public StudentResultDetail() { }
        public StudentResultDetail(StudentContact student, string regNoFormat, Guid sessionClassId)
        {
            StudentName = student.User?.FirstName + " " + student.User?.LastName;
            StudentContactId = student.StudentContactId.ToString();
            RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
            Position = "1";
            TotalSubjects = student.SessionClass.ClassScoreEntries.Select(d => d.ScoreEntries.Select(d => d.IsOffered == true)).Count();

            TotalExamScore = student.ScoreEntries.Where(d => d.ClassScoreEntry.SessionClass.SessionClassId == sessionClassId).Sum(d => d.ExamScore);
            TotalAssessmentScore = student.ScoreEntries.Where(d => d.ClassScoreEntry.SessionClass.SessionClassId == sessionClassId).Sum(d => d.AssessmentScore);

            AverageScore = TotalExamScore + TotalAssessmentScore / TotalSubjects;

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
}