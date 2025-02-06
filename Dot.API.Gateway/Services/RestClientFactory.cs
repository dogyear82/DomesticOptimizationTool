using Dot.Models.Interfaces;

namespace Dot.API.Gateway.Services
{
    internal interface IRestClientFactory
    {
        HttpClient CreateHttpClient();
    }

    internal class RestClientFactory : IRestClientFactory
    {
        private readonly IAppSettings _appSettings;

        public RestClientFactory(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public HttpClient CreateHttpClient()
        {
            return new HttpClient { BaseAddress = new Uri($"{_appSettings.ApiUrl}") };
        }
    }
}
