namespace SMP.Contracts.Routes
{
    public class fwsRoutes
    {
        public const string RouteName = "fws/client/";
        public const string validatePin = RouteName + "fws/sms/api/v1/validate-pin";
        public const string validateMultiPins = RouteName + "fws/sms/api/v1/validate-multiple-pins";
        public const string validateMultiPinsOnUpload = RouteName + "fws/sms/api/v1/validate-multiple-pins/on-upload";
        public const string countrySelect = RouteName + "fws/lookups/api/v1/get/country-select";
        public const string stateSelect = RouteName + "fws/lookups/api/v1/get/state-select?country=";
        public const string citySelect = RouteName + "fws/lookups/api/v1/get/city-select?state=";
        public const string clientInformation = RouteName + "fws/sms/api/v1/get/sms-client/information?";
        public const string createUser = RouteName + "fws/usermanager/api/v1/create-user";
        public const string updateUser = RouteName + "fws/usermanager/api/v1/update-user";
    }
}
