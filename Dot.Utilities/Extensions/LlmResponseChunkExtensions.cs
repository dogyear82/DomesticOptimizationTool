using Dot.Models;

namespace Dot.Utilities.Extensions
{
    public static class LlmResponseChunkExtensions
    {
        public static bool IsBeginningOfThought(this LlmResponseChunk chunk)
        {
            return chunk.Message.Content.Contains("<think>");
        }

        public static bool IsEndOfThought(this LlmResponseChunk chunk)
        {
            return chunk.Message.Content.Contains("</think>");
        }

        public static bool IsStartOrEndCoding(this LlmResponseChunk chunk)
        {
            return chunk.Message.Content.Contains("```");
        }
    }
}
