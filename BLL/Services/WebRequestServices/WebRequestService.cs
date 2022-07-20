﻿using Microsoft.Extensions.Options;
using SMP.Contracts.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SMP.BLL.Services.WebRequestServices
{
    public class WebRequestService: IWebRequestService
    {
        private readonly HttpClient client;
        private readonly FwsConfigSeetings fwsOptions;
        public WebRequestService(IHttpClientFactory clientFactory, IOptions<FwsConfigSeetings> options)
        {
            client = clientFactory.CreateClient();
            fwsOptions = options.Value;
        }
   
        public async Task<T> PostAsync<T, Y>(string url, Y data) where Y : class
        {
            var retries = 0;
            var maxRetries = 3;

            do
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Apikey", fwsOptions.Apikey);
                    client.DefaultRequestHeaders.Add("ClientId", fwsOptions.ClientId);

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
                }

                retries++;

            } while (retries < maxRetries);

            throw new HttpRequestException("Failed to post data.");
        }

    }
}
