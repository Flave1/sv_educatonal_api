using DAL;
using DAL.ClassEntities;
using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.Annoucement;
using SMP.DAL.Models.Attendance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Annoucements
{

    public class UpdatSeenAnnouncement
    {
        public string AnnouncementsId { get; set; }
    }
    public class CreateAnnouncement
    {
        public string AssignedTo { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
    }
    public class UpdateAnnouncement
    {
        public Guid AnnouncementsId { get; set; }
        public string AssignedTo { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
    }
    public class GetAnnouncements
    {
        public Guid AnnouncementsId { get; set; }
        public string SentBy { get; set; }
        public string SenderName { get; set; }
        public string AssignedTo { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string AnnouncementDate { get; set; }
        public bool IsSeen { get; set; }
        public bool IsEdited { get; set; }
        public GetAnnouncements(Announcements db, string userId)
        { 
            AnnouncementsId = db.AnnouncementsId;
            SentBy = db.SentBy.ToString();
            AssignedTo = db.AssignedTo;
            Header = db.Header;
            Content = db.Content;
            AnnouncementDate = db.AnnouncementDate.ToString("dd-MM-yyy hh:mm");
            IsEdited = db.IsEdited;
            //SenderName = db.Sender.FirstName + " " + db.Sender.LastName;
            IsSeen = !string.IsNullOrEmpty(db.SeenByIds) ? db.SeenByIds.Split(',').ToList().Contains(userId) : false;
        }
    }
}
