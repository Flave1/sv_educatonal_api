using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BLL.Helper;

namespace BLL.Helper
{
    public interface IHttpclientHelper
    {
        Task<T> Get<T>(string endpoint);

        Task<T> Get<T>(string endpoint, object id);

        Task<T> Post<T>(string endpoint, object model);

        Task<T> Put<T>(string endpoint, object model);
        Task<T> Put<T>(string endpoint);

        Task<T> Delete<T>(string endpoint, object id);

        Task<HttpResponseMessage> SendRequest(string endpoint, HttpMethod method, object body = null, bool isJson = true);
    }


    public abstract class HttpclientHelper : IHttpclientHelper
    {
        private readonly IHttpClientFactory _httpClient;
       //  private readonly IConfiguration _config;
       // private readonly string baseUrl;

        public abstract Uri BaseUrl { get; }

        public virtual Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public HttpclientHelper(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;          
        }

        public async Task<T> Get<T>(string endpoint, object itemId = null)
        {
            if (itemId != null)
            {
                endpoint += $"/{itemId}";
            }

            return await Get<T>(endpoint);
        }

        public async Task<T> Put<T>(string endpoint)
        {
           var response = await SendRequest(endpoint, HttpMethod.Put);
            return await SendResponse<T>(response);
        }

        public async Task<T> Get<T>(string endpoint)
        {
            var response = await SendRequest(endpoint, HttpMethod.Get);

            return await SendResponse<T>(response);
        }

        public async Task<string> Get(string endpoint)
        {
            var response = await SendRequest(endpoint, HttpMethod.Get);

            return await ReadResponse(response);
        }

        public async Task<T> Post<T>(string endpoint, object model)
        {
            var response = await SendRequest(endpoint, HttpMethod.Post, model);

            return await SendResponse<T>(response);
        }

        public async Task<T> Put<T>(string endpoint, object model)
        {
            var response = await SendRequest(endpoint, HttpMethod.Put, model);

            return await SendResponse<T>(response);
        }

        public async Task<T> Delete<T>(string endpoint, object item)
        {
            if (item != null)
            {
                endpoint += $"/{item}";
            }

            var response = await SendRequest(endpoint, HttpMethod.Delete);

            return await SendResponse<T>(response);
        }

        private async Task<string> ReadResponse(HttpResponseMessage httpResponse)
        {
            var content = await httpResponse.Content.ReadAsStringAsync();
            return content;
        }

        private async Task<T> SendResponse<T>(HttpResponseMessage httpResponse)
      {
            try
            {

                var content = await ReadResponse(httpResponse);
                if (content != null && content.Contains("<"))
                {
                    throw new HttpRequestException();
                }
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<HttpResponseMessage> SendRequest(string endpoint, HttpMethod method ,object body = null, bool isJson = true)
        {
            try
            {
                var apiUrl = new Uri(BaseUrl, endpoint); 
               // var apiUrl = new Uri(baseUrl, endpoint);
                var request = new HttpRequestMessage(method, apiUrl);
                foreach (var item in Headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }

                if (body != null)
                {
                    var json = JsonConvert.SerializeObject(body, JsonSettingHelper.GetJsonSerializer);

                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var client = _httpClient.CreateClient();
                if (isJson)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset-utf-8");
                }

                var response = await client.SendAsync(request);
                client.Dispose();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> GetAsync<T>(string path) where T : class
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(baseURL);
            var response = await client.GetAsync(path);
            if (!response.IsSuccessStatusCode)
                throw new Exception("The request was not successful...");
            string result = response.Content.ReadAsStringAsync().Result;
            T returnValue = JsonConvert.DeserializeObject<T>(result);
            return returnValue;
        }
    }
}
