using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DomesticOptimizationTool
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public async Task Run([RabbitMQTrigger("testqueue", ConnectionStringSetting = "MessageBusConnection")] string myQueueItem)
        {
        }
    }
}
