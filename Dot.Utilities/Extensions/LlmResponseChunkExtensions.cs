using Dot.Models;

namespace Dot.Utilities.Extensions
{
    public static class ChatResponseStreamExtensions
    {
        public static bool IsBeginningOfThought(this ChatStream chunk)
        {
            return chunk.Text.Contains("<think>");
        }

        public static bool IsEndOfThought(this ChatStream chunk)
        {
            return chunk.Text.Contains("</think>");
        }

        public static bool IsStartOrEndCoding(this ChatStream chunk)
        {
            return chunk.Text.Contains("```");
        }
    }
}
