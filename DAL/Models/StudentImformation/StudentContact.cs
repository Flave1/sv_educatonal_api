using DAL.Authentication;
using DAL.ClassEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public string ParentOrGuardianName { get; set; }
        public string ParentOrGuardianRelationship { get; set; }
        public string ParentOrGuardianPhone { get; set; }
        public string ParentOrGuardianEmail { get; set; }
        public string HomeAddress { get; set; }
        public string CityId { get; set; }
        public string StateId { get; set; }
        public string CountryId { get; set; }
        public string ZipCode { get; set; }  
        public string RegistrationNumber { get; set; }
        public SessionClass Class { get; set; }
        public virtual ICollection<StudentClassProgressions> ClassProgressions { get; set; }
    }
}
