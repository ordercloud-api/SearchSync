using SearchSync.Common;
using SearchSync.WebApiKafka.Services;

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

// Register Kafka configuration
var kafkaConfig = builder.Configuration.GetSection("Kafka");
builder.Services.Configure<KafkaOptions>(kafkaConfig);

// Add Kafka listener as background service only if configured
if (!string.IsNullOrEmpty(kafkaConfig["BootstrapServers"]))
{
    builder.Services.AddHostedService<KafkaListenerService>();
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

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();
