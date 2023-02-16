using DAL.Authentication;
using DAL.ClassEntities;
using SMP.DAL.Models.NoteEntities;
using SMP.DAL.Models.Parents;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.StudentInformation
{
    public class StudentContact : CommonEntity
    {
        [Key]
        public Guid StudentContactId { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
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
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public int Status { get; set; }
        public int EnrollmentStatus { get; set; }
        public string Hobbies { get; set; }
        public string BestSubjectIds { get; set; }
        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Parents Parent { get; set; }
        public virtual ICollection<ScoreEntry> ScoreEntries { get; set; }
        public virtual ICollection<StudentNote> StudentNote { get; set; }
        public virtual ICollection<SessionClassArchive> SessionClassArchive { get; set; }
    }
}
