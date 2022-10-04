using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.AssessmentEntities;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public class ScoreHomeAssessmentFeedback
    {
        public string HomeAssessmentFeedBackId { get; set; }
        public decimal Score { get; set; }
        public string Comment { get; set; }
        public bool Include { get; set; } = false;
    }
    public class SingleHomeAssessment
    {
        public string HomeAssessmentId { get; set; }
    }
    public class SingleFeedback
    {
        public string HomeAssessmentFeedBackId { get; set; }
    }
    public class StudentHomeAssessmentRequest
    {
        public string HomeAssessmentFeedBackId { get; set; }
        public string HomeAssessmentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Comment { get; set; }
        public int AssessmentScore { get; set; }
        public string SessionClassId { get; set; }
        public string SessionClassSubjectId { get; set; }
        public string SessionClassSubjectName { get; set; }
        public string SessionClassGroupId { get; set; }
        public string SessionClassGroupName { get; set; }
        public string Status { get; set; }
        public string DateDeadLine { get; set; }
        public string TimeDeadLine { get; set; }
        public string ListOfStudentContactIds { get; set; }
        public StudentHomeAssessmentRequest(HomeAssessment db, string studentContactId)
        {
            var stAss = db.HomeAssessmentFeedBacks.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactId));
            DateDeadLine = db.DateDeadLine;
            TimeDeadLine = db.TimeDeadLine;
            HomeAssessmentId = db.HomeAssessmentId.ToString();
            Title = db.Title;
            Content = db.Content;
            HomeAssessmentFeedBackId = stAss?.HomeAssessmentFeedBackId.ToString();
            AssessmentScore = db.AssessmentScore;
            SessionClassId = db.SessionClassId.ToString();
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SessionClassSubjectName = db.SessionClassSubject.Subject.Name;
            SessionClassGroupId = db.SessionClassGroupId.ToString();
            SessionClassGroupName = db.SessionClassGroup.GroupName;
            ListOfStudentContactIds = db.SessionClassGroup.ListOfStudentContactIds;
           
            Comment = db.Comment;
            if (db.Status == 1)
                Status = "open";
            if (db.Status == 2)
                Status = "closed";

            if (!string.IsNullOrEmpty(HomeAssessmentFeedBackId))
            {
                Status = "unsubmitted";
                if(stAss.Status == 3)
                {
                    Status = "submitted";
                }
            }

        }

     
    }

    public class GetHomeAssessmentFeedback
    {
        public string HomeAssessmentFeedBackId { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public decimal Score { get; set; }
        public string HomeAssessmentId { get; set; }
        public string StatusName { get; set; }
        public List<string> Files { get; set; }

        public GetHomeAssessmentRequest Assessment { get; set; }
        public GetHomeAssessmentFeedback(HomeAssessmentFeedBack db)
        {
            Content = db.Content;
            HomeAssessmentFeedBackId = db.HomeAssessmentFeedBackId.ToString();
            HomeAssessmentId = db.HomeAssessmentId.ToString();
            Status = db.Status;
            if (db.Status == 1)
                StatusName = "open";
            if (db.Status == 2)
                StatusName = "closed";
            if (db.Status == 3)
                StatusName = "submitted";
            if (db.Status == 0)
                StatusName = "saved";

            if (db.HomeAssessment is not null)
            {
                Assessment = new GetHomeAssessmentRequest(db.HomeAssessment, 0);
            }
        }

    }
}
