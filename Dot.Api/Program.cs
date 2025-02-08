using Dot.API.Hubs;
using Dot.API.Models;
using Dot.DataAccess.Extensions;
using Dot.Services;
using Dot.Services.Extensions;
using Dot.Services.Secrets;
using Microsoft.AspNetCore.ResponseCompression;

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

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables(prefix: "VAULT_")
    .AddVault(options =>
    {
        var vaultOptions = builder.Configuration.GetSection("Vault").Get<VaultOptions>();
        options.Address = builder.Configuration.GetSection("VAULT_ADDR").Value;
        options.RoleId = builder.Configuration.GetSection("VAULT_ROLE_ID").Value;
        options.MountPath = vaultOptions.MountPath;
        options.SecretType = vaultOptions.SecretType;
        options.SecretId = builder.Configuration.GetSection("VAULT_SECRET_ID").Value;
        options.Secrets = vaultOptions.Secrets;
    });

builder.Host.ConfigureLogging(builder.Configuration);

builder.Services.SetupDataAccess(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddHttpClient();

builder.Services.AddMessageSender(builder.Configuration);
builder.Services.AddSingleton<IAppSettings<ApiSettings>, AppSettings<ApiSettings>>();

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
