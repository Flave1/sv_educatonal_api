using DAL.ClassEntities;
using SMP.DAL.Models.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Admissions
{
    public class SelectAdmission
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
        public int ExamStatus { get; set; }
        public int CandidateAdmissionStatus { get; set; }
        public string CandidateCategory { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public SelectAdmission(Admission admission, ClassLookup classLookup)
        {
            AdmissionId = admission.AdmissionId.ToString();
            Firstname  = admission.Firstname;
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
            ParentName = admission.ParentName;
            ParentRelationship = admission.ParentRelationship;
            ParentPhoneNumber = admission.PhoneNumber;
            ParentEmail = admission.AdmissionNotification.ParentEmail;
            ExamStatus = admission.ExaminationStatus;
            CandidateAdmissionStatus = admission.CandidateAdmissionStatus;
            CandidateCategory = admission.CandidateCategory;
            ClassId = admission.ClassId.ToString();
            ClassName = classLookup.Name;
        }
    }
}
