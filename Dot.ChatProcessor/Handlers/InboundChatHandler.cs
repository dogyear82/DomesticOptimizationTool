using Dot.DataAccess;
using Dot.DataAccess.Repositories;
using Dot.Models;
using Dot.Models.Messaging;
using Dot.Services.Messaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;

namespace Dot.ChatProcessor.Handlers
{
    public class InboundChatHandler : IMessageHandler
    {
        private readonly IRepository _repository;

        public InboundChatHandler(IServiceProvider sp)
        {
            _repository = sp.GetRequiredService<IRepository>();
        }

        public async Task HandleAsync(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var chatMessage = JsonConvert.DeserializeObject<InboundChatMessage>(message);
            if (chatMessage.Payload.ConversationId is null)
            {
                await _repository.Conversation.AddAsync(new List<ChatMessage> { chatMessage.Payload });
            } 
            else
            {
                await _repository.Conversation.UpdateAsync(new List<ChatMessage> { chatMessage.Payload }, chatMessage.Payload.ConversationId);
            }

            var channel = ((AsyncEventingBasicConsumer)sender).Channel;
            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}
