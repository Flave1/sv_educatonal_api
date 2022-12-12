﻿using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Admission
{
    public class AdmissionSetting: CommonEntity
    {
        [Key]
        public Guid AdmissionSettingId { get; set; }
        public string Classes { get; set; }
        public int AdmissionStatus { get; set; }
        public string PassedExamEmail { get; set; }
        public string FailedExamEmail { get; set; }
        public string ScreeningEmail { get; set; }
        public bool RegistrationFee { get; set; }
    }
}
