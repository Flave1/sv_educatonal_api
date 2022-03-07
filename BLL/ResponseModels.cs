using System.Collections.Generic;

namespace BLL
{
    public class APIResponse<T>
    { 
        public bool IsSuccessful { get; set; }
        public List<T> Result { get; set; } = new List<T>();
        public APIResponseMessage Message { get; set; } = new APIResponseMessage();
    } 

    public class APIResponseMessage
    {
        public string Key { get; set; }
        public string FriendlyMessage { get; set; }
        public string TechnicalMessage { get; set; }
    }

    public class ErrorModel
    {
        public string FieldName { get; set; }
        public APIResponse<ErrorModel> Status { get; set; }
    }
}
