using Dot.Models.Interfaces;

namespace Dot.API.Gateway.Services
{
    internal interface IApiClientFactory
    {
        HttpClient CreateHttpClient();
    }

    internal class ApiClientFactory : IApiClientFactory
    {
        private readonly IAppSettings _appSettings;

        public ApiClientFactory(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public HttpClient CreateHttpClient()
        {
            return new HttpClient { BaseAddress = new Uri($"{_appSettings.ApiUrl}") };
        }
    }
}
