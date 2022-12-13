using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.AdmissionSettings
{
    public class AdmissionAuth
    {
        public string Token { get; set; }
        public string Expires { get; set; }
    }
    public class AdmissionLoginDetails 
    {
        public AdmissionAuth Auth { get; set; }
        public UserDetails UserDetails { get; set; }
        public AdmissionLoginDetails(AdmissionAuth auth, UserDetails userDetails)
        {
            Auth = auth;
            UserDetails = userDetails;
        }
    }
    public class UserDetails
    {
        public string ParentEmail { get; set; }
        public string AdmissionNotificationId { get; set; }
        public UserDetails(string parentEmail, string admissionNotificationId)
        {
            ParentEmail = parentEmail;
            AdmissionNotificationId = admissionNotificationId;
        }
    }
}
