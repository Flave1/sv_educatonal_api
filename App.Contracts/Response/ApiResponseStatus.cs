using System;
using System.Collections.Generic;

namespace App.Contracts.Response
{
    public class APIResponseStatus
    {
        public bool IsSuccessful { get; set; } = false;
        public string CustomToken { get; set; }
        public string CustomSetting { get; set; }
        public APIResponseMessage Message { get; set; }
    }
}
