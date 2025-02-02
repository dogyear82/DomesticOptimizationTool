using Dot.DataAccess.Repositories;
using Dot.Models;
using Dot.Models.LocalAPI;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Text;

namespace Dot.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepository _repo;

        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger, IHttpClientFactory httpClientFactory, IRepository repo)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _repo = repo;
        }

        public async Task SendMessage(List<LocalChatMessage> chatHistory, string content)
        {
            var httpClient = _httpClientFactory.CreateClient(); 
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:11434/api/chat");
            var messages = new List<LocalChatMessage>
            {
                new LocalChatMessage
                {
                    Role = "system",
                    Content = "Your name is Dot, which stands for Domestic Optimization Tool. You were created by Tan Nguyen, based on the phi4 large language model. You are a helpful AI companion. Your speech style is casual and you answer queries directly.  Your personality style can be overridden by the Personality Override section of a prompt."
                }                
            };
            messages.AddRange(chatHistory);
            messages.Add(new LocalChatMessage
            {
                Role = Role.User,
                Content = content
            });

            var message = new LocalChatRequest
            {
                Model = "phi4",
                Messages = messages
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            var responseContent = string.Empty;
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                responseContent += line;
                if (line is not null)
                {
                    // Forward each line to the SignalR client
                    await Clients.Caller.SendAsync("ReceiveMessage", "Stream", line);
                }
            }

            messages.Add(new LocalChatMessage
            {
                Role = Role.Assistant,
                Content = responseContent
            });

            try
            {
                await _repo.Conversation.AddAsync(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save conversation");
            }
        }
    }
}
