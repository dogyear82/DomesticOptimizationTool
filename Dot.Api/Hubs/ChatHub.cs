using Dot.API.Models;
using Dot.DataAccess.Repositories;
using Dot.Models;
using Dot.Models.LocalAPI;
using Dot.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Text;

namespace Dot.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepository _repo;
        private readonly IAppSettings<ApiSettings> _appSettings;

        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger, IHttpClientFactory httpClientFactory,IServiceProvider sp)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _repo = sp.GetRequiredService<IRepository>();
            _appSettings = sp.GetRequiredService<IAppSettings<ApiSettings>>();
        }

        public async Task SendMessage(string content, string conversationId = null)
        {
            var httpClient = _httpClientFactory.CreateClient(); 
            var request = new HttpRequestMessage(HttpMethod.Post, (await _appSettings.GetAsync()).InferenceApiUrl);
            var messages = new List<ChatMessage>
            {
                await GetSystemPrompt()
            };
            if (conversationId is not null)
            {
                messages.AddRange(await GetConversationHistory(conversationId));
            }

            var userMessage = CreateUserMessage(content);
            messages.Add(userMessage);

            var prompt = new Prompt
            {
                Model = "phi4",
                Messages = messages
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(prompt), Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            var responseContent = "[";
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                responseContent += $"{line},";
                if (line is not null)
                {
                    // Forward each line to the SignalR client
                    await Clients.Caller.SendAsync("ReceiveMessage", "Stream", line);
                }
            }
            responseContent += "]";

            var llmResponseMessage = new ChatMessage
            {
                Role = Role.Assistant,
                Content = ConstructResponseContent(responseContent)
            };
            messages.Add(llmResponseMessage);

            try
            {
                var messagesToAdd = new List<ChatMessage>
                {
                    userMessage,
                    llmResponseMessage
                };
                await _repo.Conversation.UpdateAsync(messagesToAdd, conversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save conversation");
            }
        }

        private async Task<ChatMessage> GetSystemPrompt()
        {
            return new ChatMessage
            {
                Role = Role.System,
                Content = string.Join(" ", (await _appSettings.GetAsync()).SystemPrompts)
            };
        }

        private async Task<List<ChatMessage>> GetConversationHistory(string conversationId)
        {
            var conversation = await _repo.Conversation.GetConversationById(conversationId);
            return conversation.Messages
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.CreatedBy != Role.System)
                    .Select(x => new ChatMessage
                    {
                        Role = x.CreatedBy,
                        Content = x.Content
                    }).ToList();
        }

        private ChatMessage CreateUserMessage(string content)
        {
            return new ChatMessage
            {
                Role = Role.User,
                Content = content
            };
        }

        private string ConstructResponseContent(string serializedChunks)
        {
            var chunks = JsonConvert.DeserializeObject<List<LlmResponseChunk>>(serializedChunks);
            return string.Join("", chunks.Select(x => x.Message.Content));
        }
    }
}
