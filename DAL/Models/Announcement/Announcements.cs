using DAL;
using DAL.ClassEntities;
using SMP.DAL.Models.Attendance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Annoucement
{
    public class Announcements : CommonEntity
    {
        [Key]  
        public Guid AnnouncementsId { get; set; } 
        public Guid SeenById { get; set; }
        public string AssignedBy { get; set; }  
        public string AssignedTo { get; set; }  
        public string Subject { get; set; }  
        public string Body { get; set; }  
        public DateTime AnnouncementDate { get; set; }  
        public Guid SeenAnnouncementsId { get; set; }
        public ICollection<SeenAnnouncements> SeenAnnouncements { get; set; }
    }
}
