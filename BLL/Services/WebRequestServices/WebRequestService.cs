using BLL.LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SMP.Contracts.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SMP.BLL.Services.WebRequestServices
{
    public class WebRequestService: IWebRequestService
    {
        private readonly HttpClient client;
        private readonly ILoggerService loggerService;
        private readonly FwsConfigSettings fwsOptions;
        private readonly string smsClientId;
        public WebRequestService(HttpClient httpClient, IHttpContextAccessor accessor, IOptions<FwsConfigSettings> options, ILoggerService loggerService)
        {
            //client = clientFactory.CreateClient();
            client = httpClient;
            this.loggerService = loggerService;
            fwsOptions = options.Value;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }
   
        public async Task<T> PostAsync<T, Y>(string url, Y data, Dictionary<string, string> credentials) where Y : class
        {
            var retries = 0;
            var maxRetries = 3;

            do
            {
                try
                {

                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactoryExample");
                    client.DefaultRequestHeaders.Add("Apikey", fwsOptions.Apikey);
                    client.DefaultRequestHeaders.Add("ClientId", smsClientId);
                    if (credentials is not null)
                    {
                        foreach (var pair in credentials)
                        {
                            if (client.DefaultRequestHeaders.Contains(pair.Key))
                                client.DefaultRequestHeaders.Remove(pair.Key);
                            client.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                        }
                    }
                    var serializeOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    };
                    var content = new StringContent(JsonSerializer.Serialize(data, serializeOptions), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<T>();
                        return result;
                    }
                    else
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result);
                    }
                }
                catch (HttpRequestException)
                {
                    if (retries >= (maxRetries - 1))
                    {
                        //log error

                        throw;
                    }
                }catch(Exception ex)
                {
                    loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                    throw new ArgumentException(ex.Message ?? ex.InnerException.Message);
                }

                retries++;

            } while (retries < maxRetries);

            throw new HttpRequestException("Failed to post data.");
        }

        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> credentials)
        {
            var retries = 0;
            var maxRetries = 3;

            do
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactoryExample");
                    client.DefaultRequestHeaders.Add("Apikey", fwsOptions.Apikey);
                    client.DefaultRequestHeaders.Add("ClientId", fwsOptions.ClientId);
                    if (credentials is not null)
                    {
                        foreach (var pair in credentials)
                        {
                            if (client.DefaultRequestHeaders.Contains(pair.Key))
                                client.DefaultRequestHeaders.Remove(pair.Key);
                            client.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                        }
                    }

                    var serializeOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    };
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<T>();
                        return result;
                    }
                    else
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result);
                    }
                }
                catch (HttpRequestException ex)
                {
                    if (retries >= (maxRetries - 1))
                    {
                        //log error
                        loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                        throw;
                    }
                }

                retries++;

            } while (retries < maxRetries);

            throw new HttpRequestException("Failed to post data.");
        }

    }
}
