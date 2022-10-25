﻿using DAL.Authentication;
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
        public string Address { get; set; }
        public string ShortBiography { get; set; }
        public string Hobbies { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public int Status { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
