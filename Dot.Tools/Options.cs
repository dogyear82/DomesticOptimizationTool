
namespace Dot.Tools
{
    public class HttpClientOptions
    {
        public string BaseAddress { get; set; } = string.Empty;
        public int Timeout { get; set; } = 100;
        public Dictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();
    }
}
