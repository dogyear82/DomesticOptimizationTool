using Dot.Models.Interfaces;

namespace Dot.Client.Services
{
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _config;

        public string ApiUrl => _config["ApiUrl"];

        public AppSettings(IConfiguration config)
        {
            _config = config;
        }
    }
}