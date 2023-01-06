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
        public const string countrySelect = "fws/lookups/api/v1/get/country-select";
        public const string stateSelect = "fws/lookups/api/v1/get/state-select?country=";
        public const string citySelect = "fws/lookups/api/v1/get/city-select?state=";
        public const string createCbtCandidate = "cbt/api/v1/admission/create-candidates";
        public const string clientInformation = "fws/sms/api/v1/get/sms-client/information?";
    }
}
