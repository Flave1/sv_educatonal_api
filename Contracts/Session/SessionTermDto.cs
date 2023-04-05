using SMP.DAL.Models.SessionEntities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.Contracts.Session
{
    public class SessionTermDto
    {
        public Guid SessionTermId { get; set; }
        public string TermName { get; set; }
        public bool IsActive { get; set; }
        public Guid SessionId { get; set; }
        //public virtual SessionDto Session { get; set; }
        public SessionTermDto(SessionTerm db)
        {
            SessionTermId = db.SessionTermId;
            TermName = db.TermName;
            IsActive = db.IsActive;
            SessionId = db.SessionId;
        }
    }

    public class SessionDto
    {
        public Guid SessionId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get; set; }
        public Guid? HeadTeacherId { get; set; }
        public TeacherDto HeadTeacher { get; set; }
    }
    public class TeacherDto
    {
        public string UserId { get; set; }
        public string Address { get; set; }
        public string ShortBiography { get; set; }
        public string Hobbies { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string HomePhone { get; set; }
        public string EmergencyPhone { get; set; }
        public string HomeAddress { get; set; }
        public string CityId { get; set; }
        public string StateId { get; set; }
        public string CountryId { get; set; }
        public string ZipCode { get; set; }
        public string RegistrationNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Photo { get; set; }
        public int Status { get; set; }
    }
}
