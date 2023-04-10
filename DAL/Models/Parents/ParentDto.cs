using System;

namespace SMP.DAL.Models.Parents
{
    public class ParentDto
    {
        public Guid Parentid { get; set; }
        public string Number { get; set; }
        public string Relationship { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }
        public ParentDto(Parents db)
        {
            Parentid = db.Parentid;
            Number = db.Number;
            Relationship = db.Relationship;
            Email = db.Email;
            UserId = db.UserId;
            FirstName = db.FirstName;
            LastName = db.LastName;
            MiddleName = db.MiddleName;
            Phone = db.Phone;
            Photo = db.Photo;

        }
    }
}
