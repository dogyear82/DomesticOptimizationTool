using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttpUtilities
{
    public interface IHttpClientAccessor
    {
        Task<string> GetStringAsync(string url);
        Task<T> GetAsync<T>(string url, string clientName);
        Task<T> PostAsync<T>(string url, object payload, string clientName);
    }

    public class HttpClientAccessor : IHttpClientAccessor
    {
        private readonly ILogger<HttpClientAccessor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientAccessor(ILogger<HttpClientAccessor> logger, IHttpClientFactory httpClientFactory) 
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        private T Deserialize<T>(string json)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(json);
                if (result == null)
                {
                    throw new Exception("Failed to deserialize JSON");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON");
                throw;
            }
        }

        public async Task<string> GetStringAsync(string url)
        {
            var client = _httpClientFactory.CreateClient();
            var result = await client.GetStringAsync(url);
            return result;

        }

        public async Task<T> GetAsync<T>(string url, string clientName)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().Result;
            return Deserialize<T>(responseContent);
        }

        public async Task<T> PostAsync<T>(string url, object payload, string clientName)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            var content = new StringContent(JsonConvert.SerializeObject(payload));
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return Deserialize<T>(responseContent);
        }
    }
}
