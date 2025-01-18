using AI.Gateway.API;
using AI.Gateway;
using AI.Gateway.Extensions;
using Dot.Modules;
using HttpUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults() // Configures Azure Functions worker
    .ConfigureAppConfiguration((context, config) =>
    {
        // Load app settings from 'local.settings.json' in development
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables(); // Load environment variables
    })
    .ConfigureServices(services =>
    {
        // Register additional services for dependency injection here
        services.AddSingleton<IVoice, Voice>();
        services.AddSingleton<IHttpClientAccessor, HttpClientAccessor>();
        services.AddSingleton<ITextToSpeech, TextToSpeech>();
        services.AddSingleton<IAiGateway, AiGateway>();

        services.AddHttpClient("1MinAI", (provider, client) =>
        {
            var configuration = provider.GetService<IConfiguration>();
            client.BaseAddress = new Uri(configuration["ApiUrl"]);
            client.DefaultRequestHeaders.Add("API-KEY", $"{configuration["ApiKey"]}");
        });
    })
    .Build();

host.Run();
