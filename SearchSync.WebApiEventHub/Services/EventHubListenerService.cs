using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using SearchSync.Common.EventContext.Adapters;
using SearchSync.Common.Services;
using System.Text;

namespace SearchSync.WebApiEventHub.Services
{
    /// <summary>
    /// Background service that listens to EventHub events and processes them.
    /// Runs continuously while the application is active.
    /// </summary>
    public class EventHubListenerService : BackgroundService
    {
        private readonly ILogger<EventHubListenerService> _logger;
        private readonly ISearchIngestionService _searchIngestionService;
        private readonly IEventParser _eventParser;
        private readonly EventHubOptions _eventHubOptions;
        private EventProcessorClient? _processor;

        public EventHubListenerService(
            ILogger<EventHubListenerService> logger,
            ISearchIngestionService searchIngestionService,
            IEventParser eventParser,
            IOptions<EventHubOptions> eventHubOptions)
        {
            _logger = logger;
            _searchIngestionService = searchIngestionService;
            _eventParser = eventParser;
            _eventHubOptions = eventHubOptions.Value;
        }

        /// <summary>
        /// Called when the service starts. Initializes and starts the EventHub listener.
        /// </summary>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("EventHub Listener Service starting...");

            try
            {
                await InitializeEventHubProcessor();
                await base.StartAsync(cancellationToken);
                _logger.LogInformation("EventHub Listener Service started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start EventHub Listener Service");
                throw;
            }
        }

        /// <summary>
        /// Called when the service stops. Gracefully shuts down the EventHub listener.
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("EventHub Listener Service stopping...");

            if (_processor != null)
            {
                await _processor.StopProcessingAsync();
            }

            await base.StopAsync(cancellationToken);
            _logger.LogInformation("EventHub Listener Service stopped");
        }

        /// <summary>
        /// Main execution method for the background service.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_processor == null)
            {
                _logger.LogError("EventHub processor not initialized");
                return;
            }

            // Keep the service running until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        /// <summary>
        /// Initializes the EventHub processor with event handlers.
        /// </summary>
        private async Task InitializeEventHubProcessor()
        {
            if (string.IsNullOrEmpty(_eventHubOptions.ConnectionString))
            {
                _logger.LogInformation("EventHub connection string not configured - EventHub listening will be disabled");
                return;
            }

            if (string.IsNullOrEmpty(_eventHubOptions.EventHubName))
            {
                _logger.LogInformation("EventHub name not configured - EventHub listening will be disabled");
                return;
            }

            if (string.IsNullOrEmpty(_eventHubOptions.StorageConnectionString))
            {
                _logger.LogInformation("Storage connection string not configured - EventHub listening will be disabled");
                return;
            }

            _logger.LogInformation("Initializing EventHub processor for EventHub: {EventHubName}", _eventHubOptions.EventHubName);

            // Create storage client for checkpoint management
            var storageConnectionString = _eventHubOptions.StorageConnectionString;
            var containerName = _eventHubOptions.StorageContainerName ?? "eventhub-checkpoints";
            
            BlobContainerClient? storageClient = null;
            
            if (!string.IsNullOrEmpty(storageConnectionString) && storageConnectionString != "UseDevelopmentStorage=true")
            {
                // Only try to initialize storage for real Azure accounts (not development storage)
                try
                {
                    var accountName = GetStorageAccountName(storageConnectionString);
                    var accountKey = GetStorageAccountKey(storageConnectionString);
                    
                    storageClient = new BlobContainerClient(
                        new Uri($"https://{accountName}.blob.core.windows.net/{containerName}"),
                        new Azure.Storage.StorageSharedKeyCredential(accountName, accountKey)
                    );
                    
                    await storageClient.CreateIfNotExistsAsync();
                    _logger.LogInformation("Azure Storage Account container '{ContainerName}' verified/created", containerName);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Invalid storage connection string format. Expected: 'DefaultEndpointsProtocol=https;AccountName=xxx;AccountKey=yyy;...'");
                    throw;
                }
                catch (Azure.RequestFailedException ex)
                {
                    _logger.LogError(ex, "Could not connect to Azure Storage Account - verify credentials and account exists");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not initialize storage container - continuing anyway");
                    storageClient = null;
                }
            }
            else if (storageConnectionString == "UseDevelopmentStorage=true")
            {
                // Development storage note
                _logger.LogInformation("Using development storage setting - for production, provide Azure Storage Account credentials");
            }
            else
            {
                _logger.LogWarning("Storage connection string is not configured");
            }

            if (storageClient == null)
            {
                _logger.LogError("Storage client could not be initialized - EventHub listening will be disabled");
                return;
            }

            // Create the event processor client
            var processorOptions = new EventProcessorClientOptions
            {
                Identifier = Environment.MachineName,
                MaximumWaitTime = TimeSpan.FromSeconds(1)
            };

            _processor = new EventProcessorClient(
                storageClient,
                _eventHubOptions.ConsumerGroup ?? "$Default",
                _eventHubOptions.ConnectionString,
                _eventHubOptions.EventHubName,
                processorOptions
            );

            // Register event handlers
            _processor.ProcessEventAsync += ProcessEventHandler;
            _processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processor
            await _processor.StartProcessingAsync();
            _logger.LogInformation("EventHub processor started successfully");
        }

        /// <summary>
        /// Handles individual EventHub events.
        /// </summary>
        private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            try
            {
                var eventBody = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
                _logger.LogDebug("Received event from EventHub: {EventBody}", eventBody);

                var context = new EventHubEventContext(eventArgs.Data);
                var (eventType, payload) = _eventParser.Parse(context);

                _logger.LogInformation("Processing event of type: {EventType}", eventType);
                await _searchIngestionService.ProcessEvent(eventType, payload);

                // Update checkpoint to mark this event as processed
                await eventArgs.UpdateCheckpointAsync();
                _logger.LogDebug("Successfully processed and checkpointed event of type: {EventType}", eventType);
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
                    _logger.LogError(ex, "Error processing EventHub event: {Message}", ex.Message);
                }
                // Don't throw - continue processing other events in the batch
            }
        }

        /// <summary>
        /// Handles errors that occur during EventHub processing.
        /// </summary>
        private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            _logger.LogError(
                eventArgs.Exception,
                "Error in EventHub processor - Operation: {Operation}, Partition: {Partition}",
                eventArgs.Operation,
                eventArgs.PartitionId ?? "unknown"
            );

            return Task.CompletedTask;
        }

        /// <summary>
        /// Extracts the storage account name from a connection string.
        /// </summary>
        private static string GetStorageAccountName(string connectionString)
        {
            var parts = connectionString.Split(';');
            var accountNamePart = parts.FirstOrDefault(p => p.StartsWith("AccountName="));
            if (accountNamePart == null)
            {
                throw new InvalidOperationException("Could not find AccountName in storage connection string");
            }
            return accountNamePart.Split('=')[1];
        }

        /// <summary>
        /// Extracts the storage account key from a connection string.
        /// </summary>
        private static string GetStorageAccountKey(string connectionString)
        {
            var parts = connectionString.Split(';');
            var accountKeyPart = parts.FirstOrDefault(p => p.StartsWith("AccountKey="));
            if (accountKeyPart == null)
            {
                throw new InvalidOperationException("Could not find AccountKey in storage connection string");
            }
            // Use IndexOf to find the first '=' and take everything after it
            // This handles keys that contain '=' characters (common in Base64)
            int indexOfEquals = accountKeyPart.IndexOf('=');
            return accountKeyPart.Substring(indexOfEquals + 1);
        }
    }
}
