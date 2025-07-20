using AI.Gateway.API.Models;
using HttpUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Gateway.API
{
    public interface ITextToSpeech
    {
        Task<TtsResponse> GetAsync(TtsRequest request);
    }

    public class TextToSpeech : ITextToSpeech
    {
        private readonly IHttpClientAccessor _httpClientAccessor;

        public TextToSpeech(IServiceProvider serviceProvider) 
        {
            _httpClientAccessor = serviceProvider.GetRequiredService<IHttpClientAccessor>();
        }

        public async Task<TtsResponse> GetAsync(TtsRequest request)
        {
            return new TtsResponse();//return await _httpClientAccessor.PostAsync<TtsResponse>(Environment.GetEnvironmentVariable("ApiUrl"), request);
        }
    }
}
