using Dot.Models.Interfaces;
using RabbitMQ.Client.Events;

namespace Dot.Services.Messaging.Interfaces
{
    public interface IMessageSender
    {
        Task Send<T>(IMessage<T> message);
    }

    public interface IMessageProcessor
    {
        Task Start(CancellationToken stoppingToken);
    }

    public interface IMessageHandler
    {
        Task HandleAsync(object sender, BasicDeliverEventArgs ea);
    }
}
