using DAL.TeachersInfor;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contracts.Authentication
{
    public class UserCommand: UpdateTeacher
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string ClientId { get; set; }
        public string SchoolUrl { get; set; }
        public string PasswordHash { get; set; }
        public IFormFile ProfileImage { get; set; }

        public string SchoolName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string SchoolLogo { get; set; }
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
        public string Photo { get; set; }
        public string[] Hobbies { get; set; }
        public string ShortBiography { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Status { get; set; }
        public ApplicationUser() { }
        public ApplicationUser(Teacher db)
        {
            TeacherAccountId = db.TeacherId.ToString();
            Email = db.User.Email;
            UserName = db.User.UserName;
            Verified = db.User.EmailConfirmed;
            TeacherUserAccountId = db.User.Id;
            FullName = db.FirstName + " " + db.LastName;
            FirstName = db.FirstName;
            LastName = db.LastName;
            Phone = db.Phone;
            MiddleName = db.MiddleName;
            dob = db.DOB;
            Photo = db.Photo;
            Hobbies = !string.IsNullOrEmpty(db.Hobbies) ? db.Hobbies.Split(',').ToArray() : Array.Empty<string>();
            ShortBiography = db.ShortBiography;
            Address = db.Address;
            Gender = db.Gender;
            MaritalStatus = db.MaritalStatus;
            Status = db.Status == 1 ? "active" : "inactive";
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

    public class UpdateProfileByTeacher
    {
        public string Email { get; set; }
        public IFormFile ProfileImage { get; set; }
        public List<string> Hobbies { get; set; }
        public string ShortBiography { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string TeacherUserAccountId { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
    }

    public class TeacherClassesAsFormTeacher
    {
        public string Class { get; set; }
        public  List<string> SubjectsInClass { get; set; }
    }
    public class TeacherSubjectsAsSubjectTeacher
    {
        public string Subject { get; set; }
        public List<string> Class { get; set; }
    }

    public class TeacheerClassAndSibjects
    {
        public List<TeacherClassesAsFormTeacher> ClassesAsFormTeacher { get; set; } = new List<TeacherClassesAsFormTeacher>();
        public List<TeacherSubjectsAsSubjectTeacher> SubjectsAsSubjectTeacher { get; set; } = new List<TeacherSubjectsAsSubjectTeacher>();

    }

    public class CreateUserCommand
    {
        public string Email { get; set; }
    }
    public class UpdateUserCommand
    {
        public string fwsUserId { get; set; }
        public string Email { get; set; }
    }
}
