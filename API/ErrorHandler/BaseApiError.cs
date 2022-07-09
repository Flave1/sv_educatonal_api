using Newtonsoft.Json;

namespace App.CustomError
{
    public class BaseApiError
    {
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Friendly_Message { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Technical_Message { get; set; }

        public BaseApiError(int statusCode, string statusCodeDescripton)
        {
            StatusCode = statusCode;
            StatusDescription = statusCodeDescripton;
        }

        public BaseApiError(int statusCode, string statusCodeDescripton, string F_message) : this(statusCode, statusCodeDescripton)
        {
            Friendly_Message = F_message;
        }
        public BaseApiError(int statusCode, string statusCodeDescripton, string F_message, string T_message) : this(statusCode, statusCodeDescripton)
        {
            Friendly_Message = F_message;
            Technical_Message = T_message;
        }
    }
}
