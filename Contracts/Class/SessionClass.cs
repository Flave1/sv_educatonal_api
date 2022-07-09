using DAL.ClassEntities;
using System;
using System.Linq;

namespace Contracts.Class
{
    public class SessionClassCommand
    { 
        public string SessionClassId { get; set; }
        public string SessionId { get; set; }  
        public string ClassId { get; set; }   
        public string FormTeacherId { get; set; }
        //public string ClassCaptainId { get; set; }
        public bool InSession { get; set; }
        public int ExamScore { get; set; }
        public int AssessmentScore { get; set; }
        public int PassMark { get; set; }
        public ClassSubjects[] ClassSubjects { get; set; }
    }

    public class GetSessionClass
    {
        public string SessionClassId { get; set; }
        public string SessionId { get; set; }
        public string Session { get; set; }
        public string ClassId { get; set; }
        public string Class { get; set; }
        public string FormTeacherId { get; set; }
        public string FormTeacher { get; set; }
        public string ClassCaptainId { get; set; }
        public string ClassCaptain { get; set; }
        public bool InSession { get; set; }
        public int ExamScore { get; set; }
        public int AssessmentScore { get; set; }
        public int PassMark { get; set; }
        public int SubjectCount { get;set; }
        public int StudentCount { get; set; }
        public ClassSubjects[] ClassSubjects { get; set; } = new ClassSubjects[0];

        public GetSessionClass(SessionClass sClass)
        {
            InSession = sClass.InSession;
            ClassId = sClass.ClassId.ToString();
            SessionId = sClass.SessionId.ToString();
            SessionClassId = sClass.SessionClassId.ToString();
            FormTeacherId = sClass.FormTeacherId.ToString();
            ClassCaptainId = sClass.ClassCaptainId.ToString();
            Class = sClass.Class.Name;
            ExamScore = sClass.ExamScore;
            AssessmentScore = sClass.AssessmentScore;
            PassMark = sClass.PassMark;
            Session = sClass.Session.StartDate + " / " + sClass.Session.EndDate;
            FormTeacher = sClass.Teacher.User.FirstName + " " + sClass.Teacher.User.LastName;

            if (sClass.SessionClassSubjects != null && sClass.SessionClassSubjects.Any())
            {
                SubjectCount = sClass.SessionClassSubjects.Count();
                ClassSubjects = sClass.SessionClassSubjects.Select(w => new ClassSubjects
                {
                    SubjectId = w.SubjectId.ToString().ToLower(),
                    SubjectTeacherId = w.SubjectTeacherId.ToString().ToLower(),
                    SubjectName = w.Subject.Name,
                    SubjectTeacherName = w.SubjectTeacher.User.FirstName + " " + w.SubjectTeacher.User.LastName,
                    Assessment = w.AssessmentScore,
                    ExamSCore = w.ExamScore
                }).ToArray();
                SubjectCount = sClass.SessionClassSubjects.Count();
            }
            if(sClass.Students != null && sClass.Students.Where(d => d.EnrollmentStatus == 1).Any())
            {
                StudentCount = sClass.Students.Count();
            }
        }
    }

    public class SessionQuery
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class ClassSubjects
    {
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectTeacherId { get; set; }
        public string SubjectTeacherName { get; set; }
        public int ExamSCore { get; set; }
        public int Assessment { get; set; }
    }
}
