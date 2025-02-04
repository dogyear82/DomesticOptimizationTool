namespace Dot.Client.Services
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpclient();
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly IAppSettings _appSettings;

        public HttpClientFactory(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public HttpClient GetHttpclient()
        {
            return new HttpClient { BaseAddress = new Uri($"{_appSettings.ApiUrl}") };
        }
    }
}
