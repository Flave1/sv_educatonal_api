using System.Collections.Generic;

namespace BLL
{
    public class APIResponse<T>
    {
        public APIResponse()
        {
            Message = new APIResponseMessage();
        }
        public bool IsSuccessful;
        public T Result { get; set; }
        public APIResponseMessage Message { get; set; } 
    } 

    public class APIResponseMessage
    {
        public string FriendlyMessage { get; set; }
        public string TechnicalMessage { get; set; }
    }

    public class ErrorModel
    {
        public string FieldName { get; set; }
        public APIResponse<ErrorModel> Status { get; set; }
    }
}
