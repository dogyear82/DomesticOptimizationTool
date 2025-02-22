using OllamaSharp;
using OllamaSharp.Models;

namespace Dot.Services.Repositories
{
    public interface IModelsRepository
    {
        Task<IEnumerable<string>> GetDownloadedModelNamesAsync();
        Task<IEnumerable<Model>> GetDownloadedModelsAsync();
    }

    public class ModelsRepository : IModelsRepository
    {
        private readonly IOllamaApiClient _client;

        public ModelsRepository(IOllamaApiClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<string>> GetDownloadedModelNamesAsync()
        {
            var models = await _client.ListLocalModelsAsync();
            return models.Select(x => x.Name.Split(":")[0]);
        }

        public async Task<IEnumerable<Model>> GetDownloadedModelsAsync()
        {
            return await _client.ListLocalModelsAsync();
        }
    }
}
