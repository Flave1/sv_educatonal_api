using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Examination
{
    public class Setting
    {
        [Key]
        public Guid SettingId { get; set; }
        public bool NotifyByEmail { get; set; }
        public bool NotifyBySMS { get; set; }
        public bool ShowPreviousBtn { get; set; }
        public bool ShowPreviewBtn { get; set; }
        public bool UseWebCamCapture { get; set; }
        public bool SubmitExamWhenUserLeavesScreen { get; set; }
    }
}
