using Dot.API.Hubs;
using Dot.API.Models;
using Dot.DataAccess.Extensions;
using Dot.Services;
using Dot.Services.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Dot.Services.Messaging.Extensions;
using Dot.Services.Ollama.Extensions;
using Dot.Services.Repositories;
using Dot.Services.Chat;

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

builder.Services.SetupDataAccess(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();


builder.Services.AddMessageSender(builder.Configuration);
builder.Services.AddOllamaClient(builder.Configuration);
builder.Services.AddSingleton<IAppSettings<ApiSettings>, AppSettings<ApiSettings>>();
builder.Services.AddSingleton<IRepository, Repository>();
builder.Services.AddSingleton<IConversationRepository, ConversationRepository>();
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
