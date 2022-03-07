using DAL.Authentication;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.TeachersInfor
{
    public class Teacher : CommonEntity
    {
        [Key]
        public Guid TeacherId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
