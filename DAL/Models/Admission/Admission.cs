using DAL;
using DAL.ClassEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Admission
{
    public class Admission: CommonEntity
    {
        [Key]
        public Guid AdmissionId { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string CountryOfOrigin { get; set; }
        public string StateOfOrigin { get; set; }
        public string LGAOfOrigin { get; set; }
        public string Photo { get; set; }
        public string Credentials { get; set; }
        public int CandidateAdmissionStatus { get; set; }
        public string CandidateCategory { get; set; }
        public string CandidateCategoryName { get; set; }
        public int ExaminationStatus { get; set; }
        public string ExaminationId { get; set; }
        public Guid AdmissionSettingId { get; set; }
        [ForeignKey("AdmissionSettingId")]
        public AdmissionSetting AdmissionSettings { get; set; }
        public Guid ClassId { get; set; }
        [ForeignKey("ClassId")]
        public ClassLookup Class { get; set; }
        public Guid? ParentId { get; set; }
    }
}
