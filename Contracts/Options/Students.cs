﻿using DAL.StudentInformation;
using DAL.SubjectModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Contracts.Options
{
    public class ResetAccount
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string ResetToken { get; set; }
        public string SchoolUrl { get; set; }
    }
    public class ResetAccountMobile
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
    }
    public class ValidateEmail
    { 
        public string Email { get; set; }
        public string ClientId { get; set; }
        public int UserType { get; set; }
    }
    public class ValidateOtp
    {
        public string Otp { get; set; }
        public string ClientId { get; set; }
    }
    //public class ResetAccount2
    //{
    //    public string Email { get; set; }
    //}
    public class ResetPassword
    {
        public string ResetOption { get; set; }
        public string ResetOptionValue { get; set; }
        public int UserType { get; set; }
        public string SchoolUrl { get; set; }
    }
    public class ChangePassword {
        public string UserId { get; set; }
        public string OldPassword { get; set; } 
        public string NewPassword { get; set; }
        public string SchoolUrl { get; set; }
        public int UserType { get; set; } = 0;
    }

    public class StudentContactCommand
    {
        /// <summary>
        /// CORE DETAILS
        /// </summary>
        public string UserAccountId { get; set; }
        public string StudentAccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Email { get; set; }


        public string HomePhone { get; set; }
        public string EmergencyPhone { get; set; }
        public string ParentOrGuardianFirstName { get; set; }
        public string ParentOrGuardianRelationship { get; set; }
        public string ParentOrGuardianPhone { get; set; }
        public string ParentOrGuardianEmail { get; set; }
        public string HomeAddress { get; set; }
        public string CityId { get; set; }
        public string StateId { get; set; }
        public string CountryId { get; set; }
        public string ZipCode { get; set; }
        public string Photo { get; set; }
        public string SessionClassId { get; set; } 
        public IFormFile ProfileImage { get; set; }
        public string ParentOrGuardianLastName { get; set; }
        public StudentContactCommand() { }

        public StudentContactCommand(SMP.DAL.Models.Admission.Admission
            admission, SMP.DAL.Models.Parents.Parents parent, string SessionClassId)
        {
            FirstName = admission.Firstname;
            LastName = admission.Lastname;
            MiddleName = admission.Middlename;
            Phone = admission.PhoneNumber;
            DOB = admission.DateOfBirth.ToString();
            Email = admission.Email;
            HomePhone = admission.PhoneNumber;
            EmergencyPhone = parent.Phone;
            ParentOrGuardianFirstName = parent.FirstName;
            ParentOrGuardianEmail = parent.Email;
            HomeAddress = $"{admission.LGAOfOrigin}, {admission.StateOfOrigin}, {admission.CountryOfOrigin}";
            CityId = admission.StateOfOrigin;
            StateId = admission.StateOfOrigin;
            CountryId = admission.CountryOfOrigin;
            this.SessionClassId = SessionClassId;
            Photo = admission.Photo;
        }
    }

    public class GetStudentContacts
    {
        public string StudentAccountId { get; set; }
        public string UserTypes { get; set; }
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
        public string SessionClassID { get; set; }
        public string SessionClass { get; set; }
        public string[] Hobbies { get; set; }
        public string[] BestSubjectNames { get; set; }
        public string[] BestSubjectIds { get; set; }
        public string Status { get; set; }
        public GetStudentContacts(StudentContact db, string regNoFormat, List<Subject> subjects)
        {
            RegistrationNumber = regNoFormat.Replace("%VALUE%", db.RegistrationNumber);
            UserTypes = db.User.UserTypes;
            Active = db.User.Active;
            FirstName = db.FirstName;
            LastName = db.LastName;
            MiddleName = db.MiddleName;
            Phone = db.Phone;
            DOB = db.DOB;
            Photo = db.Photo;
            HomePhone = db.HomePhone;
            EmergencyPhone = db.EmergencyPhone;
            ParentOrGuardianName = db.Parent?.FirstName +" "+ db.Parent?.LastName;
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
            Photo = db.Photo;
            Status = db.Status == 1 ? "active" : "inactive";
            SessionClassID = db.SessionClassId.ToString();
            SessionClass = db?.SessionClass?.Class?.Name;
            Hobbies = db.Hobbies is not null ? db.Hobbies.Split(',') : Array.Empty<string>();
            BestSubjectIds = !string.IsNullOrEmpty(db.BestSubjectIds) ? db.BestSubjectIds.Split(',') : Array.Empty<string>();
            BestSubjectNames = !string.IsNullOrEmpty(db.BestSubjectIds) ? subjects.Where(x => BestSubjectIds.Select(Guid.Parse)
            .Contains(x.SubjectId)).Select(a => a.Name).ToArray() : Array.Empty<string>();
        }

        public GetStudentContacts(StudentContact db, string regNoFormat)
        {
            RegistrationNumber = regNoFormat.Replace("%VALUE%", db.RegistrationNumber);
            UserTypes = db.User.UserTypes;
            Active = db.User.Active;
            FirstName = db.FirstName;
            LastName = db.LastName;
            MiddleName = db.MiddleName;
            Phone = db.Phone;
            DOB = db.DOB;
            Photo = db.Photo;
            HomePhone = db.HomePhone;
            EmergencyPhone = db.EmergencyPhone;
            ParentOrGuardianName = db.Parent?.FirstName + " " + db.Parent?.LastName;
            ParentOrGuardianRelationship = db.Parent?.Relationship;
            ParentOrGuardianPhone = db.Parent?.Number;
            ParentOrGuardianEmail = db.Parent?.Email;
            HomeAddress = db.HomeAddress;
            CityId = db.CityId;
            StateId = db.StateId;
            CountryId = db.CountryId;
            ZipCode = db.ZipCode;
            Status = db.Status == 1 ? "active" : "inactive";
            UserAccountId = db.User.Id;
            StudentAccountId = db.StudentContactId.ToString();
            UserName = db.User.UserName;
            Email = db.User.Email;
            Photo = db.Photo;
            SessionClassID = db.SessionClassId.ToString();
            SessionClass = db?.SessionClass?.Class?.Name;
            Hobbies = db.Hobbies is not null ? db.Hobbies.Split(',') : new string[0];
            BestSubjectIds = db.BestSubjectIds is not null ? db.BestSubjectIds.Split(',').ToArray() : new string[0];
        }

    }

    public class UpdateProfileByStudentRequest
    {
        public string StudentContactId { get; set; }
        public string[] Hobbies { get; set; }
        public string[] BestSubjectIds { get; set; }
        public IFormFile File { get; set; }
    }

    public class SmpStudentValidationResponse
    {
        public string Status { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string RegistrationNumber { get; set; }
        public string SchoolLogo { get; set; }
        public string Id { get; set; }
        public int UserType { get; set; }
        public string SchoolUrl { get; set; }
        public string ClientId { get; set; }
    }

    public class UploadClass
    {
        public IFormFile File { get; set; }
    }
    public class UploadStudentExcel
    {
        /// <summary>
        /// CORE DETAILS
        /// </summary>
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string RegistrationNumber { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string EmergencyPhone { get; set; }
        public string ParentOrGuardianFirstName { get; set; }
        public string ParentOrGuardianRelationship { get; set; }
        public string ParentOrGuardianPhone { get; set; }
        public string ParentOrGuardianEmail { get; set; }
        public string HomeAddress { get; set; }
        public string CityId { get; set; }
        public string StateId { get; set; }
        public string CountryId { get; set; }
        public string ZipCode { get; set; } 
        public string SessionClass { get; set; }
        public int ExcelLineNumber { get; set; }
        public string ParentOrGuardianLastName { get; set; }
    }


    public class GetStudentContacts2
    {
        public string StudentAccountId { get; set; }
        public string UserTypes { get; set; }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string RegistrationNumber { get; set; }
        public string Email { get; set; }
        public string SessionClassID { get; set; }
        public GetStudentContacts2(StudentContact db, string regNoFormat)
        {

            RegistrationNumber = regNoFormat.Replace("%VALUE%", db.RegistrationNumber);
            UserTypes = db.User.UserTypes;
            Active = db.User.Active;
            FirstName = db.FirstName;
            LastName = db.LastName;
            MiddleName = db.MiddleName;
            StudentAccountId = db.StudentContactId.ToString();
            Email = db.User.Email;
            SessionClassID = db.SessionClassId.ToString();
        }


    }

    public class SimplePostRequest
    {
        public Guid Id { get; set; }      
    }

}
