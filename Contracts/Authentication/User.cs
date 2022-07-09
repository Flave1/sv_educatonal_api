using DAL.Authentication;
using DAL.TeachersInfor;
using System; 

namespace Contracts.Authentication
{
    public class UserCommand: UpdateTeacher
    {
        public string Id { get; set; }
        public string Email { get; set; } 
    }


    public class ApplicationUser
    {
        public string TeacherAccountId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool Verified { get; set; }
        public string TeacherUserAccountId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }   
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string dob { get; set; }

        public ApplicationUser() { }
        public ApplicationUser(Teacher db)
        {
            TeacherAccountId = db.TeacherId.ToString();
            Email = db.User.Email;
            UserName = db.User.UserName;
            Verified = db.User.EmailConfirmed;
            TeacherUserAccountId = db.User.Id;
            FullName = db.User.FirstName + " " + db.User.LastName;
            FirstName = db.User.FirstName;
            LastName = db.User.LastName;
            Phone = db.User.PhoneNumber;
            MiddleName = db.User.MiddleName;
            dob = db.User.DOB;
        }
    }

    public class AddToRole
    {
        public string[] UserIds { get; set; }
        public string RoleId { get; set; }
    }

    public class UpdateTeacher
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Photo { get; set; }
        public string TeacherUserAccountId { get; set; }
    }
}
