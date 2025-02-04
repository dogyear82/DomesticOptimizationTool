using Dot.Client;
using Dot.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
builder.Services.AddScoped(sp => httpClient);

// Load appsettings.json
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

// Register API base URL in DI
builder.Services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
builder.Services.AddSingleton<IHubAccessor, HubAccessor>();
builder.Services.AddSingleton<IAppSettings, AppSettings>();

await builder.Build().RunAsync();
