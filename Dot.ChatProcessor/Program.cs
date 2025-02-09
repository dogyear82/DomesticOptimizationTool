using Dot.ChatProcessor;
using Dot.ChatProcessor.Handlers;
using Dot.ChatProcessor.Options;
using Dot.DataAccess.Extensions;
using Dot.Services.Configurations.Extensions;
using Dot.Services.Messaging.Extensions;
using Dot.Services.Messaging.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddApplicationSecrets();

builder.Services.Configure<MessagingOptions>(builder.Configuration.GetSection("MessagingOptions"));
builder.Services.AddMessageReceiver(builder.Configuration);
builder.Services.SetupDataAccess(builder.Configuration);
builder.Services.AddSingleton<IMessageHandler, InboundChatHandler>();

var host = builder.Build();
host.Run();
