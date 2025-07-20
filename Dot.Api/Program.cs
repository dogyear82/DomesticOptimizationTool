using Dot.API.Hubs;
using Dot.Services.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Dot.Services.Messaging.Extensions;
using Dot.Services.Ollama.Extensions;
using Dot.Repositories;
using Dot.Services.Chat;
using Dot.API.Models;
using Dot.Services;
using Dot.Tools;
using Dot.Tools.Search;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Host.ConfigureLogging(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddMessageSender(builder.Configuration);
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddOllamaClient(builder.Configuration);
var _ = typeof(WebSearch);
builder.Services.AddTools(client =>
{
    var httpClientOptions = builder.Configuration.GetSection("HttpClientOptions").Get<HttpClientOptions>();
	client.BaseAddress = new Uri(httpClientOptions.BaseAddress);
    foreach (var header in httpClientOptions.DefaultHeaders)
	{
		client.DefaultRequestHeaders.Add(header.Key, header.Value);
	}
});
builder.Services.AddSingleton<IAppSettings<ApiSettings>, AppSettings<ApiSettings>>();
builder.Services.AddSingleton<IChatSummarizer, ChatSummarizer>();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

var app = builder.Build();

app.UseCors();

app.UseResponseCompression();
app.MapHub<ChatHub>("/chathub");
app.MapHub<CoHostHub>("/cohosthub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
