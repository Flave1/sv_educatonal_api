using DAL;
using DAL.ClassEntities;
using DAL.SessionEntities;
using DAL.StudentInformation;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.PinManagement
{
    public class UsedPin : CommonEntity
    {
        [Key]
        public Guid UsedPinId { get; set; }
        public DateTime DateUsed { get; set; }
        public Guid StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact Student { get; set; }
        public Guid SessionTermId { get; set; }

        [ForeignKey("SessionTermId")]
        public SessionTerm Sessionterm { get; set; }
        public Guid UploadedPinId { get; set; }
        [ForeignKey("UploadedPinId")]
        public UploadedPin UploadedPin { get; set; }

        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
       
    }
}
