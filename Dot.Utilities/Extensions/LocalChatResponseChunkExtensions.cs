using Dot.Models.LocalAPI;

namespace Dot.Utilities.Extensions
{
    public static class LocalChatResponseChunkExtensions
    {
        public static bool IsBeginningOfThought(this LocalChatResponseChunk chunk)
        {
            return chunk.Message.Content.Contains("<think>");
        }

        public static bool IsEndOfThought(this LocalChatResponseChunk chunk)
        {
            return chunk.Message.Content.Contains("</think>");
        }

        public static bool IsLineBreak(this LocalChatResponseChunk chunk)
        {
            return chunk.Message.Content.Contains("\n");
        }

        public static string GetMessageContent(this LocalChatResponseChunk chunk)
        {
            return chunk.Message.Content;
        }
    }
}
