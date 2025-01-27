using AI.Gateway.API;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Gateway
{
    public interface ILocalGateway
    {
        public ITextToSpeech TextToSpeech { get; set; }
    }

    public class LocalGateway : ILocalGateway
    {
        public ITextToSpeech TextToSpeech { get; set; }

        public LocalGateway(IServiceProvider serviceProvider)
        {
            TextToSpeech = serviceProvider.GetRequiredService<ITextToSpeech>();
        }
    }
}
