using System;

namespace Contracts.Session
{
    public class CreateUpdateSession
    { 
        public string SessionId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Terms { get; set; }
        public string HeadTeacherId { get; set; }
    }

    public class GetSession
    {
        public string SessionId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get; set; }
        public Guid? HeadTeacherId { get; set; }
        public string HeadTeacherName { get; set; }
        public int NoOfStudents { get; set; }
        public int NoOfSubjects { get; set; }
        public int NoOfClasses { get; set; }
        public Terms[] Terms { get; set; } = new Terms[0];
        public GetSessionClass[] SessionClasses { get; set; } = new GetSessionClass[0];
    }

    public class Terms
    {
        public Guid SessionTermId { get; set; }
        public string TermName { get; set; }
        public bool IsActive { get; set; }
    }
    public class ActivateTerm
    {
        public string SessionTermId { get; set; }
    }

    public class ActiveSession
    {
        public Guid SessionId { get; set; }
        public string Session { get; set; }
        public string SessionTerm { get; set; }
        public Guid SessionTermId { get; set; }
        public Terms[] Terms { get; set; } = new Terms[0];
    }

    public class UpdateHeadTeacher
    {
        public string HeadTeacherId { get; set; }
        public string SessionId { get; set; }
    }

    public class GetSessionClass
    {
        public string SessionClass { get; set; }
        public string FormTeacher { get; set; }
        public Guid SessionClassId { get; set; }
    }
}
