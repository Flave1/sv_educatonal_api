using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using SMP.DAL.Models.Register;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Annoucement
{
    public class SeenAnnouncements: CommonEntity
    { 
        public Guid SeenAnnouncementsId { get; set; }
        public Guid SeenById { get; set; }
        public Guid AnnouncementsId { get; set; }
        public DateTime DateSent { get; set; }
    }
}
