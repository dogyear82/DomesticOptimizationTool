namespace Dot.Client.Services
{
    public interface IAppSettings
    {
        string ApiUrl { get; }
    }

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