using Dot.Tools.Attributes;
using Dot.Tools.Search.Models;
using Dot.Tools.Search.Models.Brave;
using HtmlAgilityPack;
using HttpUtilities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dot.Tools.Search
{
    [Tool("Search the web for information.")]
    public class WebSearch : ToolBase
    {
        private const string ClientName = "ToolWebClient";

        private readonly ILogger<WebSearch> _logger;
        private readonly IHttpClientAccessor _clientAccessor;
        private readonly IChatClient _chatClient;

        public WebSearch(ILogger<WebSearch> logger, IHttpClientAccessor clientAccessor, IChatClient chatClient)
        {
            _logger = logger;
            _clientAccessor = clientAccessor;
            _chatClient = chatClient;
        }

        public string Execute(
            [ToolParam("A search query string containing the keywords or phrase the user wants to look up on the internet. This should clearly express what the user is trying to find.")] string searchQuery)
        {
            var formattedQuery = Uri.EscapeDataString(searchQuery);
            var result = _clientAccessor.GetAsync<BraveSearchResult>($"/res/v1/web/search?q={formattedQuery}", ClientName).GetAwaiter().GetResult();

            var prompt = ConstructPromptForAiToSelectFromSearchResults(searchQuery, result.Web.Results);
            var response = _chatClient.GetResponseAsync(prompt).GetAwaiter().GetResult();

            var selectedUrl = result.Web.Results.FirstOrDefault(x => x.Title == response.Text).Url;
            _logger.LogInformation($"Selected URL: {selectedUrl}");
            var resultHtml = _clientAccessor.GetStringAsync(selectedUrl).GetAwaiter().GetResult();

            var doc = new HtmlDocument();
            doc.LoadHtml(resultHtml);
            var paragraphs = doc.DocumentNode.SelectNodes("//p");
            var data = string.Join("\n", paragraphs.Select(p => p.InnerText));

            return $"Answer the User's question using the following data: {data}";
        }

        private List<ChatMessage> ConstructPromptForAiToSelectFromSearchResults(string searchQuery, List<BraveWebResult> braveWebResults)
        {
            var searchResults = braveWebResults
                .Select(x => x.Title)
                .ToList();
            var serializedResults = JsonConvert.SerializeObject(searchResults);
            var prompt = $"""You are a search evaluator tool. Given the search query: "{searchQuery}" and the list of search result Titles below, choose the one that best answers the query. ⚠️ Return ONLY the exact Title string (verbatim). Do not explain your choice, do not add anything else. Titles: {serializedResults}""";
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, prompt)
            };
            return messages;
        }
    }
}
