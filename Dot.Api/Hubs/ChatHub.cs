using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Dot.Api.Hubs
{
    public class ChatHub : Hub
    {

        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }
        public async Task SendMessage(string user, string message)
        {
            var response = $"Chatbot: You said '{message}'";
            await Clients.All.SendAsync("ReceiveMessage", user, response);
        }
    }
}
