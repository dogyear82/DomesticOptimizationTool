using Dot.Models.Interfaces;
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
                .WithUrl("http://localhost:8080/chathub")
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();
        }
    }
}