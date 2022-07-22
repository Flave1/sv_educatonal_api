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
    public class AnnouncementsContract
    {
        public Guid AnnouncementsId { get; set; }
        public string SeenBy { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime AnnouncementDate { get; set; }
    }
    public class GetAnnouncementsContract
    {
        public Guid AnnouncementsId { get; set; }
        public string SeenBy { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime AnnouncementDate { get; set; }
        public GetAnnouncementsContract(Announcements db)
        { 

            AnnouncementsId = db.AnnouncementsId;
            SeenBy = db.SeenById.ToString();
            AssignedBy = db.AssignedBy;
            AssignedTo = db.AssignedTo;
            Subject = db.Subject;
            Body = db.Body;
            AnnouncementDate = db.AnnouncementDate;
        }
    }
}
