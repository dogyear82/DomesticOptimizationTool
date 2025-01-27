using AI.Gateway.LocalAPI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace Dot.UI.Client.Pages
{
    public partial class Chat : IAsyncDisposable
    {
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private HubConnection? hubConnection;
        public List<string> messages = new();
        private string? userInput;
        private string? messageInput;

        public bool isThinking = false;
        public string thought = string.Empty;
        public bool isResponseFinished = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                hubConnection = new HubConnectionBuilder()
                            .WithUrl(new Uri("https://localhost:7042/chathub"))
                            .Build();

                hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
                {
                    var chunk = JsonConvert.DeserializeObject<LocalChatResponseChunk>(message);
                    ProcessChunk(chunk);
                    InvokeAsync(StateHasChanged);
                });

                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ProcessChunk(LocalChatResponseChunk chunk)
        {
            var content = chunk.Message.Content;
            if (content.Contains("<think>"))
            {
                isThinking = true;
            }
            else if (content.Contains("</think>"))
            {
                isThinking = false;
            }
            else if (isThinking)
            {
                thought += content;
            }
            else if (content.Contains("\n"))
            {
                var lineBreak = content.Replace("\n", "<br />");
                if (isThinking)
                {
                    thought += lineBreak;
                }
                else
                {
                    messages.Add(lineBreak);
                }
            }
            else if (chunk.Done)
            {
                messages.Add($"<p class=\"thought\">{thought}</p>");
                thought = string.Empty;
            }
            else
            {
                messages.Add(chunk.Message.Content);
                isResponseFinished = chunk.Done;
            }
        }

        private async Task Send()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendMessage", userInput, messageInput);
            }
        }

        public bool IsConnected =>
            hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }
    }
}
