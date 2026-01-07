using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SearchSync.Common.EventContext.Adapters;
using SearchSync.Common.Services;

namespace SearchSync.AzureFunctionsKafka
{
    public class KafkaListenerFunction
    {
        private readonly ISearchIngestionService _searchIngestionService;
        private readonly IEventParser _eventParser;
        private readonly ILogger<KafkaListenerFunction> _logger;

        public KafkaListenerFunction(ISearchIngestionService searchIngestionService, IEventParser eventParser, ILogger<KafkaListenerFunction> logger)
        {
            _searchIngestionService = searchIngestionService;
            _eventParser = eventParser;
            _logger = logger;
        }

        [Function("KafkaListenerFunction")]
        public async Task Run(
            [KafkaTrigger("BrokerList", "ordercloud-events", ConsumerGroup = "%KafkaConsumerGroup%")] string[] events)
        {
            _logger.LogDebug($"Processing {events.Length} event(s)");

            var hasErrors = false;

            var tasks = events.Select(async eventBody =>
            {
                _logger.LogDebug($"Received Kafka event with body: {eventBody}");

                try
                {
                    var context = new HttpEventContext(eventBody);
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
