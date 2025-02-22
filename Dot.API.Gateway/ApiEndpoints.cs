namespace Dot.API.Gateway
{
    internal static class ApiEndpoints
    {
        internal static string NavMenuConversations => "api/navmenu/conversations";
        internal static string Conversations => "api/conversations";
        internal static string Llms => "api/llms";
        internal static string LlmNames => $"{Llms}/names";
    }
}
