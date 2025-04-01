using Dot.API.Gateway.Services;
using Dot.Models.Ollama;

namespace Dot.API.Gateway.Endpoints
{
    public interface ILlms
    {
        Task<IEnumerable<Model>> GetAsync();
        Task<IEnumerable<string>> GetNamesAsync();
    }

    internal class Llms : ILlms
    {
        private readonly IHttpClientAccessor _httpClientAccessor;

        public Llms(IHttpClientAccessor httpClientAccessor)
        {
            _httpClientAccessor = httpClientAccessor;
        }

        public async Task<IEnumerable<Model>> GetAsync()
        {
            return await _httpClientAccessor.GetAsync<IEnumerable<Model>>($"{ApiEndpoints.Llms}");
        }

        public async Task<IEnumerable<string>> GetNamesAsync()
        {
            return await _httpClientAccessor.GetAsync<IEnumerable<string>>($"{ApiEndpoints.LlmNames}");
        }
    }
}
