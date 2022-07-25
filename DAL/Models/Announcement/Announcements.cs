using DAL;
using DAL.Authentication;
using DAL.ClassEntities;
using DAL.TeachersInfor;
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
        public string SeenByIds { get; set; }
        public string SentBy { get; set; }  
        public string AssignedTo { get; set; }  
        public string Header { get; set; }  
        public string Content { get; set; }  
        public bool IsEdited { get; set; }
        public DateTime AnnouncementDate { get; set; }
        [ForeignKey("SentBy")]
        public AppUser Sender { get; set; }
    }
}
