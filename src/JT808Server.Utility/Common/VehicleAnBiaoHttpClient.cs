using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server.Utility.Common
{
    public class VehicleAnBiaoHttpClient
    {
        private readonly HttpClient _httpClient;

        public VehicleAnBiaoHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> HttpRequest(Dictionary<string, string> Para, string Url, bool IsPost)
        {
            FormUrlEncodedContent content = null;
            if (Para != null)
            {
                content = new FormUrlEncodedContent(Para);
            }
            HttpResponseMessage response = (!IsPost) ? (await _httpClient.GetAsync(_httpClient.BaseAddress + Url))
                                                     : (await _httpClient.PostAsync(_httpClient.BaseAddress + Url, content));
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostResponse(string postData, string url)
        {
            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync(url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostResponse(Dictionary<string, string> DictionaryTest, string url)
        {
            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(DictionaryTest));
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync(url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
