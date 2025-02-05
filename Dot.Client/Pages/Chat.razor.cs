using Dot.Client.Services;
using Dot.Models;
using Dot.UI.Models;
using Dot.Models.LocalAPI;
using Dot.Utilities.Extensions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Dot.API.Gateway;

namespace Dot.Client.Pages
{
    public partial class Chat : ComponentBase
    {
        [Inject]
        private IHubAccessor hubAccessor { get; set; }
        [Inject]
        private IGateway gateway { get; set; }

        [Parameter]
        public string conversationId { get; set; }

        private HubConnection? hubConnection;
        public List<ChatEntry> ChatEntries { get; set; } = new();
        public List<ChatMessage> ChatHistory { get; set; } = new();
        public List<string> messages = new();
        private string? messageInput;

        public bool isThinking = false;
        public string thought = string.Empty;
        public bool isResponseFinished = true;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadConversation();
                hubConnection = hubAccessor.GetHubConnection();

                hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
                {
                    var chunk = JsonConvert.DeserializeObject<LlmResponseChunk>(message);
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

        private async Task LoadConversation()
        {
            var conversation = await gateway.Conversations.Get(conversationId);

            ChatEntries = conversation.Messages.Select((x, i) => new ChatEntry
            {
                Index = i,
                IsUser = x.Role == Role.User,
                Content = x.Content
            }).ToList();
        }

        private void ProcessChunk(LlmResponseChunk chunk)
        {
            if (chunk.IsBeginningOfThought())
            {
                isThinking = true;
            }
            else if (chunk.IsEndOfThought())
            {
                isThinking = false;
            }
            else if (isThinking)
            {
                thought += chunk.Message.Content;
            }
            else if (chunk.Done)
            {
                var aiResponse = "";
                foreach (var message in messages)
                {
                    aiResponse += message;
                }

                messages.Clear();

                ChatHistory.Add(new ChatMessage
                {
                    Role = Role.Assistant,
                    Content = aiResponse
                });

                var chatEntry = new ChatEntry
                {
                    Index = GetChatEntryIndex(),
                    IsUser = false,
                    Content = Markdown.ToHtml(aiResponse),
                    Thought = Markdown.ToHtml(thought)
                };
                ChatEntries.Add(chatEntry);
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
            if (hubConnection is not null && !string.IsNullOrWhiteSpace(messageInput))
            {
                await hubConnection.SendAsync("SendMessage", ChatHistory, messageInput);
                var chatEntry = new ChatEntry
                {
                    Index = GetChatEntryIndex(),
                    IsUser = true,
                    Content = messageInput
                };
                ChatHistory.Add(new ChatMessage
                {
                    Role = Role.User,
                    Content = messageInput
                });
                ChatEntries.Add(chatEntry);
                messageInput = string.Empty;
            }
        }

        private int GetChatEntryIndex()
        {
            return ChatEntries.Any() ? ChatEntries.Max(x => x.Index) + 1 : 0;
        }

        private void ToggleThoughtVisibility(int index)
        {
            var entry = ChatEntries.SingleOrDefault(x => x.Index == index);
            entry.IsShowThought = !entry.IsShowThought;
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
