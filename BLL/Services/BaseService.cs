using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace SMP.BLL.Services
{
    public static class BaseService
    {
        //public static BaseService()
        //   : base()
        //{

        //}
        public static IHttpContextAccessor config;
       
        public static bool ISForClient { get; set; }
        public static string smsClientId { get; set; }
        public static void Initialize(IServiceProvider services)
        {
            config = (IHttpContextAccessor)services.GetService(typeof(IHttpContextAccessor));
            ISForClient = ISClient(smsClientId);

        }
        public static bool ISClient(string clientID)
        {
            var res = config.HttpContext.Items["smsClientId"].ToString() == clientID;
            return res;
        }
    }
}
