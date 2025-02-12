namespace Dot.UI.Models
{
    public class ChatEntry
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public bool IsUser { get; set; }
        public string Content { get; set; }
        public string Thought { get; set; }
        public bool IsShowThought { get; set; }
    }
}
