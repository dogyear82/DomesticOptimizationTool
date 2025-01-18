using AI.Gateway.API;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Gateway
{
    public interface IAiGateway
    {
        public ITextToSpeech TextToSpeech { get; set; }
    }

    public class AiGateway : IAiGateway
    {
        public ITextToSpeech TextToSpeech { get; set; }

        public AiGateway(IServiceProvider serviceProvider)
        {
            TextToSpeech = serviceProvider.GetRequiredService<ITextToSpeech>();
        }
    }
}
