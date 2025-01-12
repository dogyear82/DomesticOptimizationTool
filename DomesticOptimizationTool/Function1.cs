using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DomesticOptimizationTool
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([RabbitMQTrigger("testqueue", ConnectionStringSetting = "MessageBusConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
