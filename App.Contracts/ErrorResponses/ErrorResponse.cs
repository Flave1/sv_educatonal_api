using System;
using System.Collections.Generic;

namespace App.Contracts.ErrorResponses
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

    }
}
