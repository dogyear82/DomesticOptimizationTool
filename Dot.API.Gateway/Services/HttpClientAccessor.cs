using Newtonsoft.Json;

namespace Dot.API.Gateway.Services
{
    public interface IHttpClientAccessor
    {
        Task<T> GetAsync<T>(string uri);
    }

    internal class HttpClientAccessor : IHttpClientAccessor
    {
        private readonly IRestClientFactory _apiClientFactory;

        public HttpClientAccessor(IRestClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        private HttpClient GetHttpClient()
        {
            return _apiClientFactory.CreateHttpClient();
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var client = GetHttpClient();
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
