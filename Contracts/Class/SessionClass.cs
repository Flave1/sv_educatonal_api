using DAL.ClassEntities;
using System;

namespace Contracts.Class
{
    public class SessionClassCommand
    { 
        public string SessionClassId { get; set; }
        public string SessionId { get; set; }  
        public string ClassId { get; set; }   
        public string FormTeacherId { get; set; }
        public string ClassCaptainId { get; set; }
        public bool InSession { get; set; }
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

        public GetSessionClass(SessionClass sClass)
        {
            InSession = sClass.InSession;
            ClassId = sClass.ClassId.ToString();
            SessionId = sClass.SessionId.ToString();
            SessionClassId = sClass.SessionClassId.ToString();
            FormTeacherId = sClass.FormTeacherId.ToString();
            ClassCaptainId = sClass.ClassCaptainId.ToString();
            Class = sClass.Class.Name;
            Session = sClass.Session.StartDate.ToString("dd/MM/yyyyy") + " - " + sClass.Session.EndDate.ToString("dd/MM/yyyyy");
            FormTeacher = sClass.Teacher.User.FirstName + " " + sClass.Teacher.User.LastName; 
            //ClassCaptain = sClass.ClassCaptain.User.FirstName + " " + sClass.ClassCaptain.User.LastName;
        }
    }

    public class SessionQuery
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
