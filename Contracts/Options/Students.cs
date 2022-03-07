using DAL.StudentInformation;

namespace Contracts.Options
{
    public class ResetAccount
    {
        public string UserId { get; set; }
        public string Password { get; set; } 
        public string ResetToken { get; set; }
    }
    public class ResetPassword
    {
        public string ResetOption { get; set; }
        public string ResetOptionValue { get; set; }
        public string UserType { get; set; }
    }
    public class StudentContactCommand
    {
        /// <summary>
        /// CORE DETAILS
        /// </summary>
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Email { get; set; }



        public string StudentContactId { get; set; }
        public string UserId { get; set; }
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
        public string Photo { get; set; }

    }

    public class GetStudentContacts
    {
        public int UserType { get; set; }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Photo { get; set; }
        public string StudentContactId { get; set; }
        public string UserId { get; set; }
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
        public GetStudentContacts(StudentContact db, string regNoFormat)
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
            StudentContactId = db.StudentContactId.ToString();
            UserId = db.UserId;
            HomePhone = db.HomePhone;
            EmergencyPhone = db.EmergencyPhone;
            ParentOrGuardianName = db.ParentOrGuardianName;
            ParentOrGuardianRelationship = db.ParentOrGuardianRelationship;
            ParentOrGuardianPhone = db.ParentOrGuardianPhone;
            ParentOrGuardianEmail = db.ParentOrGuardianEmail;
            HomeAddress = db.HomeAddress;
            CityId = db.CityId;
            StateId = db.StateId;
            CountryId = db.CountryId;
            ZipCode = db.ZipCode;
        }

    }
}
