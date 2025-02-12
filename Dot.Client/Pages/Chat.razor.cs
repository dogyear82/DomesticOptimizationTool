using Dot.Client.Services;
using Dot.Models;
using Dot.UI.Models;
using Dot.Utilities.Extensions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Dot.API.Gateway;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

namespace Dot.Client.Pages
{
    public partial class Chat : ComponentBase
    {
        [Inject]
        private IHubAccessor hubAccessor { get; set; }
        [Inject]
        private IGateway gateway { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }

        [Parameter]
        public string conversationId { get; set; }

        private HubConnection? hubConnection;
        public List<ChatEntry> ChatEntries { get; set; } = new();
        public List<string> messageStream = new();
        private string? messageInput;

        public bool isBusy = false;
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

            var index = 0;
            foreach (var message in conversation.Messages)
            {
                var chatEntry = new ChatEntry
                {
                    Index = index,
                    IsUser = message.Role == Role.User,
                };

                var hasThought = message.Content.Contains("<think>") && !chatEntry.IsUser;
                if (hasThought)
                {
                    var splitMessage = message.Content.Split("</think>");
                    var thought = splitMessage[0].Split("<think>");
                    chatEntry.Content = Markdown.ToHtml(splitMessage[1]);
                    chatEntry.Thought = Markdown.ToHtml(thought[1]);
                }
                else
                {
                    chatEntry.Content = Markdown.ToHtml(message.Content);
                }

                ChatEntries.Add(chatEntry);
                index++;
            }

            await ScrollToBottom();
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
                isBusy = false;
                var aiResponse = "";
                foreach (var message in messageStream)
                {
                    aiResponse += message;
                }

                messageStream.Clear();

                var chatEntry = new ChatEntry
                {
                    Index = GetChatEntryIndex(),
                    IsUser = false,
                    Content = Markdown.ToHtml(aiResponse),
                    Thought = Markdown.ToHtml(thought)
                };
                ChatEntries.Add(chatEntry);
                thought = string.Empty;
                ScrollToBottom().Wait();
            }
            else
            {
                messageStream.Add(chunk.Message.Content);
                isResponseFinished = chunk.Done;
            }
        }

        private async Task SendOnEnter(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await Send();
            }
        }

        private async Task Send()
        {
            if (hubConnection is not null && !string.IsNullOrWhiteSpace(messageInput))
            {
                await hubConnection.SendAsync("SendMessage", messageInput, conversationId);
                var chatEntry = new ChatEntry
                {
                    Index = GetChatEntryIndex(),
                    IsUser = true,
                    Content = Markdown.ToHtml(messageInput)
                };
                ChatEntries.Add(chatEntry);
                messageInput = string.Empty;
                isBusy = true;
                await ScrollToBottom();
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

        private async Task ScrollToBottom()
        {
            await js.InvokeVoidAsync("scrollToLastChatBubble");
        }
    }
}
