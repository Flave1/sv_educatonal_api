using DAL.ClassEntities;
using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.Admission;
using SMP.DAL.Models.Parents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Admissions
{
    public class SelectCandidateAdmission
    {
        public string AdmissionId { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string CountryOfOrigin { get; set; }
        public string StateOfOrigin { get; set; }
        public string LGAOfOrigin { get; set; }
        public string Credentials { get; set; }
        public string Photo { get; set; }
        public string ParentName { get; set; }
        public string ParentRelationship { get; set; }
        public string ParentPhoneNumber { get; set; }
        public string ParentEmail { get; set; }
        public int CandidateAdmissionStatus { get; set; }
        public int ExamStatus { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public string AdmissionSettingId { get; set; }
        public string AdmissionSettingName { get; set; }
        public string CBTCategoryId { get; set; }
        public string CBTCategoryName { get; set; }
        public SelectCandidateAdmission(Admission admission, ClassLookup classLookup, Parents parents)
        {
            AdmissionId = admission.AdmissionId.ToString();
            Firstname = admission.Firstname;
            Middlename = !string.IsNullOrEmpty(admission.Middlename) ? admission.Middlename : "";
            Lastname = admission.Lastname;
            Email = admission.Email;
            PhoneNumber = admission.PhoneNumber;
            DateOfBirth = admission.DateOfBirth.ToString("yyyy-MM-dd");
            CountryOfOrigin = admission.CountryOfOrigin;
            StateOfOrigin = admission.StateOfOrigin;
            LGAOfOrigin = admission.LGAOfOrigin;
            Credentials = admission.Credentials; 
            Photo = admission.Photo; 
            ParentName = $"{parents.FirstName} {parents.LastName}";
            ParentRelationship = parents.Relationship;
            ParentPhoneNumber = parents.Phone;
            ParentEmail = parents.Email;
            CandidateAdmissionStatus = admission.CandidateAdmissionStatus;
            ExamStatus = admission.ExaminationStatus;
            ClassId = admission.ClassId.ToString();
            ClassName = classLookup.Name;
            AdmissionSettingId = admission.AdmissionSettings.AdmissionSettingId.ToString();
            AdmissionSettingName = admission.AdmissionSettings.AdmissionSettingName;
            CBTCategoryId = admission.CandidateCategory;
            CBTCategoryName = admission.CandidateCategoryName;
        }
    }
}
