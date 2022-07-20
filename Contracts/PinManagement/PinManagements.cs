using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PinManagement
{
    public class UploadedPins
    {
        public Guid UploadedPinId { get; set; }
        public Guid Pin { get; set; } 
        public string RegistractionNumber { get; set; }
        public Guid StudentContactId { get; set; }
        public Guid SessionClassid { get; set; }
        public Guid TermId { get; set; }
    }
}
