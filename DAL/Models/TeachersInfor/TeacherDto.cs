using DAL.TeachersInfor;
using System;

namespace SMP.DAL.Models.TeachersInfor
{
    public class TeacherDto
    {
        public Guid TeacherId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public string ClientId { get; set; }
        public TeacherDto(Teacher db)
        {
            TeacherId = db.TeacherId;
            UserId = db.UserId;
            FirstName = db.FirstName;
            LastName = db.LastName;
            ClientId = db.ClientId;
            Status = db.Status;
            MiddleName = db.MiddleName;
            Phone = db.Phone;
        }
    }
}
