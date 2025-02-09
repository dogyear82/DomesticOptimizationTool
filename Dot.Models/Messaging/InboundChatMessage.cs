namespace Dot.Models.Messaging
{

    public class InboundChatMessage : Message<ChatMessage>
    {
        public InboundChatMessage(ChatMessage content) : base(content)
        {
            Payload = content;
            QueueName = "inboundchat";
            RoutingKey = "chat";
        }
    }
}
