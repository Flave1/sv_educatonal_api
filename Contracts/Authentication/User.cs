using DAL.Authentication;
using DAL.TeachersInfor;
using System; 

namespace Contracts.Authentication
{
    public class UserCommand 
    {
        public string Id { get; set; }
        public string Email { get; set; } 
    }


    public class ApplicationUser
    {
        public string UserAccountId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool Verified { get; set; }
        public string TeacherUserAccountId { get; set; }
        public ApplicationUser() { }
        public ApplicationUser(Teacher db)
        {
            UserAccountId = db.TeacherId.ToString();
            Email = db.User.Email;
            UserName = db.User.UserName;
            Verified = db.User.EmailConfirmed;
            TeacherUserAccountId = db.User.Id;
        }
    }

    public class AddToRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }

    public class UpdateTeacher
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Photo { get; set; }
        public string TeacherUserAccountId { get; set; }
    }
}
