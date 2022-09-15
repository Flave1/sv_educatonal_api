using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Routes
{
    public class fwsRoutes
    {
        public const string validatePin = "fws/sms/api/v1/validate-pin";
        public const string validateMultiPins = "fws/sms/api/v1/validate-multiple-pins";
        public const string validateMultiPinsOnUpload = "fws/sms/api/v1/validate-multiple-pins/on-upload";
    }
}
