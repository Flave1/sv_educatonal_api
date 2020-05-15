using System;
using System.Collections.Generic;

namespace App.Contracts.ErrorResponses
{
    public class ErrorModel
    {
        public string FieldName { get; set; }
        public string Message { get; set; }
    }
}
