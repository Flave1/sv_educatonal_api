using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.PinManagement
{
    public class UsedPin : CommonEntity
    {
        [Key]
        public Guid UsedPinId { get; set; }
        public string UploadedPin { get; set; }
        [ForeignKey("UploadedPinId")]
        public UploadedPin Pin { get; set; }
    }
}
