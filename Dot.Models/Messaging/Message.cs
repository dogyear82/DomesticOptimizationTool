using Dot.Models.Interfaces;

namespace Dot.Models.Messaging
{
    public abstract class Message<T> : IMessage<T>
    {
        public Message(T content)
        {
            Payload = content;
        }

        public string MessageType => GetType().FullName;
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
        public T Payload { get; set; }
    }
}
