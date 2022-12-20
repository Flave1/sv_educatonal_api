using DAL.StudentInformation;
using DAL.SubjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Students
{
    public class GetStudentContactCbt
    {
        public string StudentAccountId { get; set; }
        public int UserType { get; set; }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Photo { get; set; }
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
        public string UserAccountId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public GetStudentContactCbt(StudentContact db, string regNoFormat)
        {
            RegistrationNumber = regNoFormat.Replace("%VALUE%", db.RegistrationNumber);
            UserType = db.User.UserType;
            Active = db.User.Active;
            FirstName = db.User.FirstName;
            LastName = db.User.LastName;
            MiddleName = db.User.MiddleName;
            Phone = db.User.Phone;
            DOB = db.User.DOB;
            Photo = db.User.Photo;
            HomePhone = db.HomePhone;
            EmergencyPhone = db.EmergencyPhone;
            ParentOrGuardianName = db.Parent?.Name;
            ParentOrGuardianRelationship = db.Parent?.Relationship;
            ParentOrGuardianPhone = db.Parent?.Number;
            ParentOrGuardianEmail = db.Parent?.Email;
            HomeAddress = db.HomeAddress;
            CityId = db.CityId;
            StateId = db.StateId;
            CountryId = db.CountryId;
            ZipCode = db.ZipCode;
            UserAccountId = db.User.Id;
            StudentAccountId = db.StudentContactId.ToString();
            UserName = db.User.UserName;
            Email = db.User.Email;
            Photo = db.User.Photo;
        }
    }
}
