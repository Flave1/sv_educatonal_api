using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Routes
{
    public class cbtRoutes
    {
        public const string RouteName = "cbt/client/";
        public const string createCbtCandidate = RouteName + "cbt/api/v1/admission/create-candidates";
        public const string getClassCBTs = RouteName + "cbt/api/v1/smpexams/get-all-examination/by-sessionclass";
        public const string getToken = RouteName + "cbt/api/v1/smpauth/login/by-hash";
        public const string studentResult = RouteName + "cbt/api/v1/smpexams/get-result";
        public const string getCbtResult = RouteName + "cbt/api/v1/admission/get-candidates-result";
    }
}
