using Microsoft.Extensions.AI;

namespace Dot.Services.Repositories
{
    public class Model
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
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
            return new List<string>();
            //var models = await ((IOllamaApiClient)_client).ListLocalModelsAsync();
            //var modelNames = models.Select(x => x.Name.Split(":")[0]);
            //return modelNames.Order();
        }

        public async Task<IEnumerable<Model>> GetDownloadedModelsAsync()
        {
            return new List<Model>();
            //return await ((IOllamaApiClient)_client).ListLocalModelsAsync();
        }
    }
}
