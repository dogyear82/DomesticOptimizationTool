using Dot.Client.Services;
using Dot.Models;
using Dot.UI.Models;
using Dot.Utilities.Extensions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Dot.API.Gateway;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using OllamaSharp.Models.Chat;

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
                hubConnection = hubAccessor.GetHubConnection();
                hubConnection.On<string, ChatResponseStream>("ReceiveMessage", (user, chunk) =>
                {
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

        protected override async Task OnParametersSetAsync()
        {
            await LoadConversation();
        }

        private async Task LoadConversation()
        {
            ChatEntries.Clear();
            if (conversationId == "0")
            {
                return;
            }

            var conversation = await gateway.Conversations.Get(conversationId);

            var index = 0;
            foreach (var message in conversation.Messages)
            {
                var chatEntry = new ChatEntry
                {
                    Index = index,
                    IsUser = message.Role == ChatRole.User,
                };

                var hasThought = message.Content.Contains("<think>") && !chatEntry.IsUser;
                if (hasThought)
                {
                    try
                    {
                        var splitMessage = message.Content.Split("</think>");
                        var thought = splitMessage[0].Split("<think>");
                        chatEntry.Content = Markdown.ToHtml(splitMessage[1]);
                        chatEntry.Thought = Markdown.ToHtml(thought[1]);
                    }
                    catch
                    {
                        chatEntry.Content = Markdown.ToHtml("<p>There was an error separating thought from speech.</p>");
                    }
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

        private void ProcessChunk(ChatResponseStream chunk)
        {
            if (chunk.Message.Role == ChatRole.System)
            {
                conversationId = chunk.Message.Content;
            }
            else if (chunk.IsBeginningOfThought())
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
                var convoId = conversationId == "0" ? null : conversationId;
                await hubConnection.SendAsync("SendMessage", messageInput, "mistral", convoId);
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
