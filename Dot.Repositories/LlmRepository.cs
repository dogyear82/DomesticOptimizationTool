using Dot.Models.Ollama;
using Dot.Services.Ollama;

namespace Dot.Repositories
{
    public interface ILlmRepository
    {
        Task<IEnumerable<string>> GetDownloadedModelNamesAsync();
        Task<IEnumerable<Model>> GetDownloadedModelsAsync();
    }

    public class LlmRepository : ILlmRepository
    {
        private readonly IOllamaClient _client;

        public LlmRepository(IOllamaClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<string>> GetDownloadedModelNamesAsync()
        {
            var tags = await _client.GetTagsAsync();
            var modelNames = tags.Models.Select(x => x.Name.Split(":")[0]);
            return modelNames.Order();
        }

        public async Task<IEnumerable<Model>> GetDownloadedModelsAsync()
        {
            return new List<Model>();
            //return await ((IOllamaApiClient)_client).ListLocalModelsAsync();
        }
    }
}
