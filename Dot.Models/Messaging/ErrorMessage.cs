namespace Dot.Models.Messaging
{
    public class ErrorMessage : Message<ChatMessage>
    {
        public ErrorMessage(ChatMessage content) : base(content)
        {
            Content = content;
            QueueName = "error";
            RoutingKey = "error";
        }
    }
}
