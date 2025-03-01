using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models;

namespace Dot.Services.Repositories
{
    public interface ILlmRepository
    {
        Task<IEnumerable<string>> GetDownloadedModelNamesAsync();
        Task<IEnumerable<Model>> GetDownloadedModelsAsync();
    }

    public class LlmRepository : ILlmRepository
    {
        private readonly IChatClient _client;

        public LlmRepository(IChatClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<string>> GetDownloadedModelNamesAsync()
        {
            var models = await ((IOllamaApiClient)_client).ListLocalModelsAsync();
            return models.Select(x => x.Name.Split(":")[0]);
        }

        public async Task<IEnumerable<Model>> GetDownloadedModelsAsync()
        {
            return await ((IOllamaApiClient)_client).ListLocalModelsAsync();
        }
    }
}
