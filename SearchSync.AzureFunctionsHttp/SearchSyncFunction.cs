using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SearchSync.Common.EventContext.Adapters;
using SearchSync.Common.Services;

namespace SearchSync.AzureFunctionsHttp
{
    public class SearchSyncFunction
    {
        private readonly ILogger<SearchSyncFunction> _logger;
        private readonly IEventParser _eventParser;
        private readonly ISearchIngestionService _searchIngestionService;

        public SearchSyncFunction(ILogger<SearchSyncFunction> logger, IEventParser eventParser, ISearchIngestionService searchIngestionService)
        {
            _logger = logger;
            _eventParser = eventParser;
            _searchIngestionService = searchIngestionService;
        }

        [Function(nameof(SearchSyncFunction))]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogDebug("HTTP trigger function received a request.");
            try
            {
                var requestBodyString = await req.ReadAsStringAsync();
                if (string.IsNullOrEmpty(requestBodyString))
                {
                    throw new Exception("Missing request body");
                }
                var context = new HttpEventContext(requestBodyString);
                var (eventType, payload) = _eventParser.Parse(context);
                await _searchIngestionService.ProcessEvent(eventType, payload);
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
                    _logger.LogError(ex, ex.Message);
                }
                throw;
            }
        }
    }
}
