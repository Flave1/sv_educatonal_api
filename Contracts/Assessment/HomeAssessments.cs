using SMP.DAL.Models.AssessmentEntities;
using System;
using System.Linq;
namespace SMP.Contracts.Assessment
{
    public class GetHomeAssessmentRequest
    {
        public string HomeAssessmentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int AssessmentScore { get; set; }
        public string SessionClassId { get; set; }
        public string SessionClassName { get; set; }
        public string SessionClassSubjectId { get; set; }
        public string SessionClassSubjectName { get; set; }
        public string SessionClassGroupId { get; set; }
        public string SessionClassGroupName { get; set; }
        public string SessionTermId { get; set; } 
        public string SessionTermName { get; set; }
        public int NumberOfStudentsSubmitted { get; set; }
        public int NumberOfStudentsNotSubmitted { get; set; }
        public string Status { get; set; }
        public SubmittedAndUnsubmittedStudent[] StudentList { get; set; }
        public GetHomeAssessmentRequest(HomeAssessment db)
        {
            HomeAssessmentId = db.HomeAssessmentId.ToString();
            Title = db.Title;
            Content = db.Content;
            AssessmentScore = db.AssessmentScore;
            SessionClassId = db.SessionClassId.ToString();
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SessionClassSubjectName = db.SessionClassSubject.Subject.Name;
            SessionClassGroupId = db.SessionClassGroupId.ToString();
            SessionClassGroupName = db.SessionClassGroup.GroupName;
            SessionTermId = db.SessionTermId.ToString();
            SessionTermName = db.SessionTerm.TermName;
            NumberOfStudentsSubmitted = db.HomeAssessmentFeedBacks.Count(d => d.Status == 3); //3 of HomeAssessmentStatus;
            NumberOfStudentsNotSubmitted = Convert.ToInt32((NumberOfStudentsSubmitted - db.SessionClassGroup.ListOfStudentContactIds.Split(',').Count()).ToString().TrimStart('-'));
            if (db.Status == 1)
                Status = "Open";
            if (db.Status == 2)
                Status = "Closed";
            if (db.Status == 3)
                Status = "Submitted";
            if (db.Status == 0)
                Status = "Saved";
        }

        public GetHomeAssessmentRequest(HomeAssessment db, bool isSingle)
        {
            HomeAssessmentId = db.HomeAssessmentId.ToString();
            Title = db.Title;
            Content = db.Content;
            AssessmentScore = db.AssessmentScore;
            SessionClassId = db.SessionClassId.ToString();
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SessionClassSubjectName = db.SessionClassSubject.Subject.Name;
            SessionClassGroupId = db.SessionClassGroupId.ToString();
            SessionClassGroupName = db.SessionClassGroup.GroupName;
            SessionTermId = db.SessionTermId.ToString();
            SessionTermName = db.SessionTerm.TermName;
            NumberOfStudentsSubmitted = db.HomeAssessmentFeedBacks.Count(d => d.Status == 3); //3 of HomeAssessmentStatus;
            NumberOfStudentsNotSubmitted = Convert.ToInt32((NumberOfStudentsSubmitted - db.SessionClassGroup.ListOfStudentContactIds.Split(',').Count()).ToString().TrimStart('-'));
            if (db.Status == 1)
                Status = "Open";
            if (db.Status == 2)
                Status = "Closed";
            if (db.Status == 3)
                Status = "Submitted";
            if (db.Status == 0)
                Status = "Saved";
        }
    }

    public class SubmittedAndUnsubmittedStudent
    {
        public string StudentName { get; set; }
        public string RegistrationNumber { get; set; }
        public string Status { get; set; }
        public string HomeAsessmentFeedbackId { get; set; }
    }

    public class CreateHomeAssessmentRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int AssessmentScore { get; set; }
        public string SessionClassId { get; set; }
        public string SessionClassSubjectId { get; set; }
        public string SessionClassGroupId { get; set; }
        public bool ShouldSendToStudents { get; set; }
    }

    public class UpdateHomeAssessmentRequest
    {
        public string HomeAssessmentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int AssessmentScore { get; set; }
        public string SessionClassId { get; set; }
        public string SessionClassSubjectId { get; set; }
        public string SessionClassGroupId { get; set; }
        public bool ShouldSendToStudents { get; set; }
    }
}
