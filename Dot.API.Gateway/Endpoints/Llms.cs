using Dot.API.Gateway.Services;
using OllamaSharp.Models;

namespace Dot.API.Gateway.Endpoints
{
    public interface ILlms
    {
        Task<IEnumerable<Model>> GetAsync(string id);
        Task<IEnumerable<string>> GetNamesAsync(string id);
    }

    internal class Llms : ILlms
    {
        private readonly IHttpClientAccessor _httpClientAccessor;

        public Llms(IHttpClientAccessor httpClientAccessor)
        {
            _httpClientAccessor = httpClientAccessor;
        }

        public async Task<IEnumerable<Model>> GetAsync(string id)
        {
            return await _httpClientAccessor.GetAsync<IEnumerable<Model>>($"{ApiEndpoints.Llms}");
        }

        public async Task<IEnumerable<string>> GetNamesAsync(string id)
        {
            return await _httpClientAccessor.GetAsync<IEnumerable<string>>($"{ApiEndpoints.LlmNames}");
        }
    }
}
