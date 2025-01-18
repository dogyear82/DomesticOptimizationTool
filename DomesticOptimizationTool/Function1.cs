using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dot
{
    public class Function1
    {
        [Function("Function1")]
        public async Task RunAsync([RabbitMQTrigger("testqueue", ConnectionStringSetting = "MessageBusConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
