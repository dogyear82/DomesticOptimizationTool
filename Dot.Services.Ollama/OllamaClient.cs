using Microsoft.Extensions.AI;
using System.Text;
using Dot.Models.Ollama;
using Newtonsoft.Json;

namespace Dot.Services.Ollama
{
    public interface IOllamaClient
    {
        Task<TagResponse> GetTagsAsync();
    }

    public class OllamaClient : IOllamaClient, IChatClient
    {
        private readonly HttpClient _httpClient;

        public OllamaClient(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("Ollama");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<TagResponse> GetTagsAsync()
        {
            var response = await _httpClient.GetAsync(Endpoints.Tags);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TagResponse>(content);
        }

        public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new ChatRequest
            {
                Model = options?.ModelId ?? "mistral",
                Stream = false, // 👈 non-streaming
                Messages = messages.Select(x => new Message(x)).ToList()
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(
                Endpoints.Chat,
                requestContent,
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var ollamaResponse = JsonConvert.DeserializeObject<OllamaResponse>(responseContent);

            return new ChatResponse
            {
                Messages = new List<ChatMessage> {
                    new ChatMessage(new ChatRole(ollamaResponse.Message.Role), ollamaResponse.Message.Content)
                }
            };
        }

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new ChatRequest
            {
                Model = options?.ModelId ?? "mistral",
                Stream = true,
                Messages = messages.Select(x => new Message(x)).ToList()
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(
                Endpoints.Chat,
                requestContent
            );

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                var ollamaResponse = JsonConvert.DeserializeObject<OllamaResponse>(line);
                yield return new ChatResponseUpdate(new ChatRole(ollamaResponse.Message.Role), ollamaResponse.Message.Content);
            }
        }
    }
}
