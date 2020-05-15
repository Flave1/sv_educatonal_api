using App.Contracts.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.DomainObjects.Auth
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string RefreshToken { get; set; }
        public bool IsFirstLogin { get; set; }
        public string Email { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
