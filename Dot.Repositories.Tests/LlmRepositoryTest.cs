using Dot.Models.Ollama;
using Dot.Services.Ollama;
using Moq;

namespace Dot.Repositories.Tests
{
    public class LlmRepositoryTests
    {
        private readonly ILlmRepository _llmRepository;
        private readonly Mock<IOllamaClient> _ollamaClient;

        public LlmRepositoryTests()
        {
            _ollamaClient = new Mock<IOllamaClient>();
            _llmRepository = new LlmRepository(_ollamaClient.Object);
        }

        [Fact]
        public async Task Test1()
        {
            var expectedModelName = "expectedmodel";
            _ollamaClient.Setup(x => x.GetTagsAsync()).ReturnsAsync(
                new TagResponse
            {
                Models = new List<Model>
                {
                    new Model
                    {
                        Name = $"{expectedModelName}:v1"
                    }
                }
            });

            var models = await _llmRepository.GetDownloadedModelNamesAsync();

            Assert.Single(models);
            Assert.Equal(expectedModelName, models.First());
        }
    }
}
