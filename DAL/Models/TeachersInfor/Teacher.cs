using DAL.Authentication;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.TeachersInfor
{
    public class Teacher : CommonEntity
    {
        [Key]
        public Guid TeacherId { get; set; }
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
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
