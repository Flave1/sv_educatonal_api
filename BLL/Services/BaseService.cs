using Microsoft.AspNetCore.Http;
using System;

namespace SMP.BLL.Services
{
    public class BaseService
    {
        public BaseService()
           : base()
        {

        }
        private readonly IHttpContextAccessor contextAccessor;
        private readonly Func<string, bool> clientFilter = null;
        //_clientFilter = (accessor.HttpContext.Items["smsClientId"].ToString() => "kmaklda");
        public BaseService(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }
    }
}
