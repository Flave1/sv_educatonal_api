using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BLL.Helper
{
    public class JsonSettingHelper
    {
        public static JsonSerializerSettings GetJsonSerializer
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy().ProcessDictionaryKeys));

                return settings;
            }
        }
    }
}
