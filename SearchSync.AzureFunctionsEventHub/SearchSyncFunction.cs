using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SearchSync.Common.EventContext.Adapters;
using SearchSync.Common.Services;
using System.Text;

namespace SearchSync.AzureFunctionsEventHub
{
    public class EventHubListenerFunction
    {
        private readonly ISearchIngestionService _searchIngestionService;
        private readonly IEventParser _eventParser;
        private readonly ILogger<EventHubListenerFunction> _logger;

        public EventHubListenerFunction(ISearchIngestionService searchIngestionService, IEventParser eventParser, ILogger<EventHubListenerFunction> logger)
        {
            _searchIngestionService = searchIngestionService;
            _eventParser = eventParser;
            _logger = logger;
        }

        [Function(nameof(EventHubListenerFunction))]
        public async Task Run([EventHubTrigger("YOUR_EVENTHUB_NAME_HERE", Connection = "EventHubConnectionString")] EventData[] events)
        {
            _logger.LogDebug($"Processing {events.Length} event(s)");

            var hasErrors = false;

            var tasks = events.Select(async @event =>
            {
                var eventBody = Encoding.UTF8.GetString(@event.Body.ToArray());
                var eventProperties = @event.Properties;
                _logger.LogDebug($"Received event with body: {eventBody} and event properties: {eventProperties}");

                try
                {
                    var context = new EventHubEventContext(@event);
                    var (eventType, payload) = _eventParser.Parse(context);
                    await _searchIngestionService.ProcessEvent(eventType, payload);
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    // swallow errors so the rest of events in the batch can still be processed
                    if (ex is ILoggableException loggable)
                    {
                        using (_logger.BeginScope(loggable.GetCustomDimensions()))
                        {
                            _logger.LogError(ex, loggable.GetLogMessage());
                        }
                    }
                    else
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            });

            await Task.WhenAll(tasks);

            if (hasErrors)
            {
                throw new Exception("One of the messages in this batch failed, please see logs for more details");
            }
        }
    }
}
