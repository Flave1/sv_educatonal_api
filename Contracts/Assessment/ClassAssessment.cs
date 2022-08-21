using DAL.StudentInformation;
using DAL.SubjectModels;
using SMP.DAL.Models.AssessmentEntities;
using SMP.DAL.Models.ClassEntities;
using System;

namespace SMP.Contracts.Assessment
{
    public class CreateClassAssessment
    {
        public string SessionClassSubjectId { get; set; }
    }

    public class GetClassAssessmentRequest
    {
        public string ClassAssessmentId { get; set; }
        public string SessionClassSubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SessionClassId { get; set; }
        public string SessionClassName { get; set; }
        public decimal AssessmentScore { get; set; }
        public string Title { get; set; }
        public ClassAssessmentStudents[] Students;
        public GetClassAssessmentRequest() { }
        public GetClassAssessmentRequest(ClassAssessment db)
        {
            AssessmentScore = db.AssessmentScore;
            ClassAssessmentId = db.ClassAssessmentId.ToString();
            SessionClassId = db.SessionClassId.ToString();
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SubjectName = db.SessionClassSubject.Subject.Name;
            Title = db.Description;
        }
        public GetClassAssessmentRequest(ClassAssessment db, SessionClassSubject subject)
        {
            AssessmentScore = db.AssessmentScore;
            ClassAssessmentId = db.ClassAssessmentId.ToString();
            SessionClassId = db.SessionClassId.ToString();
            SessionClassName = subject.SessionClass.Class.Name;
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SubjectName = subject.Subject.Name;
            Title = db.Description;
        }
    }

    public class ClassAssessmentStudents
    {
        public string StudentName { get; set; }
        public string StudentContactId { get; set; }
        public decimal Score { get; set; }
        public Guid[] GroupIds { get; set; }
        public ClassAssessmentStudents() { }
    }

    public class UpdatetudentAssessmentScore
    {
        public string SessionClassSubjectId { get; set; }
        public string ClassAssessmentId { get; set; }
        public string StudentContactId { get; set; }
        public decimal Score { get; set; }
    }
}
