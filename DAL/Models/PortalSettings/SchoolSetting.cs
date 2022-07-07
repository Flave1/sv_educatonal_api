using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.PortalSettings
{
    public class SchoolSetting : CommonEntity
    {
        [Key]
        public Guid SchoolSettingsId { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolAbbreviation { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
<<<<<<< HEAD
        public string Phone_no1 { get; set; }
        public string Phone_no2 { get; set; }
        public string SchoolType  { get; set; }
=======
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string SchoolType  { get; set; } 
>>>>>>> 97647fbd0bbbad293faa3ca977fa8f360c6c2cd8
    }
}
