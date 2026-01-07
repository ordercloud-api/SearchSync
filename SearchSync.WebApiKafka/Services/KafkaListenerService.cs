using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SearchSync.Common.EventContext.Adapters;
using SearchSync.Common.Services;
using System.Text;

namespace SearchSync.WebApiKafka.Services
{
    /// <summary>
    /// Background service that listens to Kafka events and processes them.
    /// Runs continuously while the application is active.
    /// </summary>
    public class KafkaListenerService : BackgroundService
    {
        private readonly ILogger<KafkaListenerService> _logger;
        private readonly ISearchIngestionService _searchIngestionService;
        private readonly IEventParser _eventParser;
        private readonly KafkaOptions _kafkaOptions;
        private IConsumer<string, string>? _consumer;

        public KafkaListenerService(
            ILogger<KafkaListenerService> logger,
            ISearchIngestionService searchIngestionService,
            IEventParser eventParser,
            IOptions<KafkaOptions> kafkaOptions)
        {
            _logger = logger;
            _searchIngestionService = searchIngestionService;
            _eventParser = eventParser;
            _kafkaOptions = kafkaOptions.Value;
        }

        /// <summary>
        /// Called when the service starts. Initializes and starts the Kafka listener.
        /// </summary>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Listener Service starting...");

            try
            {
                InitializeKafkaConsumer();
                await base.StartAsync(cancellationToken);
                _logger.LogInformation("Kafka Listener Service started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Kafka Listener Service");
                throw;
            }
        }

        /// <summary>
        /// Called when the service stops. Gracefully shuts down the Kafka listener.
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Listener Service stopping...");

            if (_consumer != null)
            {
                _consumer.Close();
                _consumer.Dispose();
            }

            await base.StopAsync(cancellationToken);
            _logger.LogInformation("Kafka Listener Service stopped");
        }

        /// <summary>
        /// Main execution method for the background service. Continuously consumes Kafka messages.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_consumer == null)
            {
                _logger.LogError("Kafka consumer not initialized");
                return;
            }

            // Keep consuming messages until cancellation is requested
            await Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(stoppingToken);

                        if (consumeResult.IsPartitionEOF)
                        {
                            _logger.LogDebug("Reached end of partition: {Partition} at offset {Offset}", 
                                consumeResult.Partition, consumeResult.Offset);
                            continue;
                        }

                        ProcessMessage(consumeResult.Message);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogDebug("Kafka consumer operation cancelled");
                        break;
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming Kafka message: {Error}", ex.Error.Reason);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error in Kafka consumer loop");
                    }
                }
            }, stoppingToken);
        }

        /// <summary>
        /// Initializes the Kafka consumer with configuration.
        /// </summary>
        private void InitializeKafkaConsumer()
        {
            if (string.IsNullOrEmpty(_kafkaOptions.BootstrapServers))
            {
                _logger.LogInformation("Kafka bootstrap servers not configured - Kafka listening will be disabled");
                return;
            }

            if (string.IsNullOrEmpty(_kafkaOptions.Topic))
            {
                _logger.LogInformation("Kafka topic not configured - Kafka listening will be disabled");
                return;
            }

            if (string.IsNullOrEmpty(_kafkaOptions.GroupId))
            {
                _logger.LogInformation("Kafka group ID not configured - Kafka listening will be disabled");
                return;
            }

            _logger.LogInformation("Initializing Kafka consumer for topic: {Topic}", _kafkaOptions.Topic);

            try
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = _kafkaOptions.BootstrapServers,
                    GroupId = _kafkaOptions.GroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    StatisticsIntervalMs = 5000,
                    SessionTimeoutMs = _kafkaOptions.SessionTimeoutMs ?? 6000,
                    FetchMinBytes = _kafkaOptions.FetchMinBytes ?? 1024,
                    FetchWaitMaxMs = _kafkaOptions.FetchWaitMaxMs ?? 100,
                    AllowAutoCreateTopics = false
                };

                // Configure SASL/SSL if provided (e.g., for Azure Event Hub)
                if (!string.IsNullOrEmpty(_kafkaOptions.SecurityProtocol))
                {
                    config.SecurityProtocol = Enum.Parse<SecurityProtocol>(_kafkaOptions.SecurityProtocol);
                    _logger.LogInformation("Kafka security protocol set to: {SecurityProtocol}", _kafkaOptions.SecurityProtocol);
                }

                if (!string.IsNullOrEmpty(_kafkaOptions.SaslMechanism))
                {
                    config.SaslMechanism = Enum.Parse<SaslMechanism>(_kafkaOptions.SaslMechanism);
                    config.SaslUsername = _kafkaOptions.SaslUsername;
                    config.SaslPassword = _kafkaOptions.SaslPassword;
                    _logger.LogInformation("Kafka SASL mechanism set to: {SaslMechanism}", _kafkaOptions.SaslMechanism);
                }

                _consumer = new ConsumerBuilder<string, string>(config)
                    .SetErrorHandler((_, e) =>
                    {
                        _logger.LogError("Kafka error: {Error}", e.Reason);
                    })
                    .SetStatisticsHandler((_, json) =>
                    {
                        _logger.LogDebug("Kafka statistics: {Stats}", json);
                    })
                    .Build();

                _consumer.Subscribe(_kafkaOptions.Topic);
                _logger.LogInformation("Kafka consumer initialized and subscribed to topic: {Topic}", _kafkaOptions.Topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Kafka consumer");
                throw;
            }
        }

        /// <summary>
        /// Processes a Kafka message by parsing and ingesting it into the search service.
        /// </summary>
        private void ProcessMessage(Message<string, string> message)
        {
            try
            {
                // Log message metadata for debugging
                var headerCount = message.Headers?.Count ?? 0;
                var headerNames = message.Headers != null 
                    ? string.Join(", ", message.Headers.Select(h => h.Key))
                    : "none";
                
                _logger.LogDebug("Received Kafka message - Key: {Key}, Headers: {HeaderCount} ({HeaderNames}), Body length: {BodyLength}",
                    message.Key, headerCount, headerNames, message.Value?.Length ?? 0);

                // For debugging, also log the first 500 chars of the message body
                if (!string.IsNullOrEmpty(message.Value))
                {
                    var bodyPreview = message.Value.Length > 500 
                        ? message.Value.Substring(0, 500) + "..." 
                        : message.Value;
                    _logger.LogDebug("Message body preview: {BodyPreview}", bodyPreview);
                }

                // Convert Kafka headers to dictionary for KafkaEventContext
                var headerDict = new Dictionary<string, byte[]>();
                if (message.Headers != null && message.Headers.Count > 0)
                {
                    foreach (var header in message.Headers)
                    {
                        headerDict[header.Key] = header.GetValueBytes();
                    }
                }

                var context = new KafkaEventContext(message.Key, message.Value, headerDict);
                var (eventType, payload) = _eventParser.Parse(context);

                _logger.LogInformation("Processing event of type: {EventType}", eventType);
                _searchIngestionService.ProcessEvent(eventType, payload);

                _logger.LogDebug("Successfully processed event of type: {EventType}", eventType);
            }
            catch (Exception ex)
            {
                if (ex is ILoggableException loggable)
                {
                    using (_logger.BeginScope(loggable.GetCustomDimensions()))
                    {
                        _logger.LogError(ex, loggable.GetLogMessage());
                    }
                }
                else
                {
                    _logger.LogError(ex, "Error processing Kafka message: {Message}", ex.Message);
                }
                // Don't throw - continue processing other messages
            }
        }
    }
}
