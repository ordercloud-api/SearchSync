using SearchSync.Common;
using SearchSync.WebApiEventHub.Services;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings and environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();

// Add SearchSync services
builder.Services.AddSearchSync(options =>
{
    builder.Configuration.GetSection("SearchSettings").Bind(options);
});

// Register EventHub configuration
var eventHubConfig = builder.Configuration.GetSection("EventHub");
builder.Services.Configure<EventHubOptions>(eventHubConfig);

// Add EventHub listener as background service only if configured
if (!string.IsNullOrEmpty(eventHubConfig["ConnectionString"]))
{
    builder.Services.AddHostedService<EventHubListenerService>();
}

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Only use HTTPS redirect in production
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
