using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.AssessmentEntities;
using System.Collections.Generic;

namespace SMP.Contracts.Assessment
{

    public class CreateHomeAssessmentFeedback
    {
        public string HomeAssessmentFeedBackId { get; set; }
        public string Content { get; set; }
        public bool ShouldSubmit { get; set; }
        public string HomeAssessmentId { get; set; }
        public List<IFormFile> Files { get; set; }
    }

    public class StudentHomeAssessmentRequest
    {
        public string HomeAssessmentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int AssessmentScore { get; set; }
        public string SessionClassId { get; set; }
        public string SessionClassSubjectId { get; set; }
        public string SessionClassSubjectName { get; set; }
        public string SessionClassGroupId { get; set; }
        public string SessionClassGroupName { get; set; }
        public string Status { get; set; }
        public string HomeAssessmentFeedBackId { get; set; }
        public string ListOfStudentContactIds { get; set; }
        public StudentHomeAssessmentRequest(HomeAssessment db)
        {
            HomeAssessmentId = db.HomeAssessmentId.ToString();
            Title = db.Title;
            Content = db.Content;
            AssessmentScore = db.AssessmentScore;
            SessionClassId = db.SessionClassId.ToString();
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SessionClassSubjectName = db.SessionClassSubject.Subject.Name;
            SessionClassGroupId = db.SessionClassGroupId.ToString();
            SessionClassGroupName = db.SessionClassGroup.GroupName;
            ListOfStudentContactIds = db.SessionClassGroup.ListOfStudentContactIds;
            if (db.Status == 1)
                Status = "open";
            if (db.Status == 2)
                Status = "closed";
        }
    }

}
