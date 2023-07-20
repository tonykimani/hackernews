using api.Services;
using libs.contracts;
using libs.Contracts;
using libs.Converters;
using libs.Providers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Extensions.Http;
using Serilog;


Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Hacker News Best Story API",
        Description = "Demo of how to use hacker news to retrieve best news stories in a production setting",
        TermsOfService = new Uri("https://en.wikipedia.org/wiki/MIT_License"),
        Contact = new OpenApiContact
        {
            Name = "Contact",
            Url = new Uri("mailto:tony@tksoftware.co.uk")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
        }
    });
});
builder.Services.AddCors();
builder.Services.AddHttpClient<INewsService, HackerNewsService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue("HACKER_NEWS_URI", "https://hacker-news.firebaseio.com")); //allow for versioning
    client.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue<double>("TIMEOUT_SECONDS", 60));

}).AddPolicyHandler(GetRetryPolicy());

builder.Services.AddSingleton<ICache, RedisCacheProvider>();
    

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseRouting();
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

Log.Information("Starting up API..");
app.Run();

/// Generically handle transient network issues with http client 
/// with 5 attempts with an exponential backoff of 2 seconds 
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}