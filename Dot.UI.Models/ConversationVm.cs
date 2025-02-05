namespace Dot.UI.Models
{
    public class ConversationVm
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ConversationMessageVm> Messages { get; set; }

    }

    public class  ConversationMessageVm
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
