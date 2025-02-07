namespace Dot.Models.Interfaces
{
    public interface IMessage<T>
    {
        string MessageType { get; }
        string QueueName { get; }
        string RoutingKey { get; }
        T Content { get; set; }
    }
}
