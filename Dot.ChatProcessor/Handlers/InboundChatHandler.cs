using Dot.Repositories;
using Dot.Models.Messaging;
using Dot.Services.Messaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.AI;

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
            if ("REPLACE WITH CONVERSATION ID" is null)
            {
                await _repository.Conversation.AddAsync(new List<ChatMessage> { chatMessage.Payload }, "MODEL NAME");
            } 
            else
            {
                await _repository.Conversation.UpdateAsync(new List<ChatMessage> { chatMessage.Payload }, "REPLACE WITH CONVERSATION ID", "MODEL NAME");
            }

            var channel = ((AsyncEventingBasicConsumer)sender).Channel;
            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}
