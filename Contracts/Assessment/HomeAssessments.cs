using DAL.StudentInformation;
using SMP.DAL.Models.AssessmentEntities;
using System;
using System.Collections.Generic;
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
        public string Comment { get; }
        public int NumberOfStudentsSubmitted { get; set; } = 0;
        public int NumberOfStudentsNotSubmitted { get; set; }
        public string Status { get; set; }
        //public string SubmitionStatus { get; set; }
        public string DateDeadLine { get; set; }
        public string TimeDeadLine { get; set; }
        public string TeacherName { get; set; }
        public Guid? TeacherId { get; set; }
        public bool Included { get; set; }
        public int StudentCount { get; set; }
        public List<SubmittedAndUnsubmittedStudents> StudentList { get; set; }
        public GetHomeAssessmentRequest()
        {

        }
        public GetHomeAssessmentRequest(HomeAssessment db, int totalNumberOfStudents, bool isSingle = true)
        {
            DateDeadLine = db.DateDeadLine;
            TimeDeadLine = db.TimeDeadLine;
            HomeAssessmentId = db.HomeAssessmentId.ToString();
            Title = db.Title;
            Content = isSingle ? db.Content : "";
            AssessmentScore = db.AssessmentScore;
            SessionClassId = db.SessionClassId.ToString();
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SessionClassSubjectName = db.SessionClassSubject.Subject.Name;
            SessionClassGroupId = db.SessionClassGroupId.ToString();
            SessionClassGroupName = db.SessionClassGroup.GroupName;
            SessionTermId = db.SessionTermId.ToString();
            SessionTermName = db.SessionTerm.TermName;
            Comment = db.Comment;
            TeacherId = db.TeacherId;
            
            if (db.HomeAssessmentFeedBacks is not null && db.HomeAssessmentFeedBacks.Any())
            {
                NumberOfStudentsSubmitted =  db.HomeAssessmentFeedBacks.Count(d => d.Status == 3); //3 of HomeAssessmentStatus;
            }
            if(db.SessionClassGroup.GroupName == "all-students")
            {
                NumberOfStudentsNotSubmitted = Convert.ToInt32((NumberOfStudentsSubmitted -
                    totalNumberOfStudents).ToString().TrimStart('-'));
            }
            else
            {
                NumberOfStudentsNotSubmitted = Convert.ToInt32((NumberOfStudentsSubmitted -
                    db.SessionClassGroup.ListOfStudentContactIds.Split(',').Count()).ToString().TrimStart('-'));
            }
            if (db.Status == 1)
                Status = "open";
            if (db.Status == 2)
                Status = "closed";
            if (db.Status == 3)
                Status = "submitted";
            if (db.Status == 0)
                Status = "saved";
        }
        public GetHomeAssessmentRequest(HomeAssessment db, ICollection<StudentContact> classtudents)
        {
            var studentIds = !string.IsNullOrEmpty(db.SessionClassGroup.ListOfStudentContactIds) ? 
                db.SessionClassGroup.ListOfStudentContactIds.Split(',').ToList() : new List<string>();
            if (db.SessionClassGroup.GroupName == "all-students")
            {
               classtudents.Select(s => s.StudentContactId).ToList().ForEach(ele =>
               {
                   studentIds.Add(ele.ToString());
               });
            }

            DateDeadLine = db.DateDeadLine;
            TimeDeadLine = db.TimeDeadLine;
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
            Comment = db.Comment;
            TeacherId = db.TeacherId;
            NumberOfStudentsSubmitted = db.HomeAssessmentFeedBacks.Count(d => d.Status == 3); //3 of HomeAssessmentStatus;
            NumberOfStudentsNotSubmitted = Convert.ToInt32((NumberOfStudentsSubmitted - studentIds.Count()).ToString().TrimStart('-'));
            if (db.Status == 1)
                Status = "open";
            if (db.Status == 2)
                Status = "closed";
            if (db.Status == 3)
                Status = "submitted";
            if (db.Status == 0)
                Status = "saved";
        
            if (studentIds.Any())
                StudentList = studentIds.OrderByDescending(x =>  db.HomeAssessmentFeedBacks.Select(s => s.StudentContactId.ToString()).Contains(x)).Select(id => new SubmittedAndUnsubmittedStudents(id, db.HomeAssessmentFeedBacks, classtudents)).ToList();
        }

        public GetHomeAssessmentRequest(HomeAssessment db, ICollection<StudentContact> classtudents, bool isMobile = true, bool isSingle = true)
        {
            var studentIds = !string.IsNullOrEmpty(db.SessionClassGroup.ListOfStudentContactIds) ?
                db.SessionClassGroup.ListOfStudentContactIds.Split(',').ToList() : new List<string>();
            if (db.SessionClassGroup.GroupName == "all-students")
            {
                classtudents.Select(s => s.StudentContactId).ToList().ForEach(ele =>
                {
                    studentIds.Add(ele.ToString());
                });
            }

            DateDeadLine = db.DateDeadLine;
            TimeDeadLine = db.TimeDeadLine;
            HomeAssessmentId = db.HomeAssessmentId.ToString();
            Title = db.Title;
            AssessmentScore = db.AssessmentScore;
            SessionClassId = db.SessionClassId.ToString();
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SessionClassSubjectName = db.SessionClassSubject.Subject.Name;
            SessionClassGroupId = db.SessionClassGroupId.ToString();
            SessionClassGroupName = db.SessionClassGroup.GroupName;
            SessionTermId = db.SessionTermId.ToString();
            SessionTermName = db.SessionTerm.TermName;
            Comment = db.Comment;
            Content = db.Content;
            TeacherId = db.TeacherId;
            NumberOfStudentsSubmitted = db.HomeAssessmentFeedBacks.Count(d => d.Status == 3); //3 of HomeAssessmentStatus;
            NumberOfStudentsNotSubmitted = Convert.ToInt32((NumberOfStudentsSubmitted - studentIds.Count()).ToString().TrimStart('-'));
            if (db.Status == 1)
                Status = "open";
            if (db.Status == 2)
                Status = "closed";
            if (db.Status == 3)
                Status = "submitted";
            if (db.Status == 0)
                Status = "saved";

            if (studentIds.Any())
                StudentCount = studentIds.Count();
                //StudentList = studentIds.OrderByDescending(x => db.HomeAssessmentFeedBacks.Select(s => s.StudentContactId.ToString()).Contains(x)).Select(id => new SubmittedAndUnsubmittedStudents(id, db.HomeAssessmentFeedBacks, classtudents)).ToList();
        }
    }

    public class SubmittedAndUnsubmittedStudents
    {
        public string StudentName { get; set; }
        public string Status { get; set; }
        public string HomeAsessmentFeedbackId { get; set; }
        public decimal Score { get; set; }
        public bool Included { get; set; }
        public SubmittedAndUnsubmittedStudents(string studentContactId, ICollection<HomeAssessmentFeedBack> feedbacks, ICollection<StudentContact> students)
        {
            var feedBack = feedbacks.FirstOrDefault(s => s.StudentContactId == Guid.Parse(studentContactId));
            var student = students.FirstOrDefault(s => s.StudentContactId == Guid.Parse(studentContactId));
            HomeAsessmentFeedbackId = feedBack is not null ? feedBack.HomeAssessmentFeedBackId.ToString() : "";
            StudentName = student?.FirstName + " " + student?.MiddleName + " " + student?.LastName;
            Score = feedBack?.Mark ??0;
            if (feedBack is not null)
            {
                if (feedBack.Status == 3)
                    Status = "submitted";
                if (feedBack.Status == 0)
                    Status = "uncompleted";
            }
            else
            {
                Status = "not started";
            }
        }
        public SubmittedAndUnsubmittedStudents(StudentContact student, ICollection<HomeAssessmentFeedBack> feedbacks)
        {
            var feedBack = feedbacks.FirstOrDefault(s => s.StudentContactId == student.StudentContactId);
            HomeAsessmentFeedbackId = feedBack is not null ? feedBack.HomeAssessmentFeedBackId.ToString() : "";
            StudentName = student?.FirstName + " " + student?.MiddleName + " " + student?.LastName;
            Score = feedBack?.Mark??0;
            if (feedBack is not null)
            {
                if (feedBack.Status == 3)
                    Status = "submitted";
                if (feedBack.Status == 0)
                    Status = "uncompleted";
            }
            else
            {
                Status = "not started";
            }
            Included = feedBack?.Included?? false;
        }
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
        public string Comment { get; set; }
        public string DateDeadLine { get; set; }
        public string TimeDeadLine { get; set; }
    }

    public class UpdateHomeAssessmentRequest
    {
        public string HomeAssessmentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int AssessmentScore { get; set; }
        public string SessionClassSubjectId { get; set; }
        public string SessionClassGroupId { get; set; }
        public bool ShouldSendToStudents { get; set; }
        public string Comment { get; set; }
        public string DateDeadLine { get; set; }
        public string TimeDeadLine { get; set; }
    }

    public class SendHomeAssessmentRequest
    {
        public string HomeAssessmentId { get; set; }
        public bool ShouldSendToStudents { get; set; }
    }

    public class GetClassAssessmentRecord
    {
        public int TotalAssessment { get; set; }
        public decimal Used { get; set; }
        public decimal Unused { get; set; }
    }
}
