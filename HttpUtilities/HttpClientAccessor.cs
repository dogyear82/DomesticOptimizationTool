using Newtonsoft.Json;

namespace HttpUtilities
{
    public interface IHttpClientAccessor
    {
        Task<T> PostAsync<T>(string url, object request);
    }

    public class HttpClientAccessor : IHttpClientAccessor
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientAccessor(IHttpClientFactory httpClientFactory) 
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> PostAsync<T>(string url, object request)
        {
            var client = _httpClientFactory.CreateClient("1MinAI");
            var content = new StringContent(JsonConvert.SerializeObject(request));
            HttpResponseMessage response = await client.PostAsync("/features", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed http call");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}
