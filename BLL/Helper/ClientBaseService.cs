using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace BLL.Helper
{
    public interface IClientBaseService : IHttpclientHelper
    {
    }

    public class ClientBaseService : HttpclientHelper, IClientBaseService
    {
        private readonly IConfiguration _config;

        public ClientBaseService(IConfiguration config, IHttpClientFactory httpClient) : base(httpClient)
        {
            _config = config;
             
            base.Headers.Add("apikey", _config["Config:apikey"]);
            base.Headers.Add("clientId", _config["Config:clientId"]);
        }

        public override Uri BaseUrl => new Uri(_config["Config:BaseUrl"]);
    }
}
