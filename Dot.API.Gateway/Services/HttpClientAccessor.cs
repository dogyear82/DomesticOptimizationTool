using Newtonsoft.Json;

namespace Dot.API.Gateway.Services
{
    public interface IHttpClientAccessor
    {
        Task<T> GetAsync<T>(string uri);
    }

    internal class HttpClientAccessor : IHttpClientAccessor
    {
        private readonly IApiClientFactory _apiClientFactory;
        private HttpClient _httpClient => _apiClientFactory.CreateHttpClient();

        public HttpClientAccessor(IApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
