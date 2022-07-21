using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.PinManagement
{
    public class UploadedPin:CommonEntity
    {
        [Key]
        public Guid UploadedPinId { get; set; }
        public string Pin { get; set; }
        public ICollection<UsedPin> UsedPin { get; set; }
    }
} 