﻿using Dot.Client.Models;
using Dot.Client.Services;
using Dot.Models;
using Dot.Models.LocalAPI;
using Dot.Utilities.Extensions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace Dot.Client.Pages
{
    public partial class Chat : ComponentBase
    {
        [Inject]
        private IHubAccessor hubAccessor { get; set; }

        private HubConnection? hubConnection;
        public List<ChatEntryVm> ChatEntries { get; set; } = new();
        public List<LocalChatMessage> ChatHistory { get; set; } = new();
        public List<string> messages = new();
        private string? messageInput;

        public bool isThinking = false;
        public string thought = string.Empty;
        public bool isResponseFinished = true;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                hubConnection = hubAccessor.GetHubConnection();

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

                ChatHistory.Add(new LocalChatMessage
                {
                    Role = Role.Assistant,
                    Content = aiResponse
                });

                var chatEntry = new ChatEntryVm
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
                var chatEntry = new ChatEntryVm
                {
                    Index = GetChatEntryIndex(),
                    IsUser = true,
                    Content = messageInput
                };
                ChatHistory.Add(new LocalChatMessage
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
