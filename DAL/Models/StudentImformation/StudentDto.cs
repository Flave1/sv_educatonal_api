using System;
using DAL.StudentInformation;

namespace SMP.DAL.Models.StudentImformation
{
    public class StudentDto
    {
        public Guid StudentContactId { get; set; }
        public string UserId { get; set; }
        public string RegistrationNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }
        public int Status { get; set; }
        public int EnrollmentStatus { get; set; }
        public Guid? ParentId { get; set; }
        public StudentDto(StudentContact db)
        {
            StudentContactId = db.StudentContactId;
            UserId = db.UserId;
            RegistrationNumber = db.RegistrationNumber;
            FirstName = db.FirstName;
            LastName = db.LastName;
            MiddleName = db.MiddleName;
            Phone = db.Phone;
            Photo= db.Photo;
            Status = db.Status;
            EnrollmentStatus = db.EnrollmentStatus;
            ParentId = db.ParentId;
        }
    }
}
