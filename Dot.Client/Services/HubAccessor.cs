using Microsoft.AspNetCore.SignalR.Client;

namespace Dot.Client.Services
{
    public interface IHubAccessor
    {
        HubConnection GetHubConnection();
    }
        
    
    public class HubAccessor : IHubAccessor
    {
        private readonly IAppSettings _appSettings;

        public HubAccessor(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public HubConnection GetHubConnection()
        {
            return new HubConnectionBuilder()
                .WithUrl(new Uri($"{_appSettings.ApiUrl}/chathub"))
                .Build();
        }
    }
}