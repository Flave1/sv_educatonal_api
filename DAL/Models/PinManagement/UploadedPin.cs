using DAL;
using DAL.SessionEntities;
using DAL.StudentInformation;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.PinManagement
{
    public class UploadedPin:CommonEntity
    {
        [Key]
        public Guid UploadedPinId { get; set; }
        public Guid StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact Student { get; set; }
        public Guid SessionId { get; set; }
        [ForeignKey("SessionId")]
        public Session Session { get; set; }
        public Guid SessionTermId { get; set; }

        [ForeignKey("SessionTermId")]
        public SessionTerm Sessionterm { get; set; }
        public string Pin { get; set; }
        public DateTime DateUsed { get; set; }
        public ICollection<UsedPin> Used { get; set; }
    }
} 