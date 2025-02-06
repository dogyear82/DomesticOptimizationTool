using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;

namespace Dot.Client.Extensions
{
    internal static class HostBuilderExtensions
    {
        internal static async Task LoadRemoteConfigAsync(this WebAssemblyHostBuilder builder)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
            var configResponse = await httpClient.GetAsync("appsettings.json");
            if (configResponse.IsSuccessStatusCode)
            {
                var config = await configResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (config is not null)
                {
                    foreach (var kvp in config)
                    {
                        builder.Configuration[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
    }
}
