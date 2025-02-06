using Dot.API.Gateway.Extensions;
using Dot.Client;
using Dot.Client.Extensions;
using Dot.Client.Services;
using Dot.Models.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.LoadRemoteConfigAsync();

// Register API base URL in DI
builder.Services.AddSingleton<IHubAccessor, HubAccessor>();
builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddAPIGateway();


await builder.Build().RunAsync();
