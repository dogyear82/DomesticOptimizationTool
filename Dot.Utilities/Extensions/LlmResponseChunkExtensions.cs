using OllamaSharp.Models.Chat;

namespace Dot.Utilities.Extensions
{
    public static class ChatResponseStreamExtensions
    {
        public static bool IsBeginningOfThought(this ChatResponseStream chunk)
        {
            return chunk.Message.Content.Contains("<think>");
        }

        public static bool IsEndOfThought(this ChatResponseStream chunk)
        {
            return chunk.Message.Content.Contains("</think>");
        }

        public static bool IsStartOrEndCoding(this ChatResponseStream chunk)
        {
            return chunk.Message.Content.Contains("```");
        }
    }
}
