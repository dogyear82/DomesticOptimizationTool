using Dot.DataAccess.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;

namespace Dot.Services.Extensions
{
    public static class LoggerExtensions
    {
        public static void ConfigureLogging(this IHostBuilder host, IConfiguration configuration)
        {
            var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.MongoDB($"{mongoDbSettings.ConnectionString}/{mongoDbSettings.DatabaseName}", collectionName: "logs")
                .Enrich.WithProperty("Application", Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownApp")
                .Enrich.FromLogContext()
                .CreateLogger();

            host.UseSerilog();
        }
    }
}
