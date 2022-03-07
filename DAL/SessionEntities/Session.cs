using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.SessionEntities
{
    public class Session : CommonEntity
    {
        [Key]
        public Guid SessionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
