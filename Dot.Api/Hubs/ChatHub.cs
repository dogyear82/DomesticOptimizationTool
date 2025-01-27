using AI.Gateway.LocalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Text;

namespace Dot.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task SendMessage(string user, string content)
        {
            var httpClient = _httpClientFactory.CreateClient(); 
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:11434/api/chat");

            var message = new LocalChatRequest
            {
                Model = "deepseek-r1",
                Messages = new List<LocalChatMessage>
                {
                    new LocalChatMessage
                    {
                        Role = "user",
                        Content = content
                    }
                }
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line is not null)
                {
                    // Forward each line to the SignalR client
                    await Clients.Caller.SendAsync("ReceiveMessage", "Stream", line);
                }
            }
        }
    }
}
