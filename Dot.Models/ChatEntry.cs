namespace Dot.Models
{
    public class ChatEntry
    {
        public int Index { get; set; }
        public bool IsUser { get; set; }
        public string Content { get; set; }
    }

    public class DeepseekChatEntry : ChatEntry
    {
        public string Thought { get; set; }
    }
}
