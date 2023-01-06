using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Routes
{
    public class fwsRoutes
    {
        public const string validatePin = "fws/client/fws/sms/api/v1/validate-pin";
        public const string validateMultiPins = "fws/client/fws/sms/api/v1/validate-multiple-pins";
        public const string validateMultiPinsOnUpload = "fws/client/fws/sms/api/v1/validate-multiple-pins/on-upload";
        public const string countrySelect = "fws/client/fws/lookups/api/v1/get/country-select";
        public const string stateSelect = "fws/client/fws/lookups/api/v1/get/state-select?country=";
        public const string citySelect = "fws/client/fws/lookups/api/v1/get/city-select?state=";
        public const string clientInformation = "fws/client/fws/sms/api/v1/get/sms-client/information?";
    }
}
