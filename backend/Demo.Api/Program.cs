using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Graph;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON Serializer
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Add Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var azureAdConfig = builder.Configuration.GetSection("AzureAd");
        options.Authority = $"{azureAdConfig["Instance"]}{azureAdConfig["TenantId"]}/v2.0";
        options.Audience = azureAdConfig["ClientId"];
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Skip token validation for manual Graph API requests
                var path = context.HttpContext.Request.Path;
                if (path.StartsWithSegments("/api/manual-graph"))
                {
                    context.NoResult(); // Prevents JWT validation from being applied
                }
                return Task.CompletedTask;
            }
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Microsoft Graph Client
builder.Services.AddSingleton<GraphServiceClient>(sp =>
{
    var config = builder.Configuration.GetSection("AzureAd");
    var clientId = config["ClientId"];
    var tenantId = config["TenantId"];
    var clientSecret = config["ClientSecret"];

    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    return new GraphServiceClient(credential);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
